using System.Windows.Controls;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI.View
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : UserControl
    {
        public GeneralView()
        {
            InitializeComponent();
            this.DataContext = ViewModelResolver.Resolve<GeneralViewModel>();
        }
    }
}