namespace TalkToMe.Core.Voice
{
    public interface IVoiceFactory
    {
        bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice);
        IVoice CreateDefault();
    }
}