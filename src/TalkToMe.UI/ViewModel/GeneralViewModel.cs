using System;
using TalkToMe.Core;

namespace TalkToMe.UI.ViewModel
{
    public class GeneralViewModel : ViewModelBase
    {
        public GeneralViewModel(ISpeechManager speechManager)
        {
            this.speechManager = speechManager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(this.AutoMode));
            });
        }

        public bool AutoMode
        {
            get
            {
                return this.speechManager.Config.AutoMode;
            }

            set
            {
                this.speechManager.UpdateConfig(this.speechManager.Config.With(autoMode: value));
            }
        }

        //public bool Mute
        //{
        //    get
        //    {
        //        return this.speechManager.Mute;
        //    }

        //    set
        //    {
        //        this.speechManager.UpdateConfig(this.speechManager.Config.With(mute: value));
        //    }
        //}

        private readonly ISpeechManager speechManager;
    }
}