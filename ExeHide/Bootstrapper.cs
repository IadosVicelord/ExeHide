using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ExeHide.ViewModels;
namespace ExeHide
{
    class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}