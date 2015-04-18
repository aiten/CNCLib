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
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Entities.Machine>().ToList().ToArray();
			}
		}

		public Entities.Machine GetMachine(int id)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Entities.Machine>().Where((m) => m.MachineID == id).Include((d) => d.MachineCommands).FirstOrDefault();
			}
        }

		public void Delete(Entities.Machine m)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				uow.MarkDeleted(m);
				uow.Save();
			}
        }

		public Entities.MachineCommand[] GetMachineCommands(int machineID)
		{
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Proxxon.Repository.Entities.MachineCommand>().Where(c => c.MachineID == machineID).ToList().ToArray();
			}
		}

		public int StoreMachine(Entities.Machine machine)
		{
			// search und update machine

			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				int id = machine.MachineID;

				try
				{
					uow.BeginTransaction();

					var machineInDb = uow.Query<Entities.Machine>().Where((m) => m.MachineID == id).Include((d) => d.MachineCommands).FirstOrDefault();

					if (machineInDb == default(Entities.Machine))
					{
						machineInDb = machine;
						uow.MarkNew(machineInDb);
						uow.Save();
						id = machineInDb.MachineID;

						if (machine.MachineCommands != null)
						{
							foreach (Entities.MachineCommand mc in machine.MachineCommands)
							{
								mc.MachineID = id;
							}
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
						uow.MarkDeleted(del);
					}

					// 2. Add To DB

					foreach (Entities.MachineCommand machineCommand in machine.MachineCommands)
					{
						var command = machineInDb.MachineCommands.FirstOrDefault(x => x.MachineCommandID == machineCommand.MachineCommandID);
						if (command == default(Entities.MachineCommand))
						{
							uow.MarkNew(machineCommand);
						}
					}

					uow.Save();
					uow.CommitTransaction();
				}
				catch (Exception ex)
				{
					uow.RollbackTransaction();
					throw;
				}

				return id;
			}
		}
    }
}
