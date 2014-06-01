using System;
using System.Windows.Media;
using Roboter.Wpf.Helpers;
using Framework.Wpf.Helpers;

namespace Roboter.Wpf.Models
{
    public class RoboterCommand : NotificationObject
    {
        #region Ctor
        public RoboterCommand()
        {
         }
        #endregion

        #region CommandText

        private string _commandText;
        public string CommandText
        {
            get { return _commandText; }
            set
            {
                if (_commandText != value)
                {
                    _commandText = value;
                    RaisePropertyChanged(() => CommandText);
                }
            }
        }

        #endregion

        #region CommandDate

        private DateTime _commandDate;
        public DateTime CommandDate
        {
            get { return _commandDate; }
            set
            {
                if (_commandDate != value)
                {
                    _commandDate = value;
                    RaisePropertyChanged(() => CommandDate);
                }
            }
        }

        #endregion

        #region Result

        private string _result;
        public string Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    RaisePropertyChanged(() => Result);
                }
            }
        }

        #endregion

    }
}
