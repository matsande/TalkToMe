using System.Windows.Controls;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI.View
{
    /// <summary>
    /// Interaction logic for SpeechManagerViewModel.xaml
    /// </summary>
    public partial class SpeechManagerView : UserControl
    {
        public SpeechManagerView()
        {
            InitializeComponent();
        }

        public SpeechManagerView(SpeechManagerViewModel speechManagerViewModel)
        {
            this.InitializeComponent();
            this.DataContext = speechManagerViewModel;
        }
    }
}