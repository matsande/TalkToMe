namespace TalkToMe.Core.Voice
{
    public interface IVoice
    {
        void Speak(string text);

        void Abort();
    }
}