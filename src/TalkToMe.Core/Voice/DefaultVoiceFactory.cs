using System.Collections.Generic;
using System.Linq;

namespace TalkToMe.Core.Voice
{
    public class DefaultVoiceFactory : IVoiceFactory
    {
        public bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice)
        {
            voice = null;

            switch (voiceDescriptor.Provider)
            {
                case VoiceProvider.SystemSpeech:
                    SystemSpeechVoice.TryCreate(voiceDescriptor, out voice);
                    break;
                case VoiceProvider.MicrosoftSpeech:
                    MicrosoftSpeechVoice.TryCreate(voiceDescriptor, out voice);
                    break;
                default:
                    // TODO: Trace
                    break;
            }

            return voice != null;
        }

        public IVoice CreateDefault()
        {
            return SystemSpeechVoice.CreateDefault();
        }

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices
        {
            get
            {
                return SystemSpeechVoice.GetAvailableVoices()
                    .Concat(MicrosoftSpeechVoice.GetAvailableVoices())
                    .ToList();
            }
        }
    }
}