using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;

namespace CNCLib.Logic.Converter
{
	static class Converter
	{
		public static Logic.DTO.Machine Convert(this Repository.Entities.Machine from)
		{
			var dto =	ObjectConverter.NewCloneProperties<DTO.Machine, Repository.Entities.Machine>(from);
			dto.MachineCommands = ObjectConverter.CloneAsList<DTO.MachineCommand, Repository.Entities.MachineCommand>(from.MachineCommands);
			dto.MachineInitCommands = ObjectConverter.CloneAsList<DTO.MachineInitCommand, Repository.Entities.MachineInitCommand>(from.MachineInitCommands);
			return dto;
		}

		public static Repository.Entities.Machine Convert(this Logic.DTO.Machine from)
		{
			var entity = ObjectConverter.NewCloneProperties<Repository.Entities.Machine, DTO.Machine>(from);
			entity.MachineCommands = ObjectConverter.CloneAsList<Repository.Entities.MachineCommand, DTO.MachineCommand>(from.MachineCommands);
			entity.MachineInitCommands = ObjectConverter.CloneAsList<Repository.Entities.MachineInitCommand, DTO.MachineInitCommand>(from.MachineInitCommands);
			return entity;
		}
	}
}
