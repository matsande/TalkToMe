namespace TalkToMe.Core.Voice
{
    public interface IVoice
    {
        VoiceDescriptor Descriptor { get; }

        void Speak(string text);

        void Abort();
    }
}