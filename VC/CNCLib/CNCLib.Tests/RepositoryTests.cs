using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using CNCLib.Repository.Entities;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;

namespace CNCLib.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {           
            //drop and recreate the test Db everytime the tests are run. 
            AppDomain.CurrentDomain.SetData("DataDirectory", testContext.TestDeploymentDir);

			using (var uow = UnitOfWorkFactory.Create())
            {
				System.Data.Entity.Database.SetInitializer<CNCLibContext>(new CNCLibInitializer());
				uow.InitializeDatabase();        
            }        
        }

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void QueryAllMachines()
        {
			MachineRepository rep = new MachineRepository();

			var machines = rep.GetMachines();
			Assert.AreEqual(true, machines.Length >= 2);
	    }

		[TestMethod]
		public void QueryOneMachines()
		{
			MachineRepository rep = new MachineRepository();

			var machines = rep.GetMachine(1);
			Assert.AreEqual(1, machines.MachineID);
		}

		[TestMethod]
		public void AddOneMachine()
		{
			MachineRepository rep = new MachineRepository();

			var machine = CreateMachine("AddOneMachine");

			int id = rep.StoreMachine(machine);

			Assert.AreNotEqual(0, id);
		}
		[TestMethod]
		public void AddOneMachineWithCommands()
		{
			MachineRepository rep = new MachineRepository();

			var machine = CreateMachine("AddOneMachineWithCommands");

			AddMachinCommands(machine);
			
			int id = rep.StoreMachine(machine);

			Assert.AreNotEqual(0, id);
		}
		[TestMethod]
		public void AddOneMachineAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("AddOneMachineAndRead");

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			Assert.AreEqual(0,machineread.MachineCommands.Count);

			CompareMachine(machine, machineread,0);
		}

		[TestMethod]
		public void AddOneMachineWithCommandsAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("AddOneMachineWithCommandsAndRead");
			int count = AddMachinCommands(machine);

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			CompareMachine(machine, machineread,count);
		}


		[TestMethod]
		public void UpdateOneMachineAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("UpdateOneMachineAndRead");

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			machine.Name = "UpdateOneMachineAndRead#2";

			repwrite.StoreMachine(machine);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			Assert.AreEqual(0, machineread.MachineCommands.Count);

			CompareMachine(machine, machineread,0);
		}

		[TestMethod]
		public void UpdateOneMachineNoCommandChangeAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
			int count = AddMachinCommands(machine);

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";

			repwrite.StoreMachine(machine);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			Assert.AreEqual(count, machineread.MachineCommands.Count);

			CompareMachine(machine, machineread,count);
		}

		[TestMethod]
		public void UpdateOneMachineCommandChangeAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("UpdateOneMachineNoCommandChangeAndRead");
			int count = AddMachinCommands(machine);

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			machine.Name = "UpdateOneMachineNoCommandChangeAndRead#2";
			machine.MachineCommands.Add(new MachineCommand() { CommandString = "New#1", MachineID = id });
			machine.MachineCommands.Add(new MachineCommand() { CommandString = "New#2", MachineID = id });
			machine.MachineCommands.Remove(machine.MachineCommands.Single(m => m.CommandString == "Test1"));
			machine.MachineCommands.Single(m => m.CommandString == "Test2").CommandString = "Test2.Changed";

			int newcount = count + 2 - 1;

			repwrite.StoreMachine(machine);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			Assert.AreEqual(newcount, machineread.MachineCommands.Count);

			CompareMachine(machine, machineread, newcount);
		}

		[TestMethod]
		public void DeleteMachineWithCommandAndRead()
		{
			MachineRepository repwrite = new MachineRepository();

			var machine = CreateMachine("DeleteMachineWithCommandAndRead");
			int count = AddMachinCommands(machine);

			int id = repwrite.StoreMachine(machine);

			Assert.AreNotEqual(0, id);

			MachineRepository repdelete = new MachineRepository();

			repdelete.Delete(machine);

			MachineRepository repread = new MachineRepository();

			var machineread = repread.GetMachine(id);

			//Assert.AreEqual(newcount, machineread.MachineCommands.Count);

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
			machine.MachineCommands.Add(new MachineCommand() { CommandString = "Test1" });
			machine.MachineCommands.Add(new MachineCommand() { CommandString = "Test2" });
			return count;
		}

		private static void CompareMachine(Machine machine, Machine machineread, int expectcount)
		{
			Assert.AreEqual(true, machineread.CompareProperties(machine));
			Assert.AreEqual(expectcount, machineread.MachineCommands.Count);

			foreach (MachineCommand mc in machineread.MachineCommands)
			{
				Assert.AreEqual(true, mc.CompareProperties(machine.MachineCommands.Single(m => m.MachineCommandID == mc.MachineCommandID)));
			}
		}

	}
}
