using System;
using System.Windows.Controls;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI.View
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : UserControl
    {
        [Obsolete("Only for designer")]
        public GeneralView()
        {
            InitializeComponent();
        }

        public GeneralView(GeneralViewModel generalViewModel)
        {
            InitializeComponent();
            this.DataContext = generalViewModel;
        }
    }
}