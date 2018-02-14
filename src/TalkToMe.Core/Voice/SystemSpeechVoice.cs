using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace TalkToMe.Core.Voice
{
    public class SystemSpeechVoice : IVoice
    {
        public static bool TryCreate(VoiceDescriptor descriptor, out IVoice voice)
        {
            if (descriptor.Provider != VoiceProvider.SystemSpeech)
            {
                throw new ArgumentException($"Invalid provider ({descriptor.Provider}) specified");
            }
            voice = null;
            var speech = new SpeechSynthesizer();
            if (speech.GetInstalledVoices().Any(v => v.Enabled && v.VoiceInfo.Name == descriptor.VoiceName))
            {
                voice = new SystemSpeechVoice(speech, descriptor);
            }

            return voice != null;
        }

        public VoiceDescriptor Descriptor { get; }

        public void Abort()
        {
            this.speech.SpeakAsyncCancelAll();
        }

        internal static IVoice CreateDefault()
        {
            return new SystemSpeechVoice(new SpeechSynthesizer());
        }

        public void Speak(string text)
        {
            this.speech.SpeakAsync(text);
        }

        private SystemSpeechVoice(SpeechSynthesizer speechSynthesizer)
            : this(speechSynthesizer, null)
        {
        }

        internal static IReadOnlyCollection<VoiceDescriptor> GetAvailableVoices()
        {
            var speech = new SpeechSynthesizer();
            return speech.GetInstalledVoices()
                .Where(voice => voice.Enabled)
                .Select(voice => new VoiceDescriptor(VoiceProvider.SystemSpeech, voice.VoiceInfo.Name))
                .ToList();
        }

        private SystemSpeechVoice(SpeechSynthesizer speechSynthesizer, VoiceDescriptor descriptor)
        {
            this.speech = speechSynthesizer;
            if (descriptor != null)
            {
                this.speech.SelectVoice(descriptor.VoiceName);
            }
            else
            {
                descriptor = new VoiceDescriptor(VoiceProvider.SystemSpeech, this.speech.Voice.Name);
            }

            this.Descriptor = descriptor;
        }

        private readonly SpeechSynthesizer speech;
    }
}