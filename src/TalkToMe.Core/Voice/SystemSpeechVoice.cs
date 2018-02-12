using System.Linq;
using System.Speech.Synthesis;

namespace TalkToMe.Core.Voice
{
    public class SystemSpeechVoice : IVoice
    {
        public static bool TryCreate(string voiceName, out IVoice voice)
        {
            voice = null;
            var speech = new SpeechSynthesizer();
            if (speech.GetInstalledVoices().Any(v => v.Enabled && v.VoiceInfo.Name == voiceName))
            {
                voice = new SystemSpeechVoice(speech, voiceName);
            }

            return voice != null;
        }

        public void Abort()
        {
            this.speech.SpeakAsyncCancelAll();
        }

        public void Speak(string text)
        {
            this.speech.SpeakAsync(text);
        }

        private SystemSpeechVoice(SpeechSynthesizer speechSynthesizer, string voiceName)
        {
            this.speech = speechSynthesizer;
            this.speech.SelectVoice(voiceName);
        }

        private readonly SpeechSynthesizer speech;
    }
}