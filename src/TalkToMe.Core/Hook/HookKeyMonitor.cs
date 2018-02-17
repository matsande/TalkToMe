using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace TalkToMe.Core.Hook
{


    public class HookKeyMonitor : IDisposable, IKeyMonitor
    {
        private HashSet<KeyInfo> observedKeys;
        private readonly Subject<KeyInfo> keysSubject;
        private readonly IHookProvider hookProvider;
        private bool disposedValue = false; // To detect redundant calls
        private Func<KeyInfo, bool> keyOverride;

        public HookKeyMonitor(IEnumerable<KeyInfo> observedKeys, IHookProvider hookProvider, IModifierStateChecker modifierStateChecker)
        {
            this.UpdateObservedKeys(observedKeys);
            this.keysSubject = new Subject<KeyInfo>();
            this.hookProvider = hookProvider;
            this.modifierStateChecker = modifierStateChecker;
            this.keyhandlers = new ConcurrentDictionary<KeyInfo, KeyHandler>();
            this.hookProvider.Install(this.OnKeyEvent);
        }

        public IObservable<KeyInfo> KeysObservable => this.keysSubject.AsObservable();

        public IDisposable Override(Func<KeyInfo, bool> keyOverride)
        {
            this.keyOverride = keyOverride;
            return Disposable.Create(() => this.keyOverride = null);
        }

        public void UpdateObservedKeys(IEnumerable<KeyInfo> observedKeys)
        {
            this.observedKeys = new HashSet<KeyInfo>(observedKeys.Where(k => k != KeyInfo.Empty));
        }

        public void AddOrUpdateKeyHandler(KeyHandler handler)
        {
            this.keyhandlers.AddOrUpdate(handler.Key, handler, (k, v) => handler);
        }

        public void RemoveKeyHandler(KeyHandler handler)
        {
            this.keyhandlers.TryRemove(handler.Key, out _);
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
                        var targetKeys = this.observedKeys;

                        if (overrideHandler != null)
                        {
                            handled = overrideHandler(keyInfo);
                        }
                        else if (this.keyhandlers.TryGetValue(keyInfo, out var handler))
                        {
                            handled = handler.Handled();
                        }

                        if (!handled && targetKeys.Contains(keyInfo))
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

        private Keys GetModifierState()
        {
            var key = Keys.None;
            key |= this.modifierStateChecker.GetModifierState(Keys.LShiftKey);
            key |= this.modifierStateChecker.GetModifierState(Keys.RShiftKey);
            key |= this.modifierStateChecker.GetModifierState(Keys.LControlKey);
            key |= this.modifierStateChecker.GetModifierState(Keys.RControlKey);
            key |= this.modifierStateChecker.GetModifierState(Keys.Alt);
            key |= this.modifierStateChecker.GetModifierState(Keys.LWin);
            key |= this.modifierStateChecker.GetModifierState(Keys.RWin);

            return key;
        }

        private IModifierStateChecker modifierStateChecker;
        private readonly ConcurrentDictionary<KeyInfo, KeyHandler> keyhandlers;
    }

    public class KeyHandler
    {
        public KeyHandler(KeyInfo key, Func<bool> handler)
        {
            this.Key = key;
            this.handler = handler;
        }

        public bool Handled()
        {
            return this.handler();
        }

        public KeyInfo Key { get; }

        private readonly Func<bool> handler;
    }
}
