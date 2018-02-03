using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

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

        public SpeechSynth()
        {
            this.speech = new SpeechSynthesizer();
        }
    }
}
