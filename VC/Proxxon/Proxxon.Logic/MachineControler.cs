using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;
using System.Data.Entity;
using Framework.EF;
using Proxxon.Repository;
using Proxxon.Repository.Context;

namespace Proxxon.Logic
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
			return ObjectConverter.NewCloneProperties<DTO.Machine, Proxxon.Repository.Entities.Machine>(machine);
        }

        public void Delete(DTO.Machine m)
        {
			new MachineRepository().Delete(m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>());
        }

		public DTO.MachineCommand[] GetMachineCommands(int machineID)
		{
			var machines = new MachineRepository().GetMachineCommands(machineID);
			List<DTO.MachineCommand> l = new List<DTO.MachineCommand>();
			l.AddCloneProperties(machines);
			return l.ToArray();
		}

		public int StoreMachine(DTO.Machine m)
		{
			var me = m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>();
			me.MachineCommands = m.MachineCommands.ToArray().CloneProperties<Proxxon.Repository.Entities.MachineCommand, Proxxon.Logic.DTO.MachineCommand>();

			return new MachineRepository().StoreMachine(me);
		}
    }
}
