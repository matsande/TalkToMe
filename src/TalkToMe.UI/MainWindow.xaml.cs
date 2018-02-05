using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TalkToMe.Core;
using TalkToMe.UI.View;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.BootStrap();
        }

        private void BootStrap()
        {
            var configPersistence = new LocalConfigPersistence();
            if (!configPersistence.TryLoad(out var config))
            {
                config = new Config(
                    true, 
                    new Dictionary<KeyInfo, CommandType>
                    {
                        { new KeyInfo(Keys.A, Keys.LWin), CommandType.ToggleAutoMode },
                        { new KeyInfo(Keys.T, Keys.LWin), CommandType.Speak },
                        { new KeyInfo(Keys.M, Keys.LWin), CommandType.ToggleMute }
                    },
                    string.Empty,
                    string.Empty);
            }

            var hook = new HookKeyMonitor(config.Hotkeys.Keys, new StaticHookProvider());
            // TODO: Remove
            hook.KeysObservable.Subscribe(k =>
            {
                System.Diagnostics.Debug.Print($"Got key: {k}");
            });

            var clipmon = new ClipboardTextMonitor(this);
            // TODO: Remove
            clipmon.ClipboardTextObservable.Subscribe(text =>
            {
                System.Diagnostics.Debug.Print($"Got clipboardtext: {text}");
            });

            var speechManager = new SpeechManager(clipmon, new SpeechSynth(), hook, new LocalConfigPersistence(), config);

            this.mainView.DataContext = new SpeechManagerViewModel(speechManager);
        }
    }
}
