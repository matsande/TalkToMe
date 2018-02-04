using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkToMe.Core;

namespace TalkToMe.UI.ViewModel
{
    public class SpeechManagerViewModel : INotifyPropertyChanged
    {
        private readonly SpeechManager speechManager;
        private readonly IDisposable stateChangeSubscription;

        public SpeechManagerViewModel(SpeechManager speechManager)
        {
            this.speechManager = speechManager;
            this.stateChangeSubscription = this.speechManager.StateChangeObservable.Subscribe(this.OnSpeechManagerStateChange);
        }

        public bool AutoMode
        {
            get
            {
                return this.speechManager.AutoMode;
            }

            set
            {
                this.speechManager.SetAutoMode(value);
            }
        }

        public IReadOnlyCollection<string> AvailableVoices => this.speechManager.AvailableVoices;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSpeechManagerStateChange(SpeechManagerStateChange stateChange)
        {
            this.RaisePropertyChanged(nameof(this.AutoMode));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
