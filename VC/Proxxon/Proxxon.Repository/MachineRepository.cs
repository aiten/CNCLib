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

		public int StoreMachine(Entities.Machine machine)
		{
			// search und update machine

			using (IUnitOfWork uow = new UnitOfWork<ProxxonContext>())
			{
				int id = machine.MachineID;

				using (System.Data.Entity.DbContextTransaction dbTran = uow.Context.Database.BeginTransaction())
				{
					try
					{
						var machineInDb = uow.Query<Entities.Machine>().Where((m) => m.MachineID == id).Include((d) => d.MachineCommands).FirstOrDefault();

						if (machineInDb == default(Entities.Machine))
						{
							machineInDb.CopyValueTypeProperties(machine);
							uow.Context.Entry(machineInDb).State = EntityState.Added;
							uow.Save();
							id = machineInDb.MachineID;

							foreach (Entities.MachineCommand mc in machine.MachineCommands)
							{
								mc.MachineID = id;
							}
						}
						else
						{
							machineInDb.CopyValueTypeProperties(machine);
						}

						// search und update machinecommands (add and delete)

						// 1. Delete from DB (in DB) and update
						List<Entities.MachineCommand> delete = new List<Entities.MachineCommand>();

						foreach (Entities.MachineCommand commandInDb in  machineInDb.MachineCommands)
						{
							var command = machine.MachineCommands.FirstOrDefault(x => x.MachineCommandID == commandInDb.MachineCommandID);
							if (command == default(Entities.MachineCommand))
							{
								delete.Add(commandInDb);
							}
							else
							{
								commandInDb.CopyValueTypeProperties(command);
							}
						}

						foreach (var del in delete)
						{
							uow.Context.Entry(del).State = EntityState.Deleted;
						}

						// 2. Add To DB

						foreach (Entities.MachineCommand machineCommand in machine.MachineCommands)
						{
							var command = machineInDb.MachineCommands.FirstOrDefault(x => x.MachineCommandID == machineCommand.MachineCommandID);
							if (command == default(Entities.MachineCommand))
							{
								uow.Context.Entry(machineCommand).State = EntityState.Added;
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
