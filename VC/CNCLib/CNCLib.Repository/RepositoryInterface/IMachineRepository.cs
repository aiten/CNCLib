using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.RepositoryInterface
{
	public interface IMachineRepository: IDisposable
	{
		Entities.Machine[] GetMachines();
		Entities.Machine GetMachine(int id);
		void Delete(Entities.Machine m);
		Entities.MachineCommand[] GetMachineCommands(int machineID);
		Entities.MachineInitCommand[] GetMachineInitCommands(int machineID);
		int StoreMachine(Entities.Machine machine);
	}
}
