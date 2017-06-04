////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

using System.Threading.Tasks;
using System.Windows.Input;
using CNCLib.GCode.GUI.Models;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.ViewModels
{
    public class LoadOptionViewModel : BaseViewModel
    {
		#region crt

		public LoadOptionViewModel()
		{
		}

        #endregion

        #region private operations

        #endregion

        #region GUI-forward

        //		public Action<int> EditMachine { get; set; }

        #endregion

        #region Properties

        private LoadOptions _loadOptions = new LoadOptions();
        public LoadOptions LoadOptionsValue
        {
            get { return _loadOptions; }
            set { SetProperty(() => _loadOptions == value, () => _loadOptions = value); }
        }

        #endregion

        #region Operations

        public async Task Connect()
        {
		}


		public bool CanConnect()
        {
            return true;
            //return !Connected && Machine != null;
        }


		#endregion

		#region Commands

		public ICommand ConnectCommand => new DelegateCommand(async () => await Connect(), CanConnect);

		#endregion
	}
}
