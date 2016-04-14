using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.WebAPI;
using CNCLib.WebAPI.Controllers;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.WebAPI.Tests.Controllers
{
	[TestClass]
	public class MachineControllerTest
	{
		[TestMethod]
		public void Get()
		{
			// Arrange
			MachineController controller = new MachineController();

			// Act
			IEnumerable<Machine> result = controller.Get();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count());
			Assert.AreEqual("value1", result.ElementAt(0));
			Assert.AreEqual("value2", result.ElementAt(1));
		}

		[TestMethod]
		public void GetById()
		{
			// Arrange
			MachineController controller = new MachineController();

			// Act
			Machine result = controller.Get(5);

			// Assert
			Assert.AreEqual("value", result);
		}

		[TestMethod]
		public void Post()
		{
			// Arrange
			MachineController controller = new MachineController();

			// Act
			controller.Post("value");

			// Assert
		}

		[TestMethod]
		public void Put()
		{
			// Arrange
			MachineController controller = new MachineController();

			// Act
			controller.Put(5, "value");

			// Assert
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			MachineController controller = new MachineController();

			// Act
			controller.Delete(5);

			// Assert
		}
	}
}
