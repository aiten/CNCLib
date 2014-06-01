using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Roboter.Wpf.ViewModel
{
    public class RoboterModel : Framework.Wpf.BaseView
    {
        #region Properties

        private Roboter.Logic.Communication Com
        {
            get { return Framework.Tools.Singleton<Roboter.Logic.Communication>.Instance; }
        }

        private string _comport;
        public string ComPort
        {
            get { return string.IsNullOrEmpty(_comport) ? "com8" : _comport; }
            set
            {
                _comport = value;
                this.OnPropertyChanged("ComPort");
            }
        }

        private UInt16 _maxSpeed = 15000;
        public UInt16 MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                _maxSpeed = value;
                this.OnPropertyChanged("MaxSpeed");
            }
        }

        private string _directCommand;
        public string DirectCommand
        {
            get { return _directCommand; }
            set
            {
                _directCommand = value;
                this.OnPropertyChanged("DirectCommand");
            }
        }


        public bool Connected
        {
            get { return Com.IsConnected; }
        }

        #endregion

        #region Operations

        public void Connect()
        {
            Com.Connect(ComPort);
            this.OnPropertyChanged("Connected");
        }
        public bool CanConnect()
        {
            return !Connected;
        }

        public void SetMaxSpeed()
        {
            Com.SendCommand("s " + MaxSpeed);
            this.OnPropertyChanged("SetMaxSpeed");
        }

        public void SendDirectCommand()
        {
            Com.SendCommand(DirectCommand);
            this.OnPropertyChanged("DirectCommand");
        }


        public bool CanSendCommand()
        {
            return Connected;
        }


        #endregion

        #region Commands

        private ICommand _cmdConnect;
        public ICommand CommandConnect
        {
            get
            {
                return Framework.Wpf.RelayCommand.CreateCommand(
                    ref this._cmdConnect,
                    p => {  this.Connect();  },
                    c => this.CanConnect() );
            }
        }

        private ICommand _cmdSetMaxSpeed;
        public ICommand CommandSetMaxSpeed
        {
            get
            {
                return Framework.Wpf.RelayCommand.CreateCommand(
                    ref this._cmdSetMaxSpeed,
                    p => { this.SetMaxSpeed(); },
                    c => this.CanSendCommand());
            }
        }

        private ICommand _cmdDirectCommand;
        public ICommand CommandDirectCommand
        {
            get
            {
                return Framework.Wpf.RelayCommand.CreateCommand(
                    ref this._cmdDirectCommand,
                    p => { this.SendDirectCommand(); },
                    c => this.CanSendCommand());
            }
        }


        #endregion
    }
}
