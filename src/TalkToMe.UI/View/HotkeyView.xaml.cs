﻿using System.Windows.Controls;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI.View
{
    /// <summary>
    /// Interaction logic for HotkeyView.xaml
    /// </summary>
    public partial class HotkeyView : UserControl
    {
        public HotkeyView()
        {
            InitializeComponent();
            this.DataContext = ViewModelResolver.Resolve<HotkeyViewModel>();
        }
    }
}