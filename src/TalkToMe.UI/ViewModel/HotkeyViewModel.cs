using System;
using TalkToMe.Core;

namespace TalkToMe.UI.ViewModel
{
    public class HotkeyViewModel : ViewModelBase
    {
        public HotkeyViewModel(SpeechManager speechmanager)
        {
            this.speechManager = speechmanager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
            });
        }

        private readonly SpeechManager speechManager;
    }
}