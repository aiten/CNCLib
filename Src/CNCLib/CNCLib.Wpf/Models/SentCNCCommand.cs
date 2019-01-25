/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.Models
{
    public class SentCNCCommand : NotificationObject
    {
        #region CommandText

        private string _commandText;

        public string CommandText
        {
            get => _commandText;
            set => SetProperty(ref _commandText, value);
        }

        #endregion

        #region CommandDate

        private DateTime _commandDate;

        public DateTime CommandDate
        {
            get => _commandDate;
            set => SetProperty(ref _commandDate, value);
        }

        #endregion

        #region Result

        private string _result;

        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        #endregion
    }
}