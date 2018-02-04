namespace TalkToMe.Core
{
    using System;

    /// <summary>
    /// Defines the <see cref="InterceptKeys" />
    /// </summary>
    internal class InterceptKeys
    {
        /// <summary>
        /// The Initialize
        /// </summary>
        /// <param name="keyEventHandler">The <see cref="Func{(int code, IntPtr wparam, IntPtr lparam), bool}"/></param>
        public static void Initialize(Func<KeyProcArgs, bool> keyEventHandler)
        {
            if (_hookID != IntPtr.Zero)
            {
                // TODO: Trace warning
                Cleanup();
            }

            onKeyEvent = keyEventHandler;
            _hookID = SetHook(_proc);
        }

        /// <summary>
        /// The Cleanup
        /// </summary>
        public static void Cleanup()
        {
            if (_hookID != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        /// <summary>
        /// The SetHook
        /// </summary>
        /// <param name="proc">The <see cref="NativeMethods.LowLevelKeyboardProc"/></param>
        /// <returns>The <see cref="IntPtr"/></returns>
        private static IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
        }

        /// <summary>
        /// The HookCallback
        /// </summary>
        /// <param name="nCode">The <see cref="int"/></param>
        /// <param name="wParam">The <see cref="IntPtr"/></param>
        /// <param name="lParam">The <see cref="IntPtr"/></param>
        /// <returns>The <see cref="IntPtr"/></returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var handled = onKeyEvent(new KeyProcArgs(nCode, wParam, lParam));
            return handled
                ? (IntPtr)1
                : NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static NativeMethods.LowLevelKeyboardProc _proc = HookCallback;

        private static IntPtr _hookID = IntPtr.Zero;

        private static Func<KeyProcArgs, bool> onKeyEvent;
    }
}