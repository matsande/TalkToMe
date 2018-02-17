using System;
using System.Collections.Generic;
using System.Linq;
using TalkToMe.Core;
using TalkToMe.Core.Voice;
using TalkToMe.UI.Common;

namespace TalkToMe.UI.ViewModel
{
    public class VoiceViewModel : ViewModelBase
    {
        public VoiceViewModel(ISpeechManager speechManager)
        {
            this.speechManager = speechManager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(this.PrimaryVoice));
                this.RaisePropertyChanged(nameof(this.SecondaryVoice));
                this.RaisePropertyChanged(nameof(this.CurrentVoice));
            });

            this.SetPrimaryVoice = new DelegateCommand(this.OnSetPrimaryVoice, this.CanSetPrimaryVoice);
            this.SetSecondaryVoice = new DelegateCommand(this.OnSetSecondaryVoice, this.CanSetSecondaryVoice);
            this.SelectedVoice = this.AvailableVoices.FirstOrDefault();
        }

        private bool CanSetSecondaryVoice()
        {
            return this.SelectedVoice != null;
        }

        private void OnSetSecondaryVoice()
        {
            this.speechManager.UpdateConfig(this.speechManager.Config.With(secondaryVoice: this.SelectedVoice));
        }

        private bool CanSetPrimaryVoice()
        {
            return this.SelectedVoice != null;
        }

        private void OnSetPrimaryVoice()
        {
            this.speechManager.UpdateConfig(this.speechManager.Config.With(primaryVoice: this.SelectedVoice));
        }

        public VoiceDescriptor CurrentVoice => this.speechManager.CurrentVoice;
        public VoiceDescriptor PrimaryVoice => this.speechManager.Config.PrimaryVoice;
        public VoiceDescriptor SecondaryVoice => this.speechManager.Config.SecondaryVoice;

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices => this.speechManager.AvailableVoices;

        public DelegateCommand SetPrimaryVoice { get; }
        public DelegateCommand SetSecondaryVoice { get; }
        public VoiceDescriptor SelectedVoice { get; set; }

        private readonly ISpeechManager speechManager;
    }
}