using System.Collections.Generic;
using System.Windows.Forms;

namespace TalkToMe.Core.Hook
{

    public class AsyncKeyStateModifierStateChecker : IModifierStateChecker
    {
        public Keys GetModifierState(Keys modifier)
        {
            Keys result;
            if (KeyMap.TryGetValue(modifier, out var vkCode))
            {
                result = (NativeMethods.GetAsyncKeyState(vkCode) & NativeMethods.HIGHORDER_KEYSTATE_BIT) > 0
                    ? modifier
                    : Keys.None;
            }
            else
            {
                result = Keys.None;
            }

            return result;
        }

        private static Dictionary<Keys, int> KeyMap = new Dictionary<Keys, int>
        {
            { Keys.LShiftKey,NativeMethods.VK_LSHIFT},
            { Keys.RShiftKey,NativeMethods.VK_RSHIFT},
            { Keys.LControlKey,NativeMethods.VK_LCONTROL},
            { Keys.RControlKey,NativeMethods.VK_RCONTROL},
            { Keys.Alt,NativeMethods.VK_ALT},
            { Keys.LWin,NativeMethods.VK_LWIN},
            { Keys.RWin,NativeMethods.VK_RWIN},
        };

    }
}