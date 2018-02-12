using System;

namespace TalkToMe.UI.Common
{

    public class DelegateCommand : DelegateCommandBase
    {
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
            : base(_ => execute(), _ => canExecute == null ? true : canExecute())
        {
        }
    }
}