using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Forms;
using NSubstitute;
using TalkToMe.Core.Hook;
using TalkToMe.Core.Voice;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class TestClass
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
            var config = new Config(false, false, hotKeys, vd1, vd2);
            
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speech = Substitute.For<ISpeech>();

            var keyMonitor = Substitute.For<IKeyMonitor>();
            keyMonitor.KeysObservable.Returns(keySubject);

            var configStore = Substitute.For<IConfigPersistence>();

            var speechManager = new SpeechManager(clipboardMonitor, speech, keyMonitor, configStore, config);

            var text = "Just a test";
            textSubject.OnNext(text);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            speech.Received().Speak(text);
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
            var config = new Config(true, false, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var speech = Substitute.For<ISpeech>();
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();

            keyMonitor.KeysObservable.Returns(keySubject);

            var speechmanager = new SpeechManager(clipboardMonitor, speech, keyMonitor, configStore, config);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            speech.Received().Abort();
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
            var config = new Config(true, false, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var speech = Substitute.For<ISpeech>();
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            keyMonitor.KeysObservable.Returns(keySubject);
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speechmanager = new SpeechManager(clipboardMonitor, speech, keyMonitor, configStore, config);
            keySubject.OnNext(new KeyInfo(Keys.A, Keys.Shift));

            speech.Received().Abort();

            textSubject.OnNext("Test string");

            keySubject.OnNext(new KeyInfo(Keys.B, Keys.Shift));

            speech.DidNotReceive().Speak(Arg.Any<string>());
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
            var config = new Config(true, false, hotKeys, vd1, vd2);
            var clipboardMonitor = Substitute.For<IClipboardTextMonitor>();
            var speech = Substitute.For<ISpeech>();
            var keyMonitor = Substitute.For<IKeyMonitor>();
            var configStore = Substitute.For<IConfigPersistence>();
            var keySubject = new Subject<KeyInfo>();
            var textSubject = new Subject<string>();

            keyMonitor.KeysObservable.Returns(keySubject);
            clipboardMonitor.ClipboardTextObservable.Returns(textSubject);

            var speechmanager = new SpeechManager(clipboardMonitor, speech, keyMonitor, configStore, config);

            textSubject.OnNext("Test string");
            textSubject.OnNext("Test string");

            speech.Received(1).Speak(Arg.Any<string>());
        }
    }
}