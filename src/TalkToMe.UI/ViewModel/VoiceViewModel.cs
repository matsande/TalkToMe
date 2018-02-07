﻿using System;
using System.Collections.Generic;
using TalkToMe.Core;

namespace TalkToMe.UI.ViewModel
{
    public class VoiceViewModel : ViewModelBase
    {
        public VoiceViewModel(SpeechManager speechManager)
        {
            this.speechManager = speechManager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
            });
        }

        public IReadOnlyCollection<string> AvailableVoices => this.speechManager.AvailableVoices;
        private readonly SpeechManager speechManager;
    }
}