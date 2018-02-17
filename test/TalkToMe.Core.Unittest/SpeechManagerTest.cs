using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Forms;
using FluentAssertions;
using NSubstitute;
using TalkToMe.Core.Hook;
using TalkToMe.Core.Voice;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class SpeechManagerTest
    {
        [Fact]
        public void ShouldInvokeSpeakCommand()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.Speak }
            };

            var vd1 = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "TestVoice");
            var vd2 = VoiceDescriptor.Empty;
            var config = new Config(false, false, true, true, hotKeys, vd1, vd2);

            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var voice = Substitute.For<IVoice>();

            var voiceFactory = CreateSimpleVoiceFactory(voice);

            var keyMonitor = Substitute.For<IKeyMonitor>();
            keyMonitor.KeysObservable.Returns(keySubject);

            var configStore = Substitute.For<IConfigPersistence>();

            var speechManager = new SpeechManager(clipboardMonitor, voiceFactory, keyMonitor, configStore, config);

            var text = "Just a test";
            textSubject.OnNext(text);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            voice.Received().Speak(text);
        }

        [Fact]
        public void ShouldInvokeToggleMuteCommand()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.ToggleMute }
            };

            var vd1 = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "TestVoice");
            var vd2 = VoiceDescriptor.Empty;
            var config = new Config(true, false, true, true, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var voice = Substitute.For<IVoice>();

            var voiceFactory = CreateSimpleVoiceFactory(voice);
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();

            keyMonitor.KeysObservable.Returns(keySubject);

            var speechmanager = new SpeechManager(clipboardMonitor, voiceFactory, keyMonitor, configStore, config);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            voice.Received().Abort();
        }

        [Fact]
        public void ShouldNotInvokeSpeakWhenMuted()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.ToggleMute },
                { new KeyInfo(Keys.B, Keys.Shift), CommandType.Speak }
            };

            var vd1 = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "TestVoice");
            var vd2 = VoiceDescriptor.Empty;
            var config = new Config(true, false, true, true, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var voice = Substitute.For<IVoice>();
            var voiceFactory = CreateSimpleVoiceFactory(voice);

            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            keyMonitor.KeysObservable.Returns(keySubject);
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speechmanager = new SpeechManager(clipboardMonitor, voiceFactory, keyMonitor, configStore, config);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            voice.Received().Abort();

            textSubject.OnNext("Test string");

            keySubject.OnNext(new KeyInfo(Keys.B, Keys.Shift));

            voice.DidNotReceive().Speak(Arg.Any<string>());
        }

        [Fact]
        public void ShouldNotAutomaticallySpeakUnchangedText()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.ToggleMute },
                { new KeyInfo(Keys.B, Keys.Shift), CommandType.Speak }
            };

            var vd1 = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "TestVoice");
            var vd2 = VoiceDescriptor.Empty;
            var config = new Config(true, false, true, true, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var voice = Substitute.For<IVoice>();

            var voiceFactory = CreateSimpleVoiceFactory(voice);
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            keyMonitor.KeysObservable.Returns(keySubject);
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speechmanager = new SpeechManager(clipboardMonitor, voiceFactory, keyMonitor, configStore, config);

            textSubject.OnNext("Test string");
            textSubject.OnNext("Test string");

            voice.Received(1).Speak(Arg.Any<string>());
        }

        [Fact]
        public void ShouldNotChangeCurrentVoiceWithUpdatingNonVoiceRelatedConfig()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.SwapLanguage },
            };

            var vd1 = new VoiceDescriptor(VoiceProvider.MicrosoftSpeech, "TestVoice");
            var vd2 = new VoiceDescriptor(VoiceProvider.SystemSpeech, "OtherVoice");
            var config = new Config(true, false, true, true, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var voice1 = Substitute.For<IVoice>();
            voice1.Descriptor.Returns(vd1);
            var voice2 = Substitute.For<IVoice>();
            voice2.Descriptor.Returns(vd2);

            var voiceFactory = new MockVoiceFactory(vd =>
            {
                if (vd.Provider == VoiceProvider.MicrosoftSpeech)
                {
                    return voice1;
                }
                else
                {
                    return voice2;
                }
            });
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            keyMonitor.KeysObservable.Returns(keySubject);
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speechmanager = new SpeechManager(clipboardMonitor, voiceFactory, keyMonitor, configStore, config);

            // Should initially be voice1
            speechmanager.CurrentVoice.Should().Be(vd1);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            // After a swap voice command, voice shoud be change to voice2
            speechmanager.CurrentVoice.Should().Be(vd2);

            speechmanager.UpdateConfig(config.With(abortOnEscape: false));

            // Current voice should remain unchanged
            speechmanager.CurrentVoice.Should().Be(vd2);
        }

        private static IVoiceFactory CreateSimpleVoiceFactory(IVoice returnedVoice)
        {
            var voiceFactory = Substitute.For<IVoiceFactory>();
            voiceFactory.TryCreate(Arg.Any<VoiceDescriptor>(), out var temp)
                .Returns(x =>
                {
                    x[1] = returnedVoice;
                    return true;
                });

            return voiceFactory;
        }
        
        private static IVoiceFactory CreateVoiceFactory(Func<VoiceDescriptor, IVoice> factory)
        {
            var voiceFactory = Substitute.For<IVoiceFactory>();
            voiceFactory.TryCreate(Arg.Any<VoiceDescriptor>(), out var temp)
                .Returns(x =>
                {
                    x[1] = factory(x[0] as VoiceDescriptor);
                    return true;
                });

            return voiceFactory;
        }
    }

    // TODO: Revisit why the 'CreateVoiceFactory' only seems to work once.
    public class MockVoiceFactory : IVoiceFactory
    {
        private Func<VoiceDescriptor, IVoice> factory;

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices => new List<VoiceDescriptor>();

        public VoiceProvider Provider => VoiceProvider.None;

        public IVoice CreateDefault()
        {
            throw new NotImplementedException();
        }

        public bool TryCreate(VoiceDescriptor voiceDescriptor, out IVoice voice)
        {
            voice = this.factory(voiceDescriptor);
            return voice != null;
        }

        public MockVoiceFactory(Func<VoiceDescriptor, IVoice> factory)
        {
            this.factory = factory;
        }
    }
}