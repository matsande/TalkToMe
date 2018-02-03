using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TalkToMe.Core
{
    /*
    TODO:
    - Ability to update/persist config
    - Unittest for KeyMonitor
    - DONE - Basic impl of speech
------------------------------ Above should be done before initial push
    */
    public class SpeechManager : IDisposable
    {
        private readonly IClipboardTextMonitor clipboardMonitor;
        private readonly ISpeech speech;
        private readonly IKeyMonitor keyMonitor;
        private readonly Dictionary<CommandType, Action> commandMap;
        private readonly IDisposable clipboardSubscription;
        private readonly IDisposable keySubscription;

        private Config config;
        private string lastText;

        public SpeechManager(IClipboardTextMonitor clipboardMonitor, ISpeech speech, IKeyMonitor keyMonitor, Config config)
        {
            this.clipboardMonitor = clipboardMonitor;
            this.speech = speech;
            this.keyMonitor = keyMonitor;
            this.config = config;

            this.commandMap = this.InitializeCommands(config);

            this.clipboardSubscription = this.clipboardMonitor.ClipboardTextObservable.Subscribe(this.OnTextChanged);
            this.keySubscription = this.keyMonitor.KeysObservable.Subscribe(this.OnKeyPressed);
        }

        private Dictionary<CommandType, Action> InitializeCommands(Config config)
        {
            var commandMap = new Dictionary<CommandType, Action>();
            foreach (var hotkey in config.Hotkeys.Where(kvp => kvp.Key.Key != Keys.None))
            {
                var command = hotkey.Value;
                switch (command)
                {
                    case CommandType.Speak:
                        commandMap.Add(command, this.SpeakLastText);
                        break;
                    case CommandType.SwapLanguage:
                        commandMap.Add(command, this.SwapLanguage);
                        break;
                    case CommandType.Mute:
                        commandMap.Add(command, this.ToggleMute);
                        break;
                    case CommandType.ToggleAutoMode:
                        commandMap.Add(command, this.ToggleAutoMode);
                        break;
                    default:
                        // TODO: Trace unhandled command
                        break;
                }
            }
            return commandMap;
        }

        private void ToggleAutoMode()
        {
            // TODO: Persistence.
            this.config = this.config.With(autoMode: !this.config.AutoMode);
        }

        private void ToggleMute()
        {
            this.muted = !this.muted;
            if (this.muted)
            {
                this.speech.Abort();
            }
        }

        private void SwapLanguage()
        {
            // More research is needed here.
        }

        private void SpeakLastText()
        {
            if (!string.IsNullOrEmpty(this.lastText))
            {
                this.speech.Speak(this.lastText);
            }
        }

        private void OnTextChanged(string text)
        {
            this.lastText = text;
            if (this.config.AutoMode)
            {
                this.SpeakLastText();
            }
        }

        private void OnKeyPressed(KeyInfo key)
        {
            if (this.config.Hotkeys.TryGetValue(key, out var command) &&
                this.commandMap.TryGetValue(command, out var executor))
            {
                executor();
            }
            else
            {
                // TODO: Trace
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private bool muted;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.clipboardSubscription.Dispose();
                    this.keySubscription.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SpeechManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Config 
    {
        public bool AutoMode => this.autoMode;
        public Dictionary<KeyInfo, CommandType> Hotkeys => this.hotkeys;
        public string SelectedLanguage => this.selectedLanguage;

        public Config With(bool? autoMode = null,
            Dictionary<KeyInfo, CommandType> hotKeys = null,
            string selectedLanguage = null)
        {
            var sameInstance =
                this.AutoMode == autoMode &&
                this.Hotkeys == hotKeys &&
                this.SelectedLanguage == selectedLanguage;

            return sameInstance
                ? this
                : new Config(
                    autoMode ?? this.AutoMode, 
                    hotKeys ?? this.Hotkeys, 
                    selectedLanguage ?? this.SelectedLanguage);

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

        public static Config Deserialize(MemoryStream ms)
        {
            using (var sr = new StreamReader(ms, Utf8NoBom, false, 1024, true))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public Config(bool autoMode, Dictionary<KeyInfo, CommandType> hotkeys, string selectedLanguage)
            : this(autoMode, hotkeys.Select(kvp => new KeyInfoAndCommand(kvp.Key, kvp.Value)).ToList(), selectedLanguage)
        {
            this.hotkeys = hotkeys;
        }

        [JsonConstructor]
        public Config(bool autoMode, List<KeyInfoAndCommand> hotkeySetup, string selectedLanguage)
        {
            this.autoMode = autoMode;
            this.hotkeySetup = hotkeySetup.ToList();
            this.hotkeys = hotkeySetup.ToDictionary(item => item.KeyInfo, item => item.Command);
            this.selectedLanguage = selectedLanguage;
        }

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly Dictionary<KeyInfo, CommandType> hotkeys;

        [JsonProperty]
        private bool autoMode;

        [JsonProperty]
        private List<KeyInfoAndCommand> hotkeySetup = new List<KeyInfoAndCommand>();

        [JsonProperty]
        private string selectedLanguage;
    }

    public enum CommandType
    {
        None,
        ToggleAutoMode,
        Speak,
        Mute,
        SwapLanguage
    }
}
