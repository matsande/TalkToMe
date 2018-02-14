namespace TalkToMe.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Forms;
    using TalkToMe.Core.Hook;
    using TalkToMe.Core.Voice;

    /*
    TODO:
    - DONE - Ability to update/persist config
        - Includes having a strategy for how to handle UI updates.
            - DONE - albeit crude atm.

    - DONE - Unittest for KeyMonitor
    - DONE - Basic impl of speech
------------------------------ Above should be done before initial push
    - UI
    - Language selection
    - KeyConfig
    - Trace
    - Version

    */

    /// <summary>
    /// Defines the <see cref="SpeechManager" />
    /// </summary>
    public class SpeechManager : IDisposable, ISpeechManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechManager"/> class.
        /// </summary>
        /// <param name="clipboardMonitor">The <see cref="IClipboardTextMonitor"/></param>
        /// <param name="speech">The <see cref="ISpeech"/></param>
        /// <param name="keyMonitor">The <see cref="IKeyMonitor"/></param>
        /// <param name="configPersistence">The <see cref="IConfigPersistence"/></param>
        /// <param name="config">The <see cref="Config"/></param>
        public SpeechManager(
            IClipboardTextMonitor clipboardMonitor,
            IVoiceFactory voiceFactory,
            IKeyMonitor keyMonitor,
            IConfigPersistence configPersistence,
            Config config)
        {
            this.clipboardMonitor = clipboardMonitor;
            this.voiceFactory = voiceFactory;
            this.keyMonitor = keyMonitor;
            this.configPersistence = configPersistence;
            this.config = config;

            this.InitializeVoices();
            this.commandMap = this.InitializeCommands(config);

            this.clipboardSubscription = this.clipboardMonitor.ClipboardTextObservable.Subscribe(this.OnTextChanged);
            this.keySubscription = this.keyMonitor.KeysObservable.Subscribe(this.OnKeyPressed);
            this.stateChangeSubject = new Subject<SpeechManagerStateChange>();
        }

        public IObservable<SpeechManagerStateChange> StateChangeObservable => this.stateChangeSubject.AsObservable();
        public Config Config => this.config;

        // TODO: Rework this a bit, should not be able to return null
        public VoiceDescriptor CurrentVoice => this.currentVoice?.Descriptor;

        public void UpdateConfig(Config config)
        {
            this.config = config;
            this.configPersistence.Save(this.config);

            // Note: This could be optimized a bit, for now though, always update.
            this.keyMonitor.UpdateObservedKeys(config.Hotkeys.Select(x => x.Key));
            this.InitializeVoices();

            this.stateChangeSubject.OnNext(new SpeechManagerStateChange());
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public IReadOnlyCollection<VoiceDescriptor> AvailableVoices => this.voiceFactory.AvailableVoices;

        /// <summary>
        /// The Dispose
        /// </summary>
        /// <param name="disposing">The <see cref="bool"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.clipboardSubscription.Dispose();
                    this.keySubscription.Dispose();
                    this.stateChangeSubject.Dispose();
                }

                disposedValue = true;
            }
        }

        private void InitializeVoices()
        {
            if (!this.voiceFactory.TryCreate(this.config.PrimaryVoice, out this.primaryVoice))
            {
                this.primaryVoice = this.voiceFactory.CreateDefault();
            }

            if (!this.voiceFactory.TryCreate(this.config.SecondaryVoice, out this.secondaryVoice))
            {
                this.secondaryVoice = this.voiceFactory.CreateDefault();
            }

            // TODO: Should we persist the last selected voice?
            this.currentVoice = this.primaryVoice;
        }

        /// <summary>
        /// The InitializeCommands
        /// </summary>
        /// <param name="config">The <see cref="Config"/></param>
        /// <returns>The <see cref="Dictionary{CommandType, Action}"/></returns>
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

                    case CommandType.ToggleMute:
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

        /// <summary>
        /// The ToggleAutoMode
        /// </summary>
        private void ToggleAutoMode()
        {
            this.UpdateConfig(this.config.With(autoMode: !this.config.AutoMode));
        }

        /// <summary>
        /// The ToggleMute
        /// </summary>
        private void ToggleMute()
        {
            this.UpdateConfig(this.config.With(mute: !this.config.Mute));
            if (this.config.Mute)
            {
                this.currentVoice.Abort();
            }
        }

        /// <summary>
        /// The SwapLanguage
        /// </summary>
        private void SwapLanguage()
        {
            if (this.currentVoice.Descriptor == this.primaryVoice.Descriptor)
            {
                this.currentVoice = this.secondaryVoice;
            }
            else
            {
                this.currentVoice = this.primaryVoice;
            }

            this.stateChangeSubject.OnNext(new SpeechManagerStateChange());
        }

        /// <summary>
        /// The SpeakLastText
        /// </summary>
        private void SpeakLastText()
        {
            if (!this.config.Mute && !string.IsNullOrEmpty(this.lastText))
            {
                this.currentVoice.Speak(this.lastText);
            }
        }

        /// <summary>
        /// The OnTextChanged
        /// </summary>
        /// <param name="text">The <see cref="string"/></param>
        private void OnTextChanged(string text)
        {
            var previousText = this.lastText;
            this.lastText = text;
            if (this.config.AutoMode && previousText != text)
            {
                this.SpeakLastText();
            }
        }

        /// <summary>
        /// The OnKeyPressed
        /// </summary>
        /// <param name="key">The <see cref="KeyInfo"/></param>
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

        private readonly IClipboardTextMonitor clipboardMonitor;
        private readonly IVoiceFactory voiceFactory;
        private readonly IKeyMonitor keyMonitor;
        private readonly IConfigPersistence configPersistence;
        private readonly Dictionary<CommandType, Action> commandMap;
        private readonly IDisposable clipboardSubscription;
        private readonly IDisposable keySubscription;
        private readonly Subject<SpeechManagerStateChange> stateChangeSubject;
        private IVoice primaryVoice;
        private IVoice secondaryVoice;
        private Config config;

        private string lastText;

        private IVoice currentVoice;

        private bool disposedValue = false;// To detect redundant calls
    }

    public class SpeechManagerStateChange
    {
        // Note: Placeholder
    }
}