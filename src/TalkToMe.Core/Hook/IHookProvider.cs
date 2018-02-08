using System;

namespace TalkToMe.Core.Hook
{

    public interface IHookProvider
    {
        void Install(Func<KeyProcArgs, bool> callback);
        void Uninstall();
    }
}