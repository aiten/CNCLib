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

using System.Collections.ObjectModel;

namespace CNCLib.Wpf.Models
{
	public class Machine 
	{
        public int MachineID { get; set; }

		public string ComPort { get; set; }
		public int BaudRate { get; set; }
		public int Axis { get; set; }

		public string Name { get; set; }

		public decimal SizeX { get; set; }
		public decimal SizeY { get; set; }
		public decimal SizeZ { get; set; }
		public decimal SizeA { get; set; }
		public decimal SizeB { get; set; }
		public decimal SizeC { get; set; }

		public int BufferSize { get; set; }

		public bool CommandToUpper { get; set; }

 		public decimal ProbeSizeX { get; set; }
		public decimal ProbeSizeY { get; set; }
		public decimal ProbeSizeZ { get; set; }
		public decimal ProbeDistUp { get; set; }
		public decimal ProbeDist { get; set; }
		public decimal ProbeFeed { get; set; }

		public bool SDSupport { get; set; }
		public bool Spindle { get; set; }
		public bool Coolant { get; set; }
        public bool Laser { get; set; }
        public bool Rotate { get; set; }

		private ObservableCollection<Models.MachineCommand> _MachineCommands;

		public ObservableCollection<Models.MachineCommand> MachineCommands
		{
			get
			{
				if (_MachineCommands == null)
				{
					_MachineCommands = new ObservableCollection<Models.MachineCommand>();
					_MachineCommands.CollectionChanged += ((sender, e) =>
					{
						if (e.NewItems != null)
						{
							foreach (Models.MachineCommand item in e.NewItems)
							{
								item.MachineID = MachineID;
							}
						}
					});
				}
				return _MachineCommands;
			}
		}

		private ObservableCollection<Models.MachineInitCommand> _MachineInitCommands;

		public ObservableCollection<Models.MachineInitCommand> MachineInitCommands
		{
			get
			{
				if (_MachineInitCommands == null)
				{
					_MachineInitCommands = new ObservableCollection<Models.MachineInitCommand>();
					_MachineInitCommands.CollectionChanged += ((sender, e) =>
					{
						if (e.NewItems != null)
						{
							foreach (Models.MachineInitCommand item in e.NewItems)
							{
								item.MachineID = MachineID;
							}
						}
					});
				}
				return _MachineInitCommands;
			}
		}
	}
}
