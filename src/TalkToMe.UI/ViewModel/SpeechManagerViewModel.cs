using System;
using System.ComponentModel;
using TalkToMe.Core;
using TalkToMe.Core.Hook;

namespace TalkToMe.UI.ViewModel
{
    public class SpeechManagerViewModel : INotifyPropertyChanged
    {
        public SpeechManagerViewModel(ISpeechManager speechManager, IKeyMonitor keyMonitor)
        {
            this.speechManager = speechManager;

            this.stateChangeSubscription = this.speechManager.StateChangeObservable.Subscribe(this.OnSpeechManagerStateChange);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSpeechManagerStateChange(SpeechManagerStateChange stateChange)
        {
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ISpeechManager speechManager;
        private readonly IDisposable stateChangeSubscription;
    }
}