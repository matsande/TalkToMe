using System;
using System.Collections.Generic;

namespace TalkToMe.Core
{
    public interface ISpeechManager
    {
        bool AutoMode
        {
            get;
        }
        IReadOnlyCollection<string> AvailableVoices
        {
            get;
        }
        IObservable<SpeechManagerStateChange> StateChangeObservable
        {
            get;
        }

        void SetAutoMode(bool automode);
    }
}