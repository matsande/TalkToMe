namespace TalkToMe.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Disposables;
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
        private Func<KeyInfo, bool> keyOverride;

        // TODO: Create an abstraction for the actual hook and create unittests.
        public HookKeyMonitor(IEnumerable<KeyInfo> observedKeys, IHookProvider hookProvider)
        {
            this.observedKeys = new HashSet<KeyInfo>(observedKeys);
            this.keysSubject = new Subject<KeyInfo>();
            this.hookProvider = hookProvider;
            this.hookProvider.Install(this.OnKeyEvent);
        }

        public IObservable<KeyInfo> KeysObservable => this.keysSubject.AsObservable();

        public IDisposable Override(Func<KeyInfo, bool> keyOverride)
        {
            this.keyOverride = keyOverride;
            return Disposable.Create(() => this.keyOverride = null);
        }

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
                if (keyCodes.WParam == (IntPtr)NativeMethods.WM_KEYDOWN || keyCodes.WParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN)
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
                        var overrideHandler = this.keyOverride;

                        if (overrideHandler != null)
                        {
                            handled = overrideHandler(keyInfo);
                        }
                        else if (this.observedKeys.Contains(keyInfo))
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
                key == Keys.LShiftKey ||
                key == Keys.RShiftKey ||
                key == Keys.RControlKey ||
                key == Keys.LControlKey ||
                key == Keys.LMenu ||
                key == Keys.RMenu ||
                key == Keys.LWin ||
                key == Keys.RWin;
        }

        private static Keys GetModifierState()
        {
            Keys CheckModifierState(int vkCode, Keys target)
            {
                return (NativeMethods.GetAsyncKeyState(vkCode) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0 ? target : 0;
            }

            // TODO: Move GetKeyState to an abstraction so this can be tested
            Keys key = Keys.None;
            key |= CheckModifierState(NativeMethods.VK_LSHIFT, Keys.LShiftKey);
            key |= CheckModifierState(NativeMethods.VK_RSHIFT, Keys.RShiftKey);
            key |= CheckModifierState(NativeMethods.VK_LCONTROL, Keys.LControlKey);
            key |= CheckModifierState(NativeMethods.VK_RCONTROL, Keys.RControlKey);
            key |= CheckModifierState(NativeMethods.VK_ALT, Keys.Alt);
            key |= CheckModifierState(NativeMethods.VK_LWIN, Keys.LWin);
            key |= CheckModifierState(NativeMethods.VK_RWIN, Keys.RWin);

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
