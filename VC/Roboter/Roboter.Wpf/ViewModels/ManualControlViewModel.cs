using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using Roboter.Wpf.Models;
using Roboter.Logic;

namespace Roboter.Wpf.ViewModels
{
    public class ManualControlViewModel : BaseViewModel
    {
        #region Properties

        public Roboter.Logic.Communication Com
        {
            get { return Framework.Tools.Singleton<Roboter.Logic.Communication>.Instance; }
        }

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

        #region Max Speed

        private UInt16 _maxSpeed = 15000;
        public UInt16 MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                if (_maxSpeed != value)
                {
                    _maxSpeed = value;
                    RaisePropertyChanged(() => MaxSpeed);
                }
            }
        }

        #endregion

        #region DirectCommand

        private string _directCommand;
        public string DirectCommand
        {
            get { return _directCommand; }
            set
            {
                if (_directCommand != value)
                {
                    _directCommand = value;
                    RaisePropertyChanged(() => DirectCommand);
                }
            }
        }

        #endregion

        #region RoboterCommandCollection

        private ObservableCollection<RoboterCommand> _roboterCommandCollection;
        public ObservableCollection<RoboterCommand> RoboterCommandCollection
        {
            get { return _roboterCommandCollection; }
            set
            {
                if (_roboterCommandCollection != value)
                {
                    _roboterCommandCollection = value;
                    RaisePropertyChanged(() => RoboterCommandCollection);
                }
            }
        }

        #endregion
        #endregion

        #region Operations

        public void SetMaxSpeed()
        {
            Com.SendCommand("s " + MaxSpeed);
            RefreshAfterCommand();
        }
        public void SendInfo()
        {
            SendRoboterCommand("?");
        }
        public void SendAbort()
        {
            SendRoboterCommand("!");
        }

        public void SendDirect()
        {
            SendRoboterCommand(DirectCommand);
        }

        public void SendRoboterCommand(string command)
        {
            Com.SendCommand(command);
            RefreshAfterCommand();
        }
 
        public void RefreshAfterCommand()
        {
            RefreshCommandHistory();
        }

        public void RefreshCommandHistory()
        {
            var results = new ObservableCollection<RoboterCommand>();

            foreach (Communication.SentCommands rc in Com.CommandHistory)
            {
                results.Add(new RoboterCommand() { CommandDate = rc.SentTime, CommandText = rc.Command, Result = rc.Result });

            }
            RoboterCommandCollection = results;
        }
        public void ClearCommandHistory()
        {
            Com.ClearCommandHistory();
            RefreshCommandHistory();
        }

        public bool CanSendCommand()
        {
            return Connected;
        }


        #endregion

        #region Commands

        public ICommand SetMaxSpeedCommand { get { return new DelegateCommand(SetMaxSpeed, CanSendCommand); } }
        public ICommand SendDirectCommand { get { return new DelegateCommand(SendDirect, CanSendCommand); } }
        public ICommand RefreshHistoryCommand { get { return new DelegateCommand(RefreshCommandHistory, CanSendCommand); } }
        public ICommand ClearHistoryCommand { get { return new DelegateCommand(ClearCommandHistory, CanSendCommand); } }
        public ICommand SendInfoCommand { get { return new DelegateCommand(SendInfo, CanSendCommand); } }
        public ICommand SendAbortCommand { get { return new DelegateCommand(SendAbort, CanSendCommand); } }

        #endregion

    }
}
