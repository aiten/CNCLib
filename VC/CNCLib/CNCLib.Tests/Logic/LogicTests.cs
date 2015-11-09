using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using CNCLib.Repository.Entities;
using CNCLib.Repository.RepositoryInterface;
using CNCLib.Logic;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Framework.Tools;

namespace CNCLib.Tests.Logic
{
	[TestClass]
	public class LogicTests
	{
/*
		[ClassInitialize]
		public static void ClassInit(TestContext testContext)
		{
		}

		[TestInitialize]
		public void Init()
		{
		}
*/
		[TestMethod]
		public void GetMachines()
		{
			var rep = Substitute.For< IMachineRepository>();
			Factory<IMachineRepository>.Register(rep);

			var machineEntity = new Machine[] { new Machine() { MachineID = 1, Name = "Maxi", BufferSize = 115200 } };
			rep.GetMachines().Returns(machineEntity);

			MachineControler ctrl = new MachineControler();

			var machines = ctrl.GetMachines();
			Assert.AreEqual(true, machines.Length == 1);
		}

		[TestMethod]
		public void QueryOneMachines()
		{
		}
	}
}
