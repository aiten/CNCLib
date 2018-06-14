////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Contracts.Entities;
using System.Collections.Generic;
using System.Linq;
using CNCLib.Repository.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System.Threading.Tasks;
using FluentAssertions;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class MachineRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public new static void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

        [TestMethod]
        public async Task QueryAllMachines()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = await rep.GetMachines();
				machines.Length.Should().BeGreaterOrEqualTo(2);
			}
	    }

		[TestMethod]
		public async Task QueryOneMachineFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = await rep.GetMachine(1);
				machines.MachineID.Should().Be(1);
			}
		}

		[TestMethod]
		public async Task QueryOneMachineNotFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = await rep.GetMachine(1000);
				machines.Should().BeNull();
			}
		}

		[TestMethod]
		public async Task AddOneMachine()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machine = CreateMachine("AddOneMachine");
				await rep.Store(machine);
				await uow.Save();
				machine.MachineID.Should().NotBe(0);
			}
		}
		[TestMethod]
		public async Task AddOneMachineWithCommands()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machine = CreateMachine("AddOneMachineWithCommands");
				AddMachinCommands(machine);
				await rep.Store(machine);
				await uow.Save();
                 machine.MachineID.Should().NotBe(0);
            }
        }

		[TestMethod]
		public async Task AddOneMachineAndRead()
        {
            var machine = CreateMachine("AddOneMachineAndRead");
            int id = await WriteMachine(machine);

            var machineread = await ReadMachine(id);

            machineread.MachineCommands.Count.Should().Be(0);
            machineread.MachineInitCommands.Count.Should().Be(0);

            CompareMachine(machine, machineread);
        }

       [TestMethod]
       public async Task AddOneMachineWithCommandsAndRead()
       {
            var machine = CreateMachine("AddOneMachineWithCommandsAndRead");
            int count = AddMachinCommands(machine);
            int id = await WriteMachine(machine);

            var machineread = await ReadMachine(id);

            machineread.MachineCommands.Count.Should().Be(count);
            machineread.MachineInitCommands.Count.Should().Be(0);

            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task AddOneMachineWithInitCommandsAndRead()
       {
            var machine = CreateMachine("AddOneMachineWithInitCommandsAndRead");
            int count = AddMachinInitCommands(machine);
            int id = await WriteMachine(machine);

            var machineread = await ReadMachine(id);

            machineread.MachineCommands.Count.Should().Be(0);
            machineread.MachineInitCommands.Count.Should().Be(count);

            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task UpdateOneMachineAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineAndRead");
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
				await rep.Store(machine);
				await uow.Save();
                id = machine.MachineID;
                id.Should().NotBe(0);

                machine.Name = "UpdateOneMachineAndRead#2";

				await rep.Store(machine);
				await uow.Save();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(0);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task UpdateOneMachineNoCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine);
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
				await rep.Store(machine);
				await uow.Save();
                id = machine.MachineID;
                id.Should().NotBe(0);

                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
				await rep.Store(machine);
				await uow.Save();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(count);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task UpdateOneMachineCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine);
            int id = await WriteMachine(machine);
            int newcount;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
                machine.MachineCommands.Add(new MachineCommand { CommandName = "Name#1", CommandString = "New#1", MachineID = id });
                machine.MachineCommands.Add(new MachineCommand { CommandName = "Name#2", CommandString = "New#2", MachineID = id });
                machine.MachineCommands.Remove(machine.MachineCommands.Single(m => m.CommandString == "Test1"));
                machine.MachineCommands.Single(m => m.CommandString == "Test2").CommandString = "Test2.Changed";

                newcount = count + 2 - 1;

				await rep.Store(machine);
				await uow.Save();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(newcount);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task DeleteMachineWithCommandAndRead()
       {
            var machine = CreateMachine("DeleteMachineWithCommandAndRead");
            int count = AddMachinCommands(machine);
            int id = await WriteMachine(machine);

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
				await rep.Delete(machine);
				await uow.Save();

                (await rep.GetMachine(id)).Should().BeNull();
            }
       }

        private static async Task<int> WriteMachine(Machine machine)
        {
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
				await rep.Store(machine);
				await uow.Save();
                id = machine.MachineID;
                id.Should().NotBe(0);
            }

            return id;
        }

        private static async Task<Machine> ReadMachine(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                return await rep.GetMachine(id);
            }
        }

        private static Machine CreateMachine(string name)
		{
			var machine = new Machine
			{
				ComPort = "com47",
				Axis = 2,
				BaudRate = 6500,
                DtrIsReset = true,
                Name = name,
				SizeX = 1m,
				SizeY = 1m,
				SizeZ = 1m,
				SizeA = 1m,
				SizeB = 1m,
				SizeC = 1m,
				BufferSize = 63,
				CommandToUpper = true,
				ProbeSizeX = 1m,
				ProbeSizeY = 1m,
				ProbeSizeZ = 1m,
				ProbeDistUp = 1m,
				ProbeDist = 1m,
				ProbeFeed = 1m
			};
			return machine;
		}

		private static int AddMachinCommands(Machine machine)
		{
		    machine.MachineCommands = new List<MachineCommand>
		    {
		        new MachineCommand {CommandName = "Name1", CommandString = "Test1"},
		        new MachineCommand {CommandName = "Name1", CommandString = "Test2"}
		    };
		    return machine.MachineCommands.Count;
		}
		private static int AddMachinInitCommands(Machine machine)
		{
		    machine.MachineInitCommands = new List<MachineInitCommand>
		    {
		        new MachineInitCommand {SeqNo = 0, CommandString = "Test1"},
		        new MachineInitCommand {SeqNo = 1, CommandString = "Test2"}
		    };
		    return machine.MachineInitCommands.Count;
		}

		private static void CompareMachine(Machine machine, Machine machineread)
		{
			machineread.CompareProperties(machine).Should().Be(true);
            (machine.MachineCommands?.Count ?? 0).Should().Be(machineread.MachineCommands.Count);
			(machine.MachineInitCommands?.Count ?? 0).Should().Be(machineread.MachineInitCommands.Count);

			foreach (MachineCommand mc in machineread.MachineCommands)
			{
				mc.CompareProperties(machine.MachineCommands.Single(m => m.MachineCommandID == mc.MachineCommandID)).Should().Be(true);
			}
			foreach (MachineInitCommand mc in machineread.MachineInitCommands)
			{
				mc.CompareProperties(machine.MachineInitCommands.Single(m => m.MachineInitCommandID == mc.MachineInitCommandID)).Should().Be(true);
			}
		}
	}
}
