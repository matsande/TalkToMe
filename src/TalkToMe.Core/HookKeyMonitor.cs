namespace TalkToMe.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;


    public class HookKeyMonitor : IDisposable, IKeyMonitor
    {
        private readonly HashSet<KeyInfo> observedKeys;
        private readonly Subject<KeyInfo> keysSubject;
        private readonly IHookProvider hookProvider;
        private bool disposedValue = false; // To detect redundant calls

        // TODO: Create an abstraction for the actual hook and create unittests.
        public HookKeyMonitor(IEnumerable<KeyInfo> observedKeys, IHookProvider hookProvider)
        {
            this.observedKeys = new HashSet<KeyInfo>(observedKeys);
            this.keysSubject = new Subject<KeyInfo>();
            this.hookProvider = hookProvider;
            this.hookProvider.Install(this.OnKeyEvent);
        }

        public IObservable<KeyInfo> KeysObservable => this.keysSubject.AsObservable();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.keysSubject.Dispose();
                }

                this.hookProvider.Uninstall();

                disposedValue = true;
            }
        }

        ~HookKeyMonitor()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private bool OnKeyEvent(KeyProcArgs keyCodes)
        {
            var handled = false;

            try
            {
                if (keyCodes.WParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                {
                    NativeMethods.KBDLLHOOKSTRUCT kbd = (NativeMethods.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(keyCodes.LParam, typeof(NativeMethods.KBDLLHOOKSTRUCT));
                    var vkCode = kbd.vkCode;
                    var key = (Keys)vkCode;
                    //Debug.Print($"Got key: {key}");

                    if (IsModifier(key))
                    {
                        // Let modifiers pass through
                        handled = false;
                    }
                    else
                    {
                        var modkey = GetModifierState();
                        //Debug.Print($"{key} With modifiers: {modkey}");
                        var keyInfo = new KeyInfo(key, modkey);
                        if (this.observedKeys.Contains(keyInfo))
                        {
                            this.keysSubject.OnNext(keyInfo);
                            handled = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Catch all, protect the caller.
                // TODO: Trace
                handled = false;
            }

            return handled;
        }

        private bool KeyDown(IntPtr lParam)
        {
            var bits = lParam.ToInt32();
            return ((bits & NativeMethods.TRANSITION_STATE_BIT) == 0);
        }

        private static bool IsModifier(Keys key)
        {
            return
                key == Keys.ShiftKey ||
                key == Keys.ControlKey ||
                key == Keys.Menu || 
                key == Keys.Alt || 
                key == Keys.LWin || 
                key == Keys.RWin;
        }

        private static Keys GetModifierState()
        {
            // TODO: Move GetKeyState to an abstraction so this can be tested
            Keys key = Keys.None;
            key |= (NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? Keys.Shift : 0;
            key |= (NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? Keys.Control : 0;
            key |= (NativeMethods.GetKeyState(NativeMethods.VK_ALT) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? Keys.Alt : 0;
            key |= (NativeMethods.GetKeyState(NativeMethods.VK_LWIN) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? Keys.LWin : 0;
            key |= (NativeMethods.GetKeyState(NativeMethods.VK_RWIN) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? Keys.RWin : 0;

            return key;
        }
    }

    public struct KeyProcArgs
    {
        public KeyProcArgs(int code, IntPtr wParam, IntPtr lParam)
        {
            this.Code = code;
            this.WParam = wParam;
            this.LParam = lParam;
        }

        public int Code
        {
            get;
        }
        public IntPtr WParam
        {
            get;
        }
        public IntPtr LParam
        {
            get;
        }
    }

    public interface IHookProvider
    {
        void Install(Func<KeyProcArgs, bool> callback);
        void Uninstall();
    }

    public class StaticHookProvider : IHookProvider
    {
        public void Install(Func<KeyProcArgs, bool> callback)
        {
            InterceptKeys.Initialize(callback);
        }

        public void Uninstall()
        {
            InterceptKeys.Cleanup();
        }
    }
}
