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
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;
using CNCLib.ServiceProxy;
using System.Threading.Tasks;

namespace CNCLib.Wpf.ViewModels
{
    public class EepromViewModel : BaseViewModel, IDisposable
	{
		#region crt

		public EepromViewModel()
		{
		}

		#endregion

		#region dispose

		public void Dispose()
		{
		}

		#endregion

		#region Properties

		private Eeprom _eeprom = new Eeprom();
		public Eeprom EepromValue
		{
			get { return _eeprom; }
			set { SetProperty(() => _eeprom == value, () => _eeprom = value); }
		}

		#endregion

		#region Operations


		public void WriteEeprom()
		{
            CloseAction();
        }
		public void ReadEeprom()
		{
			CloseAction();
		}

		public bool CanReadEeprom()
		{
			return true;
		}

		public bool CanWriteEeprom()
		{
			return true;
		}

		#endregion

		#region Commands

		public ICommand ReadEepromCommand => new DelegateCommand(ReadEeprom, CanReadEeprom);
        public ICommand WriteEepromCommand => new DelegateCommand(WriteEeprom, CanWriteEeprom);

        #endregion
    }
}
