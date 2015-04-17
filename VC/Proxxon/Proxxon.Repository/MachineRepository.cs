using Proxxon.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxxon.Repository;
using Framework.Tools;
using System.Data.Entity;
using Framework.EF;

namespace Proxxon.Repository
{
    public class MachineRepository
    {
		public Entities.Machine[] GetMachines()
		{
			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				return uow.Query<Entities.Machine>().ToList().ToArray();
			}
		}

		public Entities.Machine GetMachine(int id)
        {
			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				return uow.Query<Entities.Machine>().Where((m) => m.MachineID == id).FirstOrDefault();
			}
        }

		public void Delete(Entities.Machine m)
        {
			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				uow.Context.Entry(m).State = EntityState.Deleted;
				uow.Save();
			}
        }

		public Entities.MachineCommand[] GetMachineCommands(int machineID)
		{
			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				return uow.Query<Proxxon.Repository.Entities.MachineCommand>().Where(c => c.MachineID == machineID).ToList().ToArray();
			}
		}

		public int StoreMachine(Entities.Machine m)
		{
			// search und update machine

			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				int id = m.MachineID;

				using (System.Data.Entity.DbContextTransaction dbTran = uow.Context.Database.BeginTransaction())
				{
					try
					{
						var existingmachine = uow.Query<Entities.Machine>().Where((mm) => mm.MachineID == id).Include((d) => d.MachineCommands).FirstOrDefault();

						if (existingmachine == default(Entities.Machine))
						{
							existingmachine = m.NewCloneProperties<Entities.Machine, Entities.Machine>();
							uow.Context.Entry(existingmachine).State = EntityState.Added;
							uow.Save();
							id = existingmachine.MachineID;
							foreach (Entities.MachineCommand mc in m.MachineCommands)
							{
								mc.MachineID = id;
							}
						}
						else
						{
							ObjectConverter.CopyProperties(existingmachine, m);
						}

						// search und update machinecommands (add and delete)

						// 1. Delete from DB (in DB) and update
						List<Entities.MachineCommand> delete = new List<Entities.MachineCommand>();

						foreach (Entities.MachineCommand existing_mc in  existingmachine.MachineCommands)
						{
							var foundmc = m.MachineCommands.FirstOrDefault(x => x.MachineCommandID == existing_mc.MachineCommandID);
							if (foundmc == default(Entities.MachineCommand))
							{
								delete.Add(existing_mc);
							}
							else
							{
								ObjectConverter.CopyProperties(foundmc, existing_mc);
							}
						}

						foreach (var dm in delete)
							uow.Context.Entry(dm).State = EntityState.Deleted;

						// 2. Add To DB

						foreach (Entities.MachineCommand this_mc in m.MachineCommands)
						{
							var foundmc = existingmachine.MachineCommands.FirstOrDefault(x => x.MachineCommandID == this_mc.MachineCommandID);
							if (foundmc == default(Entities.MachineCommand))
							{
								uow.Context.Entry(this_mc).State = EntityState.Added;
							}
						}

						uow.Save();
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
}
