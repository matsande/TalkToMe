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
                this.RaisePropertyChanged(nameof(this.Mute));
                this.RaisePropertyChanged(nameof(this.AbortOnEscape));
                this.RaisePropertyChanged(nameof(this.OverrideWithNewText));
            });
        }

        public bool AutoMode
        {
            get => this.speechManager.Config.AutoMode;
            set => this.speechManager.UpdateConfig(this.speechManager.Config.With(autoMode: value));
        }

        public bool Mute
        {
            get => this.speechManager.Config.Mute;
            set => this.speechManager.UpdateConfig(this.speechManager.Config.With(mute: value));
        }

        public bool AbortOnEscape
        {
            get => this.speechManager.Config.AbortOnEscape;
            set => this.speechManager.UpdateConfig(this.speechManager.Config.With(abortOnEscape: value));
        }

        public bool OverrideWithNewText
        {
            get => this.speechManager.Config.OverrideWithNewText;
            set => this.speechManager.UpdateConfig(this.speechManager.Config.With(overrideWithNewText: value));
        }

        private readonly ISpeechManager speechManager;
    }
}