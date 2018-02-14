using System;
using System.Collections.Generic;
using TalkToMe.Core.Voice;

namespace TalkToMe.Core
{
    public interface ISpeechManager
    {
        VoiceDescriptor CurrentVoice { get; }
        IReadOnlyCollection<VoiceDescriptor> AvailableVoices { get; }
        IObservable<SpeechManagerStateChange> StateChangeObservable { get; }
        Config Config { get; }

        void UpdateConfig(Config config);
    }
}