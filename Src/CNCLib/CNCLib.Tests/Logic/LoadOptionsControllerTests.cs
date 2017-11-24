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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Logic;
using System.Linq;
using NSubstitute;
using Framework.Tools.Dependency;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Client;
using System.Threading.Tasks;

namespace CNCLib.Tests.Logic
{
    [TestClass]
	public class LoadOptionsControllerTests : CNCUnitTest
	{
		private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
		{
			var rep = Substitute.For<TInterface>();
			Dependency.Container.RegisterInstance(rep);
			return rep;
		}

		[TestMethod]
		public async Task GetAllLoadOptions()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			rep.GetAll(typeof(LoadOptions)).Returns(new DynItem[1]
			{
				new DynItem { ItemID=1, Name="Entry1"  }
			});
			rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = await ctrl.GetAll();

			Assert.AreEqual(1, all.Count());
			Assert.AreEqual(1, all.FirstOrDefault().Id);
			Assert.AreEqual("Entry1", all.FirstOrDefault().SettingName);
			Assert.AreEqual("HA", all.FirstOrDefault().FileName);
		}

		[TestMethod]
		public async Task GetLoadOptions()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = await ctrl.Get(1);

			Assert.AreEqual(1, all.Id);
			Assert.AreEqual("Entry1", all.SettingName);
			Assert.AreEqual("HA", all.FileName);
		}

		[TestMethod]
		public async Task GetLoadOptionsNull()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = await ctrl.Get(2);

			Assert.IsNull(all);
		}


		[TestMethod]
		public async Task AddLoadOptions()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

			await ctrl.Add(opt);

			await rep.Received().Add(Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
		}

		[TestMethod]
		public async Task UpdateLoadOptions()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

			await ctrl.Update(opt);

			await rep.Received().Save(Arg.Is<int>(x => x == 1), Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
		}

		[TestMethod]
		public async Task DeleteLoadOptions()
		{
			var rep = CreateMock<IDynItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

			await ctrl.Delete(opt).ConfigureAwait(false);

			await rep.Received().Delete(Arg.Is<int>(x => x == 1));
		}
	}
}
