using System;
using System.ComponentModel;
using System.Windows;
using TalkToMe.Core;
using TalkToMe.UI.View;

namespace TalkToMe.UI.ViewModel
{
    public class SpeechManagerViewModel : INotifyPropertyChanged
    {
        public SpeechManagerViewModel(SpeechManager speechManager, IKeyMonitor keyMonitor)
        {
            this.speechManager = speechManager;
            this.GeneralView = new GeneralView(new GeneralViewModel(speechManager));
            this.VoiceView = new VoiceView(new VoiceViewModel(speechManager));
            this.HotkeyView = new HotkeyView(new HotkeyViewModel(speechManager, keyMonitor));

            this.stateChangeSubscription = this.speechManager.StateChangeObservable.Subscribe(this.OnSpeechManagerStateChange);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement GeneralView { get; }

        public FrameworkElement VoiceView { get; }

        public FrameworkElement HotkeyView { get; }

        private void OnSpeechManagerStateChange(SpeechManagerStateChange stateChange)
        {
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly SpeechManager speechManager;
        private readonly IDisposable stateChangeSubscription;
    }
}