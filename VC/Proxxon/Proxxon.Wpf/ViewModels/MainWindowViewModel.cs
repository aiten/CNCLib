using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using System.Windows;


namespace Proxxon.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties

		private Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

        public bool Connected
        {
            //get { return true; }
            get { return Com.IsConnected; }
        }

        private string _comport;
        public string ComPort
        {
            get { return string.IsNullOrEmpty(_comport) ? "com8" : _comport; }
            set { SetProperty(ref _comport,value); }
        }

		private bool _resetOnConnect;
		public bool ResetOnConnect
		{
			get { return _resetOnConnect; }
            set { SetProperty(ref _resetOnConnect,value); }
		}

        #endregion

        #region Operations

        public void Connect()
        {
			try
			{
				Com.ResetOnConnect = ResetOnConnect;
				Com.Connect(ComPort);
			}
			catch(Exception e)
			{
				MessageBox.Show("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			OnPropertyChanged(() => Connected);
		}
		public bool CanConnect()
        {
            return !Connected;
        }

		public void DisConnect()
		{
			Com.Disconnect();
			OnPropertyChanged(() => Connected);
		}
		public bool CanDisConnect()
		{
			return Connected;
		}

        #endregion

        #region Commands

		public ICommand ConnectCommand { get { return new DelegateCommand(Connect, CanConnect); } }
		public ICommand DisConnectCommand	{ get { return new DelegateCommand(DisConnect, CanDisConnect); } }

        #endregion
    }
}
