using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace TalkToMe.Core
{
    public class SpeechSynth : ISpeech
    {
        private readonly SpeechSynthesizer speech;

        public void Abort()
        {
            this.speech.SpeakAsyncCancelAll();
        }

        public void Speak(string text)
        {
            this.speech.SpeakAsync(text);
        }

        public void SelectVoice(string voiceName)
        {
            this.speech.SelectVoice(voiceName);
        }

        public SpeechSynth()
        {
            this.speech = new SpeechSynthesizer();
        }

        public IReadOnlyCollection<string> AvailableVoices =>
            this.speech.GetInstalledVoices()
            .Where(voice => voice.Enabled)
            .Select(voice => voice.VoiceInfo.Name).ToList();
    }
}
