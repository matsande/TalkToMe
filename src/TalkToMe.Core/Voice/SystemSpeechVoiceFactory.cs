using System.Collections.Generic;

namespace TalkToMe.Core.Voice
{

    public class SystemSpeechVoiceFactory : IVoiceFactory
    {
        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices => SystemSpeechVoice.GetAvailableVoices();

        public VoiceProvider Provider => VoiceProvider.SystemSpeech;

        public IVoice CreateDefault()
        {
            return SystemSpeechVoice.CreateDefault();
        }

        public bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice)
        {
            return SystemSpeechVoice.TryCreate(voiceDescriptor, out voice);
        }
    }
}