using System;

namespace TalkToMe.UI.Common
{

    public class DelegateCommand<T> : DelegateCommandBase
    {
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
            : base(x => execute((T)x), x => canExecute == null ? true : canExecute((T)x))
        {
        }
    }
}