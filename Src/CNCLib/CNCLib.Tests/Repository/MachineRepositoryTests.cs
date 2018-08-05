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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using FluentAssertions;
using Framework.EF;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    [TestClass]
    public class MachineRepositoryTests : CRUDRepositoryTests<Machine, int, IMachineRepository>
    {
        #region crt and overrides

        protected override CRUDTestContext<Machine, int, IMachineRepository> CreateCRUDTestContext()
        {
            return Dependency.Resolve<CRUDTestContext<Machine, int, IMachineRepository>>();
        }

        [ClassInitialize]
        public new static void ClassInit(TestContext testContext)
        {
            RepositoryTests.ClassInit(testContext);
        }

        protected override int GetEntityKey(Machine entity)
        {
            return entity.MachineID;
        }
        protected override Machine SetEntityKey(Machine entity, int key)
        {
            entity.MachineID = key;
            return entity;
        }

        protected override bool CompareEntity(Machine entity1, Machine entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserID)
            //);
            return Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"MachineID", @"MachineCommandID", @"MachineInitCommandID" });
        }

        #endregion

        #region CRUD Test

        [TestMethod]
        public async Task GetAllTest()
        {
            var entities = await GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Where(i => i.Name == "DC-K40-Laser").Count().Should().Be(1);
            entities.Where(i => i.Name == "Laser").Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.MachineID.Should().Be(1);
        }

        [TestMethod]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.MachineID.Should().Be(2);
        }

        [TestMethod]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [TestMethod]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(
                () => CreateMachine(@"AddUpdateDeleteTest"),
                (entity) => entity.Name = "DummyNameUpdate");
        }

        [TestMethod]
        public async Task AddUpdateDeleteWithPropertiesTest()
        {
            await AddUpdateDelete(
                () => AddMachinInitCommands((AddMachinCommands(CreateMachine(@"AddUpdateDeleteWithPropertiesTest")))),
                (entity) => entity.Name = "DummyNameUpdate");
        }

        [TestMethod]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => CreateMachine(@"AddRollbackTest"));
        }

        #endregion

        [TestMethod]
        public async Task QueryAllMachines()
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                var machines = await rep.GetAll();
				machines.Count().Should().BeGreaterOrEqualTo(2);
			}
	    }

		[TestMethod]
		public async Task QueryOneMachineFound()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                var machines = await rep.Get(1);
				machines.MachineID.Should().Be(1);
			}
		}

		[TestMethod]
		public async Task QueryOneMachineNotFound()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                var machines = await rep.Get(1000);
				machines.Should().BeNull();
			}
		}

		[TestMethod]
		public async Task AddOneMachine()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                var machine = CreateMachine("AddOneMachine");
				await rep.Store(machine);
				await uow.SaveChangesAsync();
				machine.MachineID.Should().NotBe(0);
			}
		}
		[TestMethod]
		public async Task AddOneMachineWithCommands()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                var machine = CreateMachine("AddOneMachineWithCommands");
				AddMachinCommands(machine);
				await rep.Store(machine);
				await uow.SaveChangesAsync();
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
            int count = AddMachinCommands(machine).MachineCommands.Count();
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
            int count = AddMachinInitCommands(machine).MachineInitCommands.Count();
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

           using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                await rep.Store(machine);
				await uow.SaveChangesAsync();
                id = machine.MachineID;
                id.Should().NotBe(0);

                machine.Name = "UpdateOneMachineAndRead#2";

				await rep.Store(machine);
				await uow.SaveChangesAsync();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(0);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task UpdateOneMachineNoCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine).MachineCommands.Count();
            int id;

           using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                await rep.Store(machine);
				await uow.SaveChangesAsync();
                id = machine.MachineID;
                id.Should().NotBe(0);

                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
				await rep.Store(machine);
				await uow.SaveChangesAsync();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(count);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task UpdateOneMachineCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine).MachineCommands.Count();
            int id = await WriteMachine(machine);
            int newcount;

           using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
                machine.MachineCommands.Add(new MachineCommand { CommandName = "Name#1", CommandString = "New#1", MachineID = id });
                machine.MachineCommands.Add(new MachineCommand { CommandName = "Name#2", CommandString = "New#2", MachineID = id });
                machine.MachineCommands.Remove(machine.MachineCommands.Single(m => m.CommandString == "Test1"));
                machine.MachineCommands.Single(m => m.CommandString == "Test2").CommandString = "Test2.Changed";

                newcount = count + 2 - 1;

				await rep.Store(machine);
				await uow.SaveChangesAsync();
            }

            var machineread = await ReadMachine(id);
            machineread.MachineCommands.Count.Should().Be(newcount);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public async Task DeleteMachineWithCommandAndRead()
       {
            var machine = CreateMachine("DeleteMachineWithCommandAndRead");
            int count = AddMachinCommands(machine).MachineCommands.Count();
            int id = await WriteMachine(machine);

           using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                rep.Delete(machine);
				await uow.SaveChangesAsync();

                (await rep.Get(id)).Should().BeNull();
            }
       }

        private static async Task<int> WriteMachine(Machine machine)
        {
            int id;

            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                await rep.Store(machine);
				await uow.SaveChangesAsync();
                id = machine.MachineID;
                id.Should().NotBe(0);
            }

            return id;
        }

        private static async Task<Machine> ReadMachine(int id)
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new MachineRepository(ctx);
                return await rep.Get(id);
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
				ProbeFeed = 1m,
                MachineInitCommands = new HashSet<MachineInitCommand>(),
                MachineCommands = new HashSet<MachineCommand>()
			};
			return machine;
		}

		private static Machine AddMachinCommands(Machine machine)
		{
		    machine.MachineCommands = new List<MachineCommand>
		    {
		        new MachineCommand {CommandName = "Name1", CommandString = "Test1", Machine = machine},
		        new MachineCommand {CommandName = "Name1", CommandString = "Test2", Machine = machine}
		    };
		    return machine;
		}
		private static Machine AddMachinInitCommands(Machine machine)
		{
		    machine.MachineInitCommands = new List<MachineInitCommand>
		    {
		        new MachineInitCommand {SeqNo = 0, CommandString = "Test1", Machine = machine},
		        new MachineInitCommand {SeqNo = 1, CommandString = "Test2", Machine = machine}
		    };
		    return machine;
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
