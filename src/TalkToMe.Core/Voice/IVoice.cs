namespace TalkToMe.Core.Voice
{
    public interface IVoice
    {
        VoiceDescriptor Descriptor { get; }

        bool IsSpeaking { get; }

        void Speak(string text);

        void Abort();
    }
}