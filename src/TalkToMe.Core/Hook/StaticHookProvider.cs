using System;

namespace TalkToMe.Core.Hook
{

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