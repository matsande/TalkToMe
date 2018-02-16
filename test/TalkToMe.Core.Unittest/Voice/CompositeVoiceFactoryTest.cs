using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TalkToMe.Core.Voice;
using Xunit;

namespace TalkToMe.Core.Unittest.Voice
{
    public class CompositeVoiceFactoryTest
    {
        [Fact]
        public void ShouldInvokeDesiredFactory()
        {
            var factory1 = Substitute.For<IVoiceFactory>();
            factory1.Provider.Returns(VoiceProvider.MicrosoftSpeech);
            factory1.TryCreate(Arg.Any<VoiceDescriptor>(), out var voice).Returns(true);

            var factory2 = Substitute.For<IVoiceFactory>();
            factory2.Provider.Returns(VoiceProvider.SystemSpeech);
            var factories = new[] { factory1, factory2 };
            var compositeFactory = new CompositeVoiceFactory(factories);

            var result = compositeFactory.TryCreate(new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "test"), out _);

            result.Should().BeTrue();
            factory1.Received().TryCreate(Arg.Any<VoiceDescriptor>(), out var voice1);
            factory2.DidNotReceive().TryCreate(Arg.Any<VoiceDescriptor>(), out var voice2);
        }

        [Fact]
        public void ShouldReturnAllAvailableVoices()
        {
            var factory1 = Substitute.For<IVoiceFactory>();
            factory1.Provider.Returns(VoiceProvider.MicrosoftSpeech);
            factory1.AvailableVoices.Returns(new List<VoiceDescriptor> { new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "test") });

            var factory2 = Substitute.For<IVoiceFactory>();
            factory2.Provider.Returns(VoiceProvider.SystemSpeech);
            factory2.AvailableVoices.Returns(new List<VoiceDescriptor> { new VoiceDescriptor(VoiceProvider.SystemSpeech, "test2") });
            var factories = new[] { factory1, factory2 };
            var compositeFactory = new CompositeVoiceFactory(factories);

            var availableVoices = compositeFactory.AvailableVoices;
            availableVoices.Count.Should().Be(2);
            availableVoices.Should().Contain(x => x.Provider == VoiceProvider.MicrosoftSpeech);
            availableVoices.Should().Contain(x => x.Provider == VoiceProvider.SystemSpeech);
        }
    }
}
