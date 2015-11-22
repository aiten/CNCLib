using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNCLib.Logic;

namespace CNCLib.Logic.Interfaces
{
	public interface IMachineControler : IDisposable
	{
		IEnumerable<DTO.Machine> GetMachines();
		DTO.Machine GetMachine(int id);
		void Delete(DTO.Machine m);

		int StoreMachine(DTO.Machine m);
		int GetDetaultMachine();
		void SetDetaultMachine(int defaultMachineID);
	}
}
