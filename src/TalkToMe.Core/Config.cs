using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TalkToMe.Core.Hook;
using TalkToMe.Core.Voice;

namespace TalkToMe.Core
{

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Config 
    {
        public bool AutoMode => this.autoMode;
        public bool Mute => this.mute;
        public bool AbortOnEscape => this.abortOnEscape;
        public bool OverrideWithNewText => this.overrideWithNewText;
        public Dictionary<KeyInfo, CommandType> Hotkeys => this.hotkeys;
        public VoiceDescriptor PrimaryVoice => this.primaryVoice;
        public VoiceDescriptor SecondaryVoice => this.secondaryVoice;

        public Config With(bool? autoMode = null,
            bool? mute = null,
            bool? abortOnEscape = null,
            bool? overrideWithNewText = null,
            Dictionary<KeyInfo, CommandType> hotKeys = null,
            VoiceDescriptor primaryVoice = null,
            VoiceDescriptor secondaryVoice = null)
        {
            var sameInstance =
                (this.AutoMode == autoMode || autoMode == null) &&
                (this.Mute == mute || mute == null) &&
                (this.AbortOnEscape == abortOnEscape || abortOnEscape == null) &&
                (this.OverrideWithNewText == overrideWithNewText || overrideWithNewText == null) &&
                (this.Hotkeys == hotKeys || hotKeys == null) &&
                (this.PrimaryVoice == primaryVoice || primaryVoice == null) &&
                (this.SecondaryVoice == secondaryVoice || secondaryVoice == null);

            return sameInstance
                ? this
                : new Config(
                    autoMode ?? this.AutoMode, 
                    mute ?? this.Mute,
                    abortOnEscape ?? this.AbortOnEscape,
                    overrideWithNewText ?? this.OverrideWithNewText,
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

        public Config(bool autoMode, bool mute, bool abortOnEscape, bool overrideWithNewText, Dictionary<KeyInfo, CommandType> hotkeys, VoiceDescriptor primaryVoice, VoiceDescriptor secondaryVoice)
            : this(autoMode, mute, abortOnEscape, overrideWithNewText, hotkeys.Select(kvp => new KeyInfoAndCommand(kvp.Key, kvp.Value)).ToList(), primaryVoice, secondaryVoice)
        {
            this.hotkeys = hotkeys;
        }

        [JsonConstructor]
        public Config(
            bool autoMode, 
            bool mute, 
            bool abortOnEscape,
            bool overrideWithNewText,
            List<KeyInfoAndCommand> hotkeySetup, 
            VoiceDescriptor primaryVoice, 
            VoiceDescriptor secondaryVoice)
        {
            this.autoMode = autoMode;
            this.mute = mute;
            this.abortOnEscape = abortOnEscape;
            this.overrideWithNewText = overrideWithNewText;
            this.hotkeySetup = hotkeySetup.ToList();
            this.hotkeys = hotkeySetup.ToDictionary(item => item.KeyInfo, item => item.Command);
            this.primaryVoice = primaryVoice ?? throw new ArgumentNullException(nameof(primaryVoice));
            this.secondaryVoice = secondaryVoice ?? throw new ArgumentNullException(nameof(secondaryVoice));
        }

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly Dictionary<KeyInfo, CommandType> hotkeys;

        [JsonProperty]
        private bool autoMode;

        [JsonProperty]
        private bool mute;

        [JsonProperty]
        private bool abortOnEscape;

        [JsonProperty]
        private bool overrideWithNewText;

        [JsonProperty]
        private List<KeyInfoAndCommand> hotkeySetup = new List<KeyInfoAndCommand>();

        [JsonProperty]
        private VoiceDescriptor primaryVoice;

        [JsonProperty]
        private VoiceDescriptor secondaryVoice;
    }
}