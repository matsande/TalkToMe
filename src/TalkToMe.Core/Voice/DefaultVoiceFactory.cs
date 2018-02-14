using System.Collections.Generic;

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
                // TODO: Add support
                //case VoiceProvider.MicrosoftSpeech:
                //    MicrosoftSpeechVoice.TryCreate(voiceDescriptor.VoiceName, out voice);
                //    break;
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
                // TODO: Concat with MicrosoftSpeechVoice.GetAvailableVoices();
                return SystemSpeechVoice.GetAvailableVoices();
            }
        }
    }
}