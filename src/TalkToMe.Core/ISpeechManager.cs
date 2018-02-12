using System;
using System.Collections.Generic;

namespace TalkToMe.Core
{
    public interface ISpeechManager
    {
        //TODO: Restore IReadOnlyCollection<string> AvailableVoices { get; }
        IObservable<SpeechManagerStateChange> StateChangeObservable { get; }
        Config Config { get; }

        void UpdateConfig(Config config);
    }
}