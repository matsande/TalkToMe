using System;
using System.Collections.Generic;

namespace TalkToMe.Core.Voice
{
    public class MicrosoftSpeechVoiceFactory : IVoiceFactory
    {
        public static bool IsSupported
        {
            get
            {
                bool supported;
                try
                {
                    var temp = MicrosoftSpeechVoice.CreateDefault();
                    supported = temp != null;
                }
                catch (Exception)
                {
                    supported = false;
                }

                return supported;
            }
        }

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices => MicrosoftSpeechVoice.GetAvailableVoices();

        public VoiceProvider Provider => VoiceProvider.MicrosoftSpeech;

        public IVoice CreateDefault()
        {
            return MicrosoftSpeechVoice.CreateDefault();
        }

        public bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice)
        {
            return MicrosoftSpeechVoice.TryCreate(voiceDescriptor, out voice);
        }
    }
}