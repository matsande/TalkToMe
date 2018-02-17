using System.Collections.Generic;

namespace TalkToMe.Core.Voice
{
    public interface IVoiceFactory
    {
        IReadOnlyCollection<VoiceDescriptor> AvailableVoices { get; }

        VoiceProvider Provider { get; }

        bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice);

        IVoice CreateDefault();
    }
}