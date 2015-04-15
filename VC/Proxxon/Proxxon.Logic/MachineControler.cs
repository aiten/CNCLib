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

		public int StoreMachine(DTO.Machine m)
		{
			int id = m.MachineID;
			using (System.Data.Entity.DbContextTransaction dbTran = Context.Database.BeginTransaction())
			{
				try
				{
					var existingmachines = Repository.Query<Proxxon.Repository.Entities.Machine>().Where((mm) => mm.MachineID == id).FirstOrDefault();

					if (existingmachines == default(Proxxon.Repository.Entities.Machine))
					{
						existingmachines = m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>();
						Context.Entry(existingmachines).State = EntityState.Added;
						Context.SaveChanges();
						id = existingmachines.MachineID;
						foreach (DTO.MachineCommand mc in m.MachineCommands)
						{
							mc.MachineID = id;
						}
					}
					else
					{
						ObjectConverter.CopyProperties(existingmachines,m);
					}

					var existingmachineCommands = Repository.Query<Proxxon.Repository.Entities.MachineCommand>().Where(c => c.MachineID == id).ToList();
					var machineCommands = m.MachineCommands.ToArray().CloneProperties<Proxxon.Repository.Entities.MachineCommand, Proxxon.Logic.DTO.MachineCommand>();

					foreach (Proxxon.Repository.Entities.MachineCommand existing_mc in existingmachineCommands)
					{
						var foundmc = machineCommands.FirstOrDefault(x => x.MachineCommandID == existing_mc.MachineCommandID);
						if (foundmc == default(Proxxon.Repository.Entities.MachineCommand))
						{
							Context.Entry(existing_mc).State = EntityState.Deleted;
						}
						else
						{
							ObjectConverter.CopyProperties(existingmachines, m);
						}
					}

					foreach (Proxxon.Repository.Entities.MachineCommand this_mc in machineCommands)
					{
						var foundmc = existingmachineCommands.FirstOrDefault(x => x.MachineCommandID == this_mc.MachineCommandID);
						if (foundmc == default(Proxxon.Repository.Entities.MachineCommand))
						{
							Context.Entry(this_mc).State = EntityState.Added;
						}
					}
					
					//saves all above operations within one transaction
					Context.SaveChanges();

					//commit transaction
					dbTran.Commit();
				}
				catch (Exception ex)
				{
					dbTran.Rollback();
					throw;
				}

				return id;

			}
		}

    }
}
