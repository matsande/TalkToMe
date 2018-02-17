using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Speech.Synthesis;

namespace TalkToMe.Core.Voice
{
    public class MicrosoftSpeechVoice : IVoice
    {
        public VoiceDescriptor Descriptor { get; }

        public void Abort()
        {
            this.speech.SpeakAsyncCancelAll();
        }

        public void Speak(string text)
        {
            this.speech.SpeakAsync(text);
        }

        internal static IReadOnlyCollection<VoiceDescriptor> GetAvailableVoices()
        {
            var speech = new SpeechSynthesizer();
            return speech.GetInstalledVoices()
                .Where(voice => voice.Enabled)
                .Select(voice => new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, voice.VoiceInfo.Name))
                .ToList();
        }

        internal static bool TryCreate(VoiceDescriptor descriptor, out IVoice voice)
        {
            if (descriptor.Provider != VoiceProvider.MicrosoftSpeech)
            {
                throw new ArgumentException($"Invalid provider ({descriptor.Provider}) specified");
            }

            voice = null;

            var speech = new SpeechSynthesizer();
            if (speech.GetInstalledVoices().Any(v => v.Enabled && v.VoiceInfo.Name == descriptor.VoiceName))
            {
                voice = new MicrosoftSpeechVoice(speech, descriptor);
            }

            return voice != null;
        }

        internal static IVoice CreateDefault()
        {
            return new MicrosoftSpeechVoice(new SpeechSynthesizer(), null);
        }

        private MicrosoftSpeechVoice(SpeechSynthesizer speechSynthesizer, VoiceDescriptor descriptor)
        {
            this.speech = speechSynthesizer;
            this.speech.SetOutputToDefaultAudioDevice();
            if (descriptor == null)
            {
                descriptor = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, this.speech.Voice.Name);
            }

            this.Descriptor = descriptor;
        }

        private readonly SpeechSynthesizer speech;
    }
}