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

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
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

        public String Desc00 { get { return GetDesc(0,0); } }
        public String Desc01 { get { return GetDesc(0,1); } }
        public String Desc02 { get { return GetDesc(0,2); } }
        public String Desc03 { get { return GetDesc(0,3); } }
        public String Desc04 { get { return GetDesc(0,4); } }
        public String Desc10 { get { return GetDesc(1,0); } }
        public String Desc11 { get { return GetDesc(1,1); } }
        public String Desc12 { get { return GetDesc(1,2); } }
        public String Desc13 { get { return GetDesc(1,3); } }
        public String Desc14 { get { return GetDesc(1,4); } }
        public String Desc20 { get { return GetDesc(2, 0); } }
        public String Desc21 { get { return GetDesc(2, 1); } }
        public String Desc22 { get { return GetDesc(2, 2); } }
        public String Desc23 { get { return GetDesc(2, 3); } }
        public String Desc24 { get { return GetDesc(2, 4); } }
        public String Desc30 { get { return GetDesc(3, 0); } }
        public String Desc31 { get { return GetDesc(3, 1); } }
        public String Desc32 { get { return GetDesc(3, 2); } }
        public String Desc33 { get { return GetDesc(3, 3); } }
        public String Desc34 { get { return GetDesc(3, 4); } }
        public String Desc40 { get { return GetDesc(4, 0); } }
        public String Desc41 { get { return GetDesc(4, 1); } }
        public String Desc42 { get { return GetDesc(4, 2); } }
        public String Desc43 { get { return GetDesc(4, 3); } }
        public String Desc44 { get { return GetDesc(4, 4); } }
        public String Desc50 { get { return GetDesc(5, 0); } }
        public String Desc51 { get { return GetDesc(5, 1); } }
        public String Desc52 { get { return GetDesc(5, 2); } }
        public String Desc53 { get { return GetDesc(5, 3); } }
        public String Desc54 { get { return GetDesc(5, 4); } }
        public String Desc60 { get { return GetDesc(6, 0); } }
        public String Desc61 { get { return GetDesc(6, 1); } }
        public String Desc62 { get { return GetDesc(6, 2); } }
        public String Desc63 { get { return GetDesc(6, 3); } }
        public String Desc64 { get { return GetDesc(6, 4); } }

		#endregion

		private void MachineChanged()
		{
			OnPropertyChanged(() => Desc00);
			OnPropertyChanged(() => Desc01);
			OnPropertyChanged(() => Desc02);
			OnPropertyChanged(() => Desc03);
			OnPropertyChanged(() => Desc04);

			OnPropertyChanged(() => Desc10);
			OnPropertyChanged(() => Desc11);
			OnPropertyChanged(() => Desc12);
			OnPropertyChanged(() => Desc13);
			OnPropertyChanged(() => Desc14);

			OnPropertyChanged(() => Desc20);
			OnPropertyChanged(() => Desc21);
			OnPropertyChanged(() => Desc22);
			OnPropertyChanged(() => Desc23);
			OnPropertyChanged(() => Desc24);

			OnPropertyChanged(() => Desc30);
			OnPropertyChanged(() => Desc31);
			OnPropertyChanged(() => Desc32);
			OnPropertyChanged(() => Desc33);
			OnPropertyChanged(() => Desc34);

			OnPropertyChanged(() => Desc40);
			OnPropertyChanged(() => Desc41);
			OnPropertyChanged(() => Desc42);
			OnPropertyChanged(() => Desc43);
			OnPropertyChanged(() => Desc44);

			OnPropertyChanged(() => Desc50);
			OnPropertyChanged(() => Desc51);
			OnPropertyChanged(() => Desc52);
			OnPropertyChanged(() => Desc53);
			OnPropertyChanged(() => Desc54);

			OnPropertyChanged(() => Desc60);
			OnPropertyChanged(() => Desc61);
			OnPropertyChanged(() => Desc62);
			OnPropertyChanged(() => Desc63);
			OnPropertyChanged(() => Desc64);
		}

		private CNCLib.Logic.Contracts.DTO.MachineCommand GetCmd(int x, int y)
        {
			if (Global.Instance.Machine == null || Global.Instance.Machine.MachineCommands == null)
				return null;

            return Global.Instance.Machine.MachineCommands.Where(m => m.PosX == x && m.PosY == y).FirstOrDefault();
        }

        private string GetDesc(int x, int y)
        {
            var cmd = GetCmd(x,y);
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
            RunInNewTask(() => 
                {
                    var cmd = GetCmd(x, y);
                    if (cmd !=null)
                    {
						new MachineGCodeHelper().SendCommandAsync(cmd.CommandString).ConfigureAwait(false).GetAwaiter().GetResult();
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
