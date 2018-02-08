using System;

namespace TalkToMe.Core.Hook
{

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
}