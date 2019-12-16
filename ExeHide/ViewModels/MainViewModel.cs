using System;
using System.Linq;
using Caliburn.Micro;
using ExeHide.Models;

namespace ExeHide.ViewModels
{
    class MainViewModel : Screen
    {
        private string _password = "";

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(Password);
                NotifyOfPropertyChange(() => CanCreatePassword);
            }
        }

        public bool CanCreatePassword => Password.Contains(' ') || String.IsNullOrEmpty(Password) ? false : true;

        public void CreatePassword()
        {
            GeneralModel.SetPassword(Password);
        }

        public void RemovePassword()
        {
            GeneralModel.UnsetPassword();
        }
    }
}
