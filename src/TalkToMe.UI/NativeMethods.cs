namespace TalkToMe.UI
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines the <see cref="NativeMethods" />
    /// </summary>
    internal static class NativeMethods
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;

        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        /// <summary>
        /// The AddClipboardFormatListener
        /// </summary>
        /// <param name="hwnd">The <see cref="IntPtr"/></param>
        /// <returns>The <see cref="bool"/></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
    }
}
