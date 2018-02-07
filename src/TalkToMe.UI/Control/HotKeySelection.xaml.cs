using System;
using System.Windows;
using System.Windows.Controls;
using TalkToMe.Core;

namespace TalkToMe.UI.Control
{
    /// <summary>
    /// Interaction logic for HotKeySelection.xaml
    /// </summary>
    public partial class HotKeySelection : UserControl
    {
        public HotKeySelection()
        {
            InitializeComponent();
        }

        public IKeyMonitor KeyMonitor
        {
            get
            {
                return (IKeyMonitor)GetValue(KeyMonitorProperty);
            }
            set
            {
                SetValue(KeyMonitorProperty, value);
            }
        }

        public KeyInfo SelectedHotkey
        {
            get
            {
                return this.selectedHotkey;
            }
            set
            {
                // TODO: This means we should change modifiers to be a list, will be easier to manage really.
                this.keyTextBox.Text = FormatKeyText(value);
                this.selectedHotkey = value;
            }
        }

        public static readonly DependencyProperty KeyMonitorProperty =
                    DependencyProperty.Register(
                "KeyMonitor",
                typeof(IKeyMonitor),
                typeof(HotKeySelection),
                new FrameworkPropertyMetadata(null));

        private static string FormatKeyText(KeyInfo keyInfo)
        {
            return keyInfo.Modifier == System.Windows.Forms.Keys.None
                ? keyInfo.Key.ToString()
                : $"{keyInfo.Key} + {keyInfo.Modifier}";
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.keyOverride?.Dispose();
            //this.keysSubscription = this.KeyMonitor?.KeysObservable.Subscribe(this.OnKey);
            this.keyOverride = this.KeyMonitor?.Override(this.OnKey);
        }

        private bool OnKey(KeyInfo key)
        {
            bool handled;
            try
            {
                // TODO: Do we need to contextswitch here?
                this.SelectedHotkey = key;
                handled = true;
            }
            catch (Exception)
            {
                this.keyOverride.Dispose();
                handled = false;
            }

            return handled;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.keyOverride?.Dispose();
            this.keyOverride = null;
        }

        private KeyInfo selectedHotkey;
        private IDisposable keyOverride;
    }
}