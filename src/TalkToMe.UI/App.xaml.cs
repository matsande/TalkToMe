using System;
using System.Windows;
using SimpleInjector;

namespace TalkToMe.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal Container Container => this.container;

        internal void SetContainer(Container container)
        {
            if (this.container != null)
            {
                throw new InvalidOperationException("Attempt to reassign container");
            }

            this.container = container;
        }

        private Container container = null;
    }
}