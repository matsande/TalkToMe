namespace TalkToMe.UI.View
{

    internal static class ViewModelResolver
    {
        internal static TViewModel Resolve<TViewModel>()
            where TViewModel : class
        {
            return (App.Current as App)?.Container.GetInstance<TViewModel>();
        }
    }
}