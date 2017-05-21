////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;
using System.Collections.Generic;
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System.Threading.Tasks;

namespace CNCLib.Logic
{
    public class MachineController : ControllerBase, IMachineController
	{
		public async Task<IEnumerable<Machine>> GetAll()
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var machines = await rep.GetMachines();
				List<Machine> l = new List<Machine>();
				foreach (var m in machines)
				{
					l.Add(m.Convert());
				}
				return (IEnumerable < Machine >) l;
			}
		}

        public async Task<Machine> Get(int id)
        {
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var machine = await rep.GetMachine(id);
				if (machine == null)
					return null;

				var dto = machine.Convert();
				return dto;
			}
		}

		public async Task Delete(Machine m)
        {
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				await rep.Delete(m.Convert());
				await uow.Save();
			}
		}

		public async Task<int> Add(Machine m)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var me = m.Convert();
				me.MachineID = 0;
				foreach (var mc in me.MachineInitCommands) mc.MachineID = 0;
				foreach (var mi in me.MachineInitCommands) mi.MachineID = 0;
				await rep.Store(me);
				await uow.Save();
				return me.MachineID;
			}
		}

		public async Task<int> Update(Machine m)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var me = m.Convert();
				await rep.Store(me);
				await uow.Save();
				return me.MachineID;
			}
		}

		#region Default machine

		public async Task<Machine> DefaultMachine()
		{
			var dto = new Machine()
			{
				Name = "New",
				ComPort = "comX",
				Axis = 3,
				SizeX = 130m,
				SizeY = 45m,
				SizeZ = 81m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 115200,
				BufferSize = 63,
				CommandToUpper = false,
				ProbeSizeZ = 25,
				ProbeDist = 10m,
				ProbeDistUp = 3m,
				ProbeFeed = 100m,
				SDSupport = true,
				Spindle = true,
				Coolant = true,
				Rotate = true,
				Laser = false,
				MachineCommands = new MachineCommand[0],
				MachineInitCommands = new MachineInitCommand[0]
			};
			return await Task.FromResult(dto);
		}

		public async Task<int> GetDetaultMachine()
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
			{
				var config = await rep.Get("Environment", "DefaultMachineID");

				if (config == default(Repository.Contracts.Entities.Configuration))
					return -1;

				return int.Parse(config.Value);
			}
		}

		public async Task SetDetaultMachine(int defaultMachineID)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
			{
				await rep.Save(new Repository.Contracts.Entities.Configuration() { Group = "Environment", Name = "DefaultMachineID", Type = "Int32", Value = defaultMachineID.ToString() });
				await uow.Save();
			}
		}

        #endregion

        #region IDisposable Support
        // see ControllerBase
        #endregion
    }
}
