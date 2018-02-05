using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class ConfigTest
    {
        [Fact]
        public void ShouldSerializeAsExpected()
        {
            var key1 = new KeyInfo(Keys.A, Keys.None);
            var key2 = new KeyInfo(Keys.B, Keys.None);

            var res = key1.Equals(key2);

            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                {new KeyInfo(Keys.A, Keys.None), CommandType.Speak },
                {new KeyInfo(Keys.B, Keys.None), CommandType.ToggleMute }
            };
            var config = new Config(true, hotKeys, "SomeLang", "OtherLang");

            Config newConfig;
            using (var ms = new MemoryStream())
            {
                config.Serialize(ms);
                ms.Seek(0, SeekOrigin.Begin);
                newConfig = Config.Deserialize(ms);
            }

            newConfig.Should().NotBeNull();
            newConfig.AutoMode.Should().Be(config.AutoMode);
            newConfig.Hotkeys.Should().Equal(config.Hotkeys);
            newConfig.PrimaryVoice.Should().Be(config.PrimaryVoice);
            newConfig.SecondaryVoice.Should().Be(config.SecondaryVoice);
        }

        [Fact]
        public void ShouldCreateNewInstanceForDifferentHotkeys()
        {
            var hotKeys = new Dictionary<KeyInfo, CommandType>
            {
                {new KeyInfo(Keys.A, Keys.None), CommandType.Speak },
                {new KeyInfo(Keys.B, Keys.None), CommandType.ToggleMute }
            };
            var configA = new Config(true, hotKeys, "SomeLang", "OtherLang");
            var newHotkeys = new Dictionary<KeyInfo, CommandType>
            {
                {new KeyInfo(Keys.C, Keys.None), CommandType.SwapLanguage }
            };
            var newConfig = configA.With(hotKeys: newHotkeys);

            newConfig.AutoMode.Should().Be(configA.AutoMode);
            newConfig.Hotkeys.Should().NotEqual(configA.Hotkeys);
            newConfig.PrimaryVoice.Should().Be(configA.PrimaryVoice);
            newConfig.SecondaryVoice.Should().Be(configA.SecondaryVoice);
        }
    }
}
