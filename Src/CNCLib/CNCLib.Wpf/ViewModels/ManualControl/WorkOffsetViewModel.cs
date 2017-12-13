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

using System.Windows.Input;
using System.Windows.Media.Converters;
using CNCLib.Wpf.Helpers;
using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels.ManualControl
{
	public class WorkOffsetViewModel : DetailViewModel
	{
		public WorkOffsetViewModel(IManualControlViewModel vm)
			: base(vm)
		{
		}

        #region Properties

        private decimal? _g54X;
	    public decimal? G54X
	    {
	        get => _g54X;
	        set => SetProperty(ref _g54X, value);
	    }

        private decimal? _g54Y;
	    public decimal? G54Y
	    {
	        get => _g54Y;
	        set => SetProperty(ref _g54Y, value);
	    }

        private decimal? _g54Z;
	    public decimal? G54Z
	    {
	        get => _g54Z;
	        set => SetProperty(ref _g54Z, value);
	    }


        #endregion

        #region Commands / CanCommands

        public void SendG53() { RunAndUpdate(() => { Com.QueueCommand("g53"); }); }
		public void SendG54() { RunAndUpdate(() => { Com.QueueCommand("g54"); }); }
		public void SendG55() { RunAndUpdate(() => { Com.QueueCommand("g55"); }); }
		public void SendG56() { RunAndUpdate(() => { Com.QueueCommand("g56"); }); }
		public void SendG57() { RunAndUpdate(() => { Com.QueueCommand("g57"); }); }
		public void SendG58() { RunAndUpdate(() => { Com.QueueCommand("g58"); }); }
		public void SendG59() { RunAndUpdate(() => { Com.QueueCommand("g59"); }); }

	    public void GetG5x(int offsetG)
	    {
	        RunAndUpdate(async () =>
	        {
/*
	            string message = await Com.SendCommandAndReadOKReplyAsync(MachineGCodeHelper.PrepareCommand("m114"));

	            if (!string.IsNullOrEmpty(message))
	            {
	                message = message.Replace("ok", "");
	                message = message.Replace(" ", "");
	                SetPositions(message.Split(':'), 0);
	            }

	            message = await Com.SendCommandAndReadOKReplyAsync(MachineGCodeHelper.PrepareCommand("m114 s1"));

	            if (!string.IsNullOrEmpty(message))
	            {
	                message = message.Replace("ok", "");
	                message = message.Replace(" ", "");
	                SetPositions(message.Split(':'), 1);
	            }
*/
	        });
        }
        public bool CanGetG5x(int offsetG)
	    {
	        return CanSendGCode();
	    }

	    public void SetG5x(int offsetG)
	    {
	        
	    }

	    public bool CanGSetG5x(int offsetG)
	    {
	        return CanSendGCode();
	    }

        #endregion

        #region ICommand
        public ICommand SendG53Command => new DelegateCommand(SendG53, CanSendGCode);
		public ICommand SendG54Command => new DelegateCommand(SendG54, CanSendGCode);
		public ICommand SendG55Command => new DelegateCommand(SendG55, CanSendGCode);
		public ICommand SendG56Command => new DelegateCommand(SendG56, CanSendGCode);
		public ICommand SendG57Command => new DelegateCommand(SendG57, CanSendGCode);
		public ICommand SendG58Command => new DelegateCommand(SendG58, CanSendGCode);
		public ICommand SendG59Command => new DelegateCommand(SendG59, CanSendGCode);
	    public ICommand GetG54Command => new DelegateCommand(() => GetG5x(0), () => CanGetG5x(0));
	    public ICommand SetG54Command => new DelegateCommand(() => SetG5x(0), () => CanSetG5x(0));

        #endregion
    }
}
