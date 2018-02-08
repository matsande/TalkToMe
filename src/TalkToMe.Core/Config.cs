using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TalkToMe.Core.Hook;

namespace TalkToMe.Core
{

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Config 
    {
        public bool AutoMode => this.autoMode;
        public Dictionary<KeyInfo, CommandType> Hotkeys => this.hotkeys;
        public string PrimaryVoice => this.primaryVoice;
        public string SecondaryVoice => this.secondaryVoice;

        public Config With(bool? autoMode = null,
            Dictionary<KeyInfo, CommandType> hotKeys = null,
            string primaryVoice = null,
            string secondaryVoice = null)
        {
            var sameInstance =
                (this.AutoMode == autoMode || autoMode == null) &&
                (this.Hotkeys == hotKeys || hotKeys == null) &&
                (this.PrimaryVoice == primaryVoice || primaryVoice == null) &&
                (this.SecondaryVoice == secondaryVoice || secondaryVoice == null);

            return sameInstance
                ? this
                : new Config(
                    autoMode ?? this.AutoMode, 
                    hotKeys ?? this.Hotkeys, 
                    primaryVoice ?? this.PrimaryVoice,
                    secondaryVoice ?? this.SecondaryVoice);

        }

        public void Serialize(Stream stream)
        {
            using (var sw = new StreamWriter(stream, Utf8NoBom, 1024, true))
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                stream.SetLength(0);
                sw.Write(json);
            }
        }

        public static Config Deserialize(Stream stream)
        {
            using (var sr = new StreamReader(stream, Utf8NoBom, false, 1024, true))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public Config(bool autoMode, Dictionary<KeyInfo, CommandType> hotkeys, string primaryVoice, string secondaryVoice)
            : this(autoMode, hotkeys.Select(kvp => new KeyInfoAndCommand(kvp.Key, kvp.Value)).ToList(), primaryVoice, secondaryVoice)
        {
            this.hotkeys = hotkeys;
        }

        [JsonConstructor]
        public Config(bool autoMode, List<KeyInfoAndCommand> hotkeySetup, string primaryVoice, string secondaryVoice)
        {
            this.autoMode = autoMode;
            this.hotkeySetup = hotkeySetup.ToList();
            this.hotkeys = hotkeySetup.ToDictionary(item => item.KeyInfo, item => item.Command);
            this.primaryVoice = primaryVoice;
            this.secondaryVoice = secondaryVoice;
        }

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly Dictionary<KeyInfo, CommandType> hotkeys;

        [JsonProperty]
        private bool autoMode;

        [JsonProperty]
        private List<KeyInfoAndCommand> hotkeySetup = new List<KeyInfoAndCommand>();

        [JsonProperty]
        private string primaryVoice;

        [JsonProperty]
        private string secondaryVoice;
    }
}