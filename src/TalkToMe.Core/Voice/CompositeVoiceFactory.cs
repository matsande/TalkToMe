using System.Collections.Generic;
using System.Linq;

namespace TalkToMe.Core.Voice
{
    public class CompositeVoiceFactory : IVoiceFactory
    {
        public CompositeVoiceFactory(IEnumerable<IVoiceFactory> factories)
        {
            this.factories = factories.ToDictionary(x => x.Provider, x => x);
        }

        public VoiceProvider Provider => VoiceProvider.None;

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices
        {
            get
            {
                return this.factories.Values
                    .SelectMany(x => x.AvailableVoices)
                    .ToList();
            }
        }

        public bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice)
        {
            voice = null;
            return this.factories.TryGetValue(voiceDescriptor.Provider, out var factory) &&
                   factory.TryCreate(voiceDescriptor, out voice);
        }

        public IVoice CreateDefault()
        {
            return SystemSpeechVoice.CreateDefault();
        }

        private readonly Dictionary<VoiceProvider, IVoiceFactory> factories;
    }
}