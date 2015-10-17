using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;
using Framework.EF;
using CNCLib.Repository;
using CNCLib.Repository.Context;

namespace CNCLib.Logic
{
    public class MachineControler
    {
		public DTO.Machine[] GetMachines()
		{
			var machines = new MachineRepository().GetMachines();
			List<DTO.Machine> l = new List<DTO.Machine>();
			l.AddCloneProperties(machines);
			return l.ToArray();
		}

        public DTO.Machine GetMachine(int id)
        {
			var machine = new MachineRepository().GetMachine(id);
			return ObjectConverter.NewCloneProperties<DTO.Machine, CNCLib.Repository.Entities.Machine>(machine);
        }

        public void Delete(DTO.Machine m)
        {
			new MachineRepository().Delete(m.NewCloneProperties<CNCLib.Repository.Entities.Machine, DTO.Machine>());
        }

		public DTO.MachineCommand[] GetMachineCommands(int machineID)
		{
			var machines = new MachineRepository().GetMachineCommands(machineID);
			List<DTO.MachineCommand> l = new List<DTO.MachineCommand>();
			l.AddCloneProperties(machines);
			return l.ToArray();
		}
		public DTO.MachineInitCommand[] GetMachineInitCommands(int machineID)
		{
			var machines = new MachineRepository().GetMachineInitCommands(machineID);
			List<DTO.MachineInitCommand> l = new List<DTO.MachineInitCommand>();
			l.AddCloneProperties(machines);
			return l.ToArray();
		}

		public int StoreMachine(DTO.Machine m)
		{
			var me = m.NewCloneProperties<CNCLib.Repository.Entities.Machine, DTO.Machine>();
			me.MachineCommands = m.MachineCommands.ToArray().CloneProperties<CNCLib.Repository.Entities.MachineCommand, CNCLib.Logic.DTO.MachineCommand>();
			me.MachineInitCommands = m.MachineInitCommands.ToArray().CloneProperties<CNCLib.Repository.Entities.MachineInitCommand, CNCLib.Logic.DTO.MachineInitCommand>();

			return new MachineRepository().StoreMachine(me);
		}

		#region Default machine

		public int GetDetaultMachine()
		{
			var config = new ConfigurationRepository().Get("Environment","DefaultMachineID");

			if (config == default(CNCLib.Repository.Entities.Configuration))
				return -1;

			return int.Parse(config.Value);
		}
		public void SetDetaultMachine(int defulatMachineID)
		{
			new ConfigurationRepository().Save(new Repository.Entities.Configuration() { Group = "Environment", Name = "DefaultMachineID", Type = "Int32", Value = defulatMachineID.ToString() } );
		}


		#endregion
	}
}
