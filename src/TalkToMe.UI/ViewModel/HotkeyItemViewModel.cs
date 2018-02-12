using TalkToMe.Core;
using TalkToMe.Core.Hook;

namespace TalkToMe.UI.ViewModel
{

    public class HotkeyItemViewModel : ViewModelBase
    {
        private KeyInfo keyInfo;

        public HotkeyItemViewModel(KeyInfo keyInfo, CommandType command)
        {
            this.keyInfo = keyInfo;
            this.Command = command;
        }

        public CommandType Command {get;}

        public KeyInfo Key
        {
            get
            {
                return this.keyInfo; 
            }

            set
            {
                this.keyInfo = value;
                this.RaisePropertyChanged(nameof(this.Key));
            }
        }
    }
}