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

using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Wpf.Helpers;
using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
    public class CustomViewModel : DetailViewModel
	{
		public CustomViewModel(IManualControlViewModel vm)
			: base(vm)
		{
			Global.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { if (e.PropertyName == "Machine") MachineChanged(); }; 
		}

        #region Properties

        public string Desc00 { get { return GetDesc(0,0); } }
        public string Desc01 { get { return GetDesc(0,1); } }
        public string Desc02 { get { return GetDesc(0,2); } }
        public string Desc03 { get { return GetDesc(0,3); } }
        public string Desc04 { get { return GetDesc(0,4); } }
        public string Desc10 { get { return GetDesc(1,0); } }
        public string Desc11 { get { return GetDesc(1,1); } }
        public string Desc12 { get { return GetDesc(1,2); } }
        public string Desc13 { get { return GetDesc(1,3); } }
        public string Desc14 { get { return GetDesc(1,4); } }
        public string Desc20 { get { return GetDesc(2, 0); } }
        public string Desc21 { get { return GetDesc(2, 1); } }
        public string Desc22 { get { return GetDesc(2, 2); } }
        public string Desc23 { get { return GetDesc(2, 3); } }
        public string Desc24 { get { return GetDesc(2, 4); } }
        public string Desc30 { get { return GetDesc(3, 0); } }
        public string Desc31 { get { return GetDesc(3, 1); } }
        public string Desc32 { get { return GetDesc(3, 2); } }
        public string Desc33 { get { return GetDesc(3, 3); } }
        public string Desc34 { get { return GetDesc(3, 4); } }
        public string Desc40 { get { return GetDesc(4, 0); } }
        public string Desc41 { get { return GetDesc(4, 1); } }
        public string Desc42 { get { return GetDesc(4, 2); } }
        public string Desc43 { get { return GetDesc(4, 3); } }
        public string Desc44 { get { return GetDesc(4, 4); } }
        public string Desc50 { get { return GetDesc(5, 0); } }
        public string Desc51 { get { return GetDesc(5, 1); } }
        public string Desc52 { get { return GetDesc(5, 2); } }
        public string Desc53 { get { return GetDesc(5, 3); } }
        public string Desc54 { get { return GetDesc(5, 4); } }
        public string Desc60 { get { return GetDesc(6, 0); } }
        public string Desc61 { get { return GetDesc(6, 1); } }
        public string Desc62 { get { return GetDesc(6, 2); } }
        public string Desc63 { get { return GetDesc(6, 3); } }
        public string Desc64 { get { return GetDesc(6, 4); } }

		#endregion

		private void MachineChanged()
		{
			RaisePropertyChanged(nameof(Desc00));
			RaisePropertyChanged(nameof(Desc01));
			RaisePropertyChanged(nameof(Desc02));
			RaisePropertyChanged(nameof(Desc03));
			RaisePropertyChanged(nameof(Desc04));
 
			RaisePropertyChanged(nameof(Desc10));
			RaisePropertyChanged(nameof(Desc11));
			RaisePropertyChanged(nameof(Desc12));
			RaisePropertyChanged(nameof(Desc13));
			RaisePropertyChanged(nameof(Desc14));

			RaisePropertyChanged(nameof(Desc20));
			RaisePropertyChanged(nameof(Desc21));
			RaisePropertyChanged(nameof(Desc22));
			RaisePropertyChanged(nameof(Desc23));
			RaisePropertyChanged(nameof(Desc24));

			RaisePropertyChanged(nameof(Desc30));
			RaisePropertyChanged(nameof(Desc31));
			RaisePropertyChanged(nameof(Desc32));
			RaisePropertyChanged(nameof(Desc33));
			RaisePropertyChanged(nameof(Desc34));

			RaisePropertyChanged(nameof(Desc40));
			RaisePropertyChanged(nameof(Desc41));
			RaisePropertyChanged(nameof(Desc42));
			RaisePropertyChanged(nameof(Desc43));
			RaisePropertyChanged(nameof(Desc44));

			RaisePropertyChanged(nameof(Desc50));
			RaisePropertyChanged(nameof(Desc51));
			RaisePropertyChanged(nameof(Desc52));
			RaisePropertyChanged(nameof(Desc53));
			RaisePropertyChanged(nameof(Desc54));

			RaisePropertyChanged(nameof(Desc60));
			RaisePropertyChanged(nameof(Desc61));
			RaisePropertyChanged(nameof(Desc62));
			RaisePropertyChanged(nameof(Desc63));
			RaisePropertyChanged(nameof(Desc64));
		}

		private CNCLib.Logic.Contracts.DTO.MachineCommand GetCmd(int x, int y)
        {
			if (Global.Instance.Machine == null || Global.Instance.Machine.MachineCommands == null)
				return null;

            return Global.Instance.Machine.MachineCommands.FirstOrDefault(m => m.PosX == x && m.PosY == y);
        }

        private string GetDesc(int x, int y)
        {
            MachineCommand cmd = GetCmd(x,y);
            return cmd == null ? "" : cmd.CommandName;
        }

        #region Commands / CanCommands

        public bool CanSendXY(int x, int y)
        {
            var cmd = GetCmd(x, y);
            return cmd != null && string.IsNullOrEmpty(cmd.CommandString) == false && CanSend();
        }

        public void SendXY(int x, int y)
        {
			RunAndUpdate(async () => 
                {
                    var cmd = GetCmd(x, y);
                    if (cmd !=null)
                    {
						await new MachineGCodeHelper().SendCommandAsync(cmd.CommandString);
                    }
                });
        }
		
		#endregion

		#region ICommand
		public ICommand Send00Command => new DelegateCommand(() => SendXY(0,0), () => CanSendXY(0,0));
        public ICommand Send01Command => new DelegateCommand(() => SendXY(0, 1), () => CanSendXY(0, 1));
        public ICommand Send02Command => new DelegateCommand(() => SendXY(0, 2), () => CanSendXY(0, 2));
        public ICommand Send03Command => new DelegateCommand(() => SendXY(0, 3), () => CanSendXY(0, 3));
        public ICommand Send04Command => new DelegateCommand(() => SendXY(0, 4), () => CanSendXY(0, 4));
        public ICommand Send10Command => new DelegateCommand(() => SendXY(1, 0), () => CanSendXY(1, 0));
        public ICommand Send11Command => new DelegateCommand(() => SendXY(1, 1), () => CanSendXY(1, 1));
        public ICommand Send12Command => new DelegateCommand(() => SendXY(1, 2), () => CanSendXY(1, 2));
        public ICommand Send13Command => new DelegateCommand(() => SendXY(1, 3), () => CanSendXY(1, 3));
        public ICommand Send14Command => new DelegateCommand(() => SendXY(1, 4), () => CanSendXY(1, 4));
        public ICommand Send20Command => new DelegateCommand(() => SendXY(2, 0), () => CanSendXY(2, 0));
        public ICommand Send21Command => new DelegateCommand(() => SendXY(2, 1), () => CanSendXY(2, 1));
        public ICommand Send22Command => new DelegateCommand(() => SendXY(2, 2), () => CanSendXY(2, 2));
        public ICommand Send23Command => new DelegateCommand(() => SendXY(2, 3), () => CanSendXY(2, 3));
        public ICommand Send24Command => new DelegateCommand(() => SendXY(2, 4), () => CanSendXY(2, 4));
        public ICommand Send30Command => new DelegateCommand(() => SendXY(3, 0), () => CanSendXY(3, 0));
        public ICommand Send31Command => new DelegateCommand(() => SendXY(3, 1), () => CanSendXY(3, 1));
        public ICommand Send32Command => new DelegateCommand(() => SendXY(3, 2), () => CanSendXY(3, 2));
        public ICommand Send33Command => new DelegateCommand(() => SendXY(3, 3), () => CanSendXY(3, 3));
        public ICommand Send34Command => new DelegateCommand(() => SendXY(3, 4), () => CanSendXY(3, 4));
        public ICommand Send40Command => new DelegateCommand(() => SendXY(4, 0), () => CanSendXY(4, 0));
        public ICommand Send41Command => new DelegateCommand(() => SendXY(4, 1), () => CanSendXY(4, 1)); 
        public ICommand Send42Command => new DelegateCommand(() => SendXY(4, 2), () => CanSendXY(4, 2));
        public ICommand Send43Command => new DelegateCommand(() => SendXY(4, 3), () => CanSendXY(4, 3));
        public ICommand Send44Command => new DelegateCommand(() => SendXY(4, 4), () => CanSendXY(4, 4));
        public ICommand Send50Command => new DelegateCommand(() => SendXY(5, 0), () => CanSendXY(5, 0));
        public ICommand Send51Command => new DelegateCommand(() => SendXY(5, 1), () => CanSendXY(5, 1));
        public ICommand Send52Command => new DelegateCommand(() => SendXY(5, 2), () => CanSendXY(5, 2));
        public ICommand Send53Command => new DelegateCommand(() => SendXY(5, 3), () => CanSendXY(5, 3));
        public ICommand Send54Command => new DelegateCommand(() => SendXY(5, 4), () => CanSendXY(5, 4));
        public ICommand Send60Command => new DelegateCommand(() => SendXY(6, 0), () => CanSendXY(6, 0));
        public ICommand Send61Command => new DelegateCommand(() => SendXY(6, 1), () => CanSendXY(6, 1));
        public ICommand Send62Command => new DelegateCommand(() => SendXY(6, 2), () => CanSendXY(6, 2));
        public ICommand Send63Command => new DelegateCommand(() => SendXY(6, 3), () => CanSendXY(6, 3));
        public ICommand Send64Command => new DelegateCommand(() => SendXY(6, 4), () => CanSendXY(6, 4));  

        #endregion
    }
}
