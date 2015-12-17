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
		public static Logic.Contracts.DTO.Machine Convert(this Machine from)
		{
			var dto =	ObjectConverter.NewCloneProperties<Logic.Contracts.DTO.Machine, Machine>(from);
			dto.MachineCommands = from.MachineCommands.CloneAsList<CNCLib.Logic.Contracts.DTO.MachineCommand, Models.MachineCommand>();
			dto.MachineInitCommands = from.MachineInitCommands.CloneAsList<CNCLib.Logic.Contracts.DTO.MachineInitCommand, Models.MachineInitCommand>();
			return dto;
		}

		public static Machine Convert(this Logic.Contracts.DTO.Machine from)
		{
			var model = ObjectConverter.NewCloneProperties<Machine, Logic.Contracts.DTO.Machine>(from);
			model.MachineCommands.Count();  //force init
			model.MachineInitCommands.Count();

			model.MachineCommands.AddCloneProperties(from.MachineCommands);
			model.MachineInitCommands.AddCloneProperties(from.MachineInitCommands);
			return model;
		}
	}
}
