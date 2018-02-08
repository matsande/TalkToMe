using System;
using System.Windows;
using TalkToMe.Core;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(SpeechManagerViewModel viewModel, IClipboardTextMonitor clipboardTextMonitor)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.clipboardTextMonitor = clipboardTextMonitor;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            (this.clipboardTextMonitor as ClipboardTextMonitor)?.InstallClipboardHook(this);
        }

        private readonly IClipboardTextMonitor clipboardTextMonitor;
    }
}