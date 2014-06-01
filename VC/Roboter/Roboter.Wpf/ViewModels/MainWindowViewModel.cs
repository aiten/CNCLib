using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;


namespace Roboter.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties

        private Roboter.Logic.Communication Com
        {
            get { return Framework.Tools.Singleton<Roboter.Logic.Communication>.Instance; }
        }

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

        private string _comport;
        public string ComPort
        {
            get { return string.IsNullOrEmpty(_comport) ? "com8" : _comport; }
            set
            {
                if (_comport != value)
                {
                    _comport = value;
                    RaisePropertyChanged(() => ComPort);
                }
            }
        }

        #endregion

        #region Operations

        public void Connect()
        {
            Com.Connect(ComPort);
            RaisePropertyChanged(() => Connected);
        }
        public bool CanConnect()
        {
            return !Connected;
        }

        #endregion

        #region Commands

        public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }

        #endregion
    }
}
