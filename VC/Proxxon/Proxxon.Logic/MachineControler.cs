using Proxxon.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxxon.Repository;
using Framework.Tools;
using System.Data.Entity;

namespace Proxxon.Logic
{
    public class MachineControler : ControlerBase
    {
		public DTO.Machine[] GetMachines()
		{
			var machines = Repository.Query<Proxxon.Repository.Entities.Machine>().ToList();
			List<DTO.Machine> l = new List<DTO.Machine>();
            l.AddCloneProperties(machines);
			return l.ToArray();
		}

        public DTO.Machine GetMachine(int id)
        {
            var machines = Repository.Query<Proxxon.Repository.Entities.Machine>().Where((m) => m.MachineID == id).FirstOrDefault();
            return ObjectConverter.NewCloneProperties<DTO.Machine, Proxxon.Repository.Entities.Machine>(machines);
        }

        public void Update(DTO.Machine m)
        {
           Context.Entry(m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>()).State = EntityState.Modified;
           Context.SaveChanges(); 
        }
        public int Add(DTO.Machine m)
        {
            var entity = m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>();
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
            return entity.MachineID;
        }
        public void Delete(DTO.Machine m)
        {
            Context.Entry(m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>()).State = EntityState.Deleted;
            Context.SaveChanges();
        }

		public DTO.MachineCommand[] GetMachineCommands(int machineID)
		{
			var machineCommands = Repository.Query<Proxxon.Repository.Entities.MachineCommand>().Where(c => c.MachineID == machineID).ToList();
			List<DTO.MachineCommand> l = new List<DTO.MachineCommand>();
			l.AddCloneProperties(machineCommands);
			return l.ToArray();
		}
    }
}
