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
using CNCLib.Repository.Contracts;
using CNCLib.Logic;
using System.Linq;
using NSubstitute;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.EF;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.Tests.Logic
{
	[TestClass]
	public class LoadOptionsControllerTests : CNCUnitTest
	{
		private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
		{
			TInterface rep = Substitute.For<TInterface>();
			Dependency.Container.RegisterInstance(rep);
			return rep;
		}

		[TestMethod]
		public void GetAllLoadOptions()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			rep.GetAll(typeof(LoadOptions)).Returns(new Item[1]
			{
				new Item() { ItemID=1, Name="Entry1"  }
			});
			rep.Create(1).Returns(new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = ctrl.GetAll();

			Assert.AreEqual(1, all.Count());
			Assert.AreEqual(1, all.FirstOrDefault().Id);
			Assert.AreEqual("Entry1", all.FirstOrDefault().SettingName);
			Assert.AreEqual("HA", all.FirstOrDefault().FileName);
		}

		[TestMethod]
		public void GetLoadOptions()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			rep.Create(1).Returns(new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = ctrl.Get(1);

			Assert.AreEqual(1, all.Id);
			Assert.AreEqual("Entry1", all.SettingName);
			Assert.AreEqual("HA", all.FileName);
		}

		[TestMethod]
		public void GetLoadOptionsNull()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			rep.Create(1).Returns(new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" });

			var all = ctrl.Get(2);

			Assert.IsNull(all);
		}


		[TestMethod]
		public void AddLoadOptions()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" };

			ctrl.Add(opt);

			rep.Received().Add(Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
		}

		[TestMethod]
		public void UpdateLoadOptions()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" };

			ctrl.Update(opt);

			rep.Received().Save(Arg.Is<int>(x => x == 1), Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
		}

		[TestMethod]
		public void DeleteLoadOptions()
		{
			var rep = CreateMock<IItemController>();
			var ctrl = new LoadOptionsController();

			var opt = new LoadOptions() { SettingName = "Entry1", Id = 1, FileName = "HA" };

			ctrl.Delete(opt);

			rep.Received().Delete(Arg.Is<int>(x => x == 1));
		}
	}
}
