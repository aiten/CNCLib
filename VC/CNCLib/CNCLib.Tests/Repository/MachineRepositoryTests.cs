////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using CNCLib.Repository.Contracts.Entities;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;
using Framework.Tools;
using CNCLib.Repository.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class MachineRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public static new void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

        [TestMethod]
        public void QueryAllMachines()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = rep.GetMachines();
				Assert.AreEqual(true, machines.Length >= 2);
			}
	    }

		[TestMethod]
		public void QueryOneMachineFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = rep.GetMachine(1);
				Assert.AreEqual(1, machines.MachineID);
			}
		}

		[TestMethod]
		public void QueryOneMachineNotFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machines = rep.GetMachine(1000);
				Assert.IsNull(machines);
			}
		}

		[TestMethod]
		public void AddOneMachine()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machine = CreateMachine("AddOneMachine");
				rep.Store(machine);
                uow.Save();
				Assert.AreNotEqual(0, machine.MachineID);
			}
		}
		[TestMethod]
		public void AddOneMachineWithCommands()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machine = CreateMachine("AddOneMachineWithCommands");
				AddMachinCommands(machine);
                rep.Store(machine);
                uow.Save();
                Assert.AreNotEqual(0, machine.MachineID);
            }
        }

		[TestMethod]
		public void AddOneMachineAndRead()
        {
            var machine = CreateMachine("AddOneMachineAndRead");
            int id = WriteMachine(machine);

            var machineread = ReadMachine(id);

            Assert.AreEqual(0, machineread.MachineCommands.Count);
            Assert.AreEqual(0, machineread.MachineInitCommands.Count);

            CompareMachine(machine, machineread);
        }

       [TestMethod]
       public void AddOneMachineWithCommandsAndRead()
       {
            var machine = CreateMachine("AddOneMachineWithCommandsAndRead");
            int count = AddMachinCommands(machine);
            int id = WriteMachine(machine);

            var machineread = ReadMachine(id);

            Assert.AreEqual(count, machineread.MachineCommands.Count);
            Assert.AreEqual(0, machineread.MachineInitCommands.Count);

            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public void AddOneMachineWithInitCommandsAndRead()
       {
            var machine = CreateMachine("AddOneMachineWithInitCommandsAndRead");
            int count = AddMachinInitCommands(machine);
            int id = WriteMachine(machine);

            var machineread = ReadMachine(id);

            Assert.AreEqual(0, machineread.MachineCommands.Count);
            Assert.AreEqual(count, machineread.MachineInitCommands.Count);

            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public void UpdateOneMachineAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineAndRead");
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                rep.Store(machine);
                uow.Save();
                id = machine.MachineID;
                Assert.AreNotEqual(0, id);

                machine.Name = "UpdateOneMachineAndRead#2";

                rep.Store(machine);
                uow.Save();
            }

            var machineread = ReadMachine(id);
            Assert.AreEqual(0, machineread.MachineCommands.Count);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public void UpdateOneMachineNoCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine);
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                rep.Store(machine);
                uow.Save();
                id = machine.MachineID;
                Assert.AreNotEqual(0, id);

                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
                rep.Store(machine);
                uow.Save();
            }

            var machineread = ReadMachine(id);
            Assert.AreEqual(count, machineread.MachineCommands.Count);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public void UpdateOneMachineCommandChangeAndRead()
       {
            var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
            int count = AddMachinCommands(machine);
            int id = WriteMachine(machine);
            int newcount;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
                machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name#1", CommandString = "New#1", MachineID = id });
                machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name#2", CommandString = "New#2", MachineID = id });
                machine.MachineCommands.Remove(machine.MachineCommands.Single(m => m.CommandString == "Test1"));
                machine.MachineCommands.Single(m => m.CommandString == "Test2").CommandString = "Test2.Changed";

                newcount = count + 2 - 1;

                rep.Store(machine);
                uow.Save();
            }

            var machineread = ReadMachine(id);
            Assert.AreEqual(newcount, machineread.MachineCommands.Count);
            CompareMachine(machine, machineread);
       }

       [TestMethod]
       public void DeleteMachineWithCommandAndRead()
       {
            var machine = CreateMachine("DeleteMachineWithCommandAndRead");
            int count = AddMachinCommands(machine);
            int id = WriteMachine(machine);

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                rep.Delete(machine);
                uow.Save();

                Assert.IsNull(rep.GetMachine(id));
            }
       }

        private static int WriteMachine(Machine machine)
        {
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                rep.Store(machine);
                uow.Save();
                id = machine.MachineID;
                Assert.AreNotEqual(0, id);
            }

            return id;
        }

        private static Machine ReadMachine(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                return rep.GetMachine(id);
            }
        }


        private static Machine CreateMachine(string name)
		{
			var machine = new Machine()
			{
				ComPort = "com47",
				Axis = 2,
				BaudRate = 6500,
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
			int count = 2;
			machine.MachineCommands = new List<MachineCommand>();
			machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name1", CommandString = "Test1" });
			machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name1", CommandString = "Test2" });
			return count;
		}
		private static int AddMachinInitCommands(Machine machine)
		{
			int count = 2;
			machine.MachineInitCommands = new List<MachineInitCommand>();
			machine.MachineInitCommands.Add(new MachineInitCommand() { SeqNo = 0, CommandString = "Test1" });
			machine.MachineInitCommands.Add(new MachineInitCommand() { SeqNo = 1, CommandString = "Test2" });
			return count;
		}

		private static void CompareMachine(Machine machine, Machine machineread)
		{
			Assert.AreEqual(true, machineread.CompareProperties(machine));
            Assert.AreEqual(machine.MachineCommands == null ? 0 : machine.MachineCommands.Count, machineread.MachineCommands.Count);
			Assert.AreEqual(machine.MachineInitCommands == null ? 0 : machine.MachineInitCommands.Count, machineread.MachineInitCommands.Count);

			foreach (MachineCommand mc in machineread.MachineCommands)
			{
				Assert.AreEqual(true, mc.CompareProperties(machine.MachineCommands.Single(m => m.MachineCommandID == mc.MachineCommandID)));
			}
			foreach (MachineInitCommand mc in machineread.MachineInitCommands)
			{
				Assert.AreEqual(true, mc.CompareProperties(machine.MachineInitCommands.Single(m => m.MachineInitCommandID == mc.MachineInitCommandID)));
			}
		}
	}
}
