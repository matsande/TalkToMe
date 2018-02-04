using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Forms;
using NSubstitute;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class TestClass
    {
        [Fact]
        public void ShouldInvokeExpectedCommand()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.Shift), CommandType.Speak }
            };

            var config = new Config(false, hotKeys, "TestLang", "OtherLang");
            
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
    }
}