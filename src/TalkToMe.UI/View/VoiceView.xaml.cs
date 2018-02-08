using System.Windows.Controls;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI.View
{
    /// <summary>
    /// Interaction logic for VoiceView.xaml
    /// </summary>
    public partial class VoiceView : UserControl
    {
        public VoiceView()
        {
            InitializeComponent();
            this.DataContext = ViewModelResolver.Resolve<VoiceViewModel>();
        }
    }
}