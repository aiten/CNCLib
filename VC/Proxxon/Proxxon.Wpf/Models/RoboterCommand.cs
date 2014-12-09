////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

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
            set { SetProperty(_commandText, value);  }
        }

        #endregion

        #region CommandDate

        private DateTime _commandDate;
        public DateTime CommandDate
        {
            get { return _commandDate; }
            set { SetProperty(_commandDate, value);  }
        }

        #endregion

        #region Result

        private string _result;
        public string Result
        {
            get { return _result; }
            set { SetProperty(_result, value);  }
        }

        #endregion

    }
}
