using System.Windows.Forms;

namespace TalkToMe.Core.Hook
{

    public interface IModifierStateChecker
    {
        Keys GetModifierState(Keys modifier);
    }
}