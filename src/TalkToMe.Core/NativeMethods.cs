using System;
using System.Runtime.InteropServices;

namespace TalkToMe.Core
{
    internal static class NativeMethods
    {
        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern ushort GetKeyState(int vkKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetKeyboardState(byte[] keyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern ushort GetAsyncKeyState(int vkKey);

        internal const int WH_KEYBOARD = 2;
        internal const int WH_KEYBOARD_LL = 13;
        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_SYSKEYDOWN = 0x0104;

        internal const int VK_SHIFT = 0x10;
        internal const int VK_LSHIFT = 0xA0;
        internal const int VK_RSHIFT = 0xA1;
        internal const int VK_CONTROL = 0x11;
        internal const int VK_LCONTROL = 0xA2;
        internal const int VK_RCONTROL = 0xA3;
        internal const int VK_ALT = 0x12;
        internal const int VK_LWIN = 0x5B;
        internal const int VK_RWIN = 0x5C;

        internal const int HC_NOREMOVE = 3;
        internal const int TRANSITION_STATE_BIT = (0x1 << 31);
        internal const int HIGHORDER_KEYSTATE_BIT = 0x8000;
    }
}