using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using TalkToMe.Core;
using TalkToMe.UI.View;

namespace TalkToMe.UI.ViewModel
{
    public class SpeechManagerViewModel : INotifyPropertyChanged
    {
        public SpeechManagerViewModel(SpeechManager speechManager)
        {
            this.speechManager = speechManager;
            this.generalView = new GeneralView(new GeneralViewModel(speechManager));
            this.voiceView = new VoiceView(new VoiceViewModel(speechManager));
            this.hotkeyView = new HotkeyView(new HotkeyViewModel(speechManager));

            this.stateChangeSubscription = this.speechManager.StateChangeObservable.Subscribe(this.OnSpeechManagerStateChange);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement GeneralView => this.generalView;
        public FrameworkElement VoiceView => this.voiceView;
        public FrameworkElement HotkeyView => this.hotkeyView;

        private void OnSpeechManagerStateChange(SpeechManagerStateChange stateChange)
        {
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly SpeechManager speechManager;
        private readonly GeneralView generalView;
        private readonly VoiceView voiceView;
        private readonly HotkeyView hotkeyView;
        private readonly IDisposable stateChangeSubscription;
    }
}