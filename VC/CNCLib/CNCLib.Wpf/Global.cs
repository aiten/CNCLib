////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Framework.Tools.Pattern;

namespace CNCLib.Wpf
{
	public class Global : Singleton<Global>, INotifyPropertyChanged
	{
		private CNCLib.Logic.Contracts.DTO.Machine _machine;
		public CNCLib.Logic.Contracts.DTO.Machine Machine { get { return _machine; } set { _machine = value; OnPropertyChanged(); } }

		public event PropertyChangedEventHandler PropertyChanged;


		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
