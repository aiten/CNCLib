using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxxon.Repository.Context;
using Proxxon.Repository;
using Proxxon.Repository.Entities;
using System.Threading.Tasks;
using Framework.EF;

namespace Proxxon.Tests
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
				System.Data.Entity.Database.SetInitializer<ProxxonContext>(new ProxxonInitializer());
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
			Assert.AreEqual(2, machines.Length);
	    }

		[TestMethod]
		public void QueryOneMachines()
		{
			MachineRepository rep = new MachineRepository();

			var machines = rep.GetMachine(1);
			Assert.AreEqual(1, machines.MachineID);
		}

		[TestMethod]
		public void AddOneMachines()
		{
			MachineRepository rep = new MachineRepository();

			var machine = new Machine()
				{
					ComPort				= "com47",
					Axis				= 2,
					BaudRate			= 6500,
					Name				= "Test",
					SizeX				= 1m,
					SizeY				= 1m,
					SizeZ				= 1m,
					SizeA				= 1m,
					SizeB				= 1m,
					SizeC				= 1m,
					BufferSize			= 63,
					CommandToUpper    = true,
					Default			  = false,
					ProbeSizeX		  = 1m,
					ProbeSizeY		  = 1m,
					ProbeSizeZ		  = 1m,
					ProbeDistUp		  = 1m,
					ProbeDist		  = 1m,
					ProbeFeed		  = 1m
				};

			int id = rep.StoreMachine(machine);

			Assert.AreEqual(3, id);
		}

	}
}
