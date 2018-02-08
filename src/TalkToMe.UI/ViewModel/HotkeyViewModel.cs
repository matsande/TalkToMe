﻿using System;
using TalkToMe.Core;

namespace TalkToMe.UI.ViewModel
{
    public class HotkeyViewModel : ViewModelBase
    {
        public HotkeyViewModel(ISpeechManager speechmanager, IKeyMonitor keyMonitor)
        {
            this.KeyMonitor = keyMonitor;
            this.speechManager = speechmanager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
            });
        }

        public IKeyMonitor KeyMonitor { get; }

        private readonly ISpeechManager speechManager;
    }
}