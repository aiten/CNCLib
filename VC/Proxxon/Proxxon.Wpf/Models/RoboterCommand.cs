using System;
using System.Windows.Media;
using Proxxon.Wpf.Helpers;
using Framework.Wpf.Helpers;

namespace Proxxon.Wpf.Models
{
    public class ProxxonCommand : NotificationObject
    {
        #region Ctor
        public ProxxonCommand()
        {
         }
        #endregion

        #region CommandText

        private string _commandText;
        public string CommandText
        {
            get { return _commandText; }
            set { SetProperty(ref _commandText, value);  }
        }

        #endregion

        #region CommandDate

        private DateTime _commandDate;
        public DateTime CommandDate
        {
            get { return _commandDate; }
            set { SetProperty(ref _commandDate, value);  }
        }

        #endregion

        #region Result

        private string _result;
        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value);  }
        }

        #endregion

    }
}
