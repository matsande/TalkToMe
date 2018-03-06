using System;
using System.Windows;
using System.Windows.Controls;
using TalkToMe.Core.Hook;

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
                return (KeyInfo)GetValue(SelectedHotkeyProperty);
            }
            set
            {
                SetValue(SelectedHotkeyProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedHotkeyProperty =
            DependencyProperty.Register("SelectedHotkey", typeof(KeyInfo), typeof(HotKeySelection), new FrameworkPropertyMetadata(null, OnSelectedHotkeyChanged));

        private static void OnSelectedHotkeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (d is HotKeySelection self)
            {
                self.keyTextBox.Text = FormatKeyText((KeyInfo)eventArgs.NewValue);
            }
        }

        public static readonly DependencyProperty KeyMonitorProperty =
            DependencyProperty.Register("KeyMonitor", typeof(IKeyMonitor), typeof(HotKeySelection), new FrameworkPropertyMetadata(null));

        private static string FormatKeyText(KeyInfo keyInfo)
        {
            if (keyInfo == KeyInfo.Empty)
            {
                return "<no key assigned>";
            }
            else
            {
                return keyInfo.Modifier == System.Windows.Forms.Keys.None
                    ? keyInfo.Key.ToString()
                    : $"{keyInfo.Modifier} + {keyInfo.Key}";
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.keyOverride?.Dispose();
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

        private IDisposable keyOverride;
    }
}