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
            var config = new Config(true, new Dictionary<KeyInfo, CommandType>
            {
                { new KeyInfo(Keys.A, Keys.LWin), CommandType.ToggleAutoMode },
                { new KeyInfo(Keys.T, Keys.LWin), CommandType.Speak }
            },
            "Nope");

            var hook = new HookKeyMonitor(config.Hotkeys.Keys);
            hook.KeysObservable.Subscribe(k =>
            {
                System.Diagnostics.Debug.Print($"Got key: {k}");
            });

            var clipmon = new ClipboardTextMonitor(this);
            clipmon.ClipboardTextObservable.Subscribe(text =>
            {
                System.Diagnostics.Debug.Print($"Got clipboardtext: {text}");
            });

            var speechManager = new SpeechManager(clipmon, new SpeechSynth(), hook, new LocalConfigPersistence(), config);
        }
    }
}
