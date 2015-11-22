using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;
using CNCLib.Logic;

namespace CNCLib.Wpf.Models
{
	static class Converter
	{
		public static Logic.DTO.Machine Convert(this Machine from)
		{
			var dto =	ObjectConverter.NewCloneProperties<Logic.DTO.Machine, Machine>(from);
			dto.MachineCommands = from.MachineCommands.CloneAsList<CNCLib.Logic.DTO.MachineCommand, Models.MachineCommand>();
			dto.MachineInitCommands = from.MachineInitCommands.CloneAsList<CNCLib.Logic.DTO.MachineInitCommand, Models.MachineInitCommand>();
			return dto;
		}

		public static Machine Convert(this Logic.DTO.Machine from)
		{
			var model = ObjectConverter.NewCloneProperties<Machine, Logic.DTO.Machine>(from);
			model.MachineCommands.Count();  //force init
			model.MachineInitCommands.Count();

			model.MachineCommands.AddCloneProperties(from.MachineCommands);
			model.MachineInitCommands.AddCloneProperties(from.MachineInitCommands);
			return model;
		}
	}
}
