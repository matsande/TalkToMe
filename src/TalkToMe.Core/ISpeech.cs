namespace TalkToMe.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ISpeech" />
    /// </summary>
    public interface ISpeech
    {
        /// <summary>
        /// The SpeakAsync
        /// </summary>
        /// <param name="text">The <see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        void Speak(string text);
        void Abort();
        IReadOnlyCollection<string> AvailableVoices
        {
            get;
        }
        void SelectVoice(string voiceName);
    }
}
