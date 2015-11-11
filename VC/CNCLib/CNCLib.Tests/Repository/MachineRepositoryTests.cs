////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using CNCLib.Repository.Entities;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;
using Framework.Tools;
using CNCLib.Repository.Interfaces;

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
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machines = rep.GetMachines();
				Assert.AreEqual(true, machines.Length >= 2);
			}
	    }

		[TestMethod]
		public void QueryOneMachines()
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machines = rep.GetMachine(1);
				Assert.AreEqual(1, machines.MachineID);
			}
		}

		[TestMethod]
		public void AddOneMachine()
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machine = CreateMachine("AddOneMachine");
				int id = rep.StoreMachine(machine);
				Assert.AreNotEqual(0, id);
			}
		}
		[TestMethod]
		public void AddOneMachineWithCommands()
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machine = CreateMachine("AddOneMachineWithCommands");
				AddMachinCommands(machine);
				int id = rep.StoreMachine(machine);
				Assert.AreNotEqual(0, id);
			}
		}
		[TestMethod]
		public void AddOneMachineAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{

				var machine = CreateMachine("AddOneMachineAndRead");

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(0, machineread.MachineCommands.Count);
				Assert.AreEqual(0, machineread.MachineInitCommands.Count);

				CompareMachine(machine, machineread);
			}
		}

		[TestMethod]
		public void AddOneMachineWithCommandsAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{

				var machine = CreateMachine("AddOneMachineWithCommandsAndRead");
				int count = AddMachinCommands(machine);

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(count, machineread.MachineCommands.Count);
				Assert.AreEqual(0, machineread.MachineInitCommands.Count);

				CompareMachine(machine, machineread);
			}
		}

		[TestMethod]
		public void AddOneMachineWithInitCommandsAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{
				var machine = CreateMachine("AddOneMachineWithInitCommandsAndRead");
				int count = AddMachinInitCommands(machine);

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(0, machineread.MachineCommands.Count);
				Assert.AreEqual(count, machineread.MachineInitCommands.Count);

				CompareMachine(machine, machineread);
			}
		}


		[TestMethod]
		public void UpdateOneMachineAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{

				var machine = CreateMachine("UpdateOneMachineAndRead");

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				machine.Name = "UpdateOneMachineAndRead#2";

				repwrite.StoreMachine(machine);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(0, machineread.MachineCommands.Count);

				CompareMachine(machine, machineread);
			}
		}

		[TestMethod]
		public void UpdateOneMachineNoCommandChangeAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{
				var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
				int count = AddMachinCommands(machine);

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";

				repwrite.StoreMachine(machine);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(count, machineread.MachineCommands.Count);

				CompareMachine(machine, machineread);
			}
		}

		[TestMethod]
		public void UpdateOneMachineCommandChangeAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			{

				var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
				int count = AddMachinCommands(machine);

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
				machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name#1", CommandString = "New#1", MachineID = id });
				machine.MachineCommands.Add(new MachineCommand() { CommandName = "Name#2", CommandString = "New#2", MachineID = id });
				machine.MachineCommands.Remove(machine.MachineCommands.Single(m => m.CommandString == "Test1"));
				machine.MachineCommands.Single(m => m.CommandString == "Test2").CommandString = "Test2.Changed";

				int newcount = count + 2 - 1;

				repwrite.StoreMachine(machine);

				var machineread = repread.GetMachine(id);

				Assert.AreEqual(newcount, machineread.MachineCommands.Count);

				CompareMachine(machine, machineread);
			}
		}

		[TestMethod]
		public void DeleteMachineWithCommandAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IMachineRepository>())
			using (var repread = RepositoryFactory.Create<IMachineRepository>())
			using (var repdelete = RepositoryFactory.Create<IMachineRepository>())
			{

				var machine = CreateMachine("DeleteMachineWithCommandAndRead");
				int count = AddMachinCommands(machine);

				int id = repwrite.StoreMachine(machine);

				Assert.AreNotEqual(0, id);

				repdelete.Delete(machine);

				var machineread = repread.GetMachine(id);

				//Assert.AreEqual(newcount, machineread.MachineCommands.Count);
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
