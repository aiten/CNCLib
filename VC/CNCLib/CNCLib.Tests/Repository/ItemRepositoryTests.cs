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
using CNCLib.Repository.Contracts.Entities;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;
using Framework.Tools;
using CNCLib.Repository.Contracts;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class ItemRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public static new void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

		[TestMethod]
		public void QueryNotFound()
		{
			using (var rep = RepositoryFactory.Create<IItemRepository>())
			{
				var item = rep.Get(1000);
				Assert.IsNull(item);
			}
		}

		[TestMethod]
		public void AddOne()
		{
			using (var rep = RepositoryFactory.Create<IItemRepository>())
			{
				var item = CreateItem("AddOne");
				int id = rep.Store(item);
				Assert.AreNotEqual(0, id);
			}
		}
		[TestMethod]
		public void AddOneWithValues()
		{
			using (var rep = RepositoryFactory.Create<IItemRepository>())
			{
				var item = CreateItem("AddOneWithValues");
                AddItemProperties(item);
				int id = rep.Store(item);
				Assert.AreNotEqual(0, id);
			}
		}
		[TestMethod]
		public void AddOneAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			{

				var item = CreateItem("AddOneAndRead");

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				var optread = repread.Get(id);

				Assert.AreEqual(0, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void AddOneWithValuesAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			{

				var item = CreateItem("AddOneMachineWithCommandsAndRead");
				int count = AddItemProperties(item);

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				var optread = repread.Get(id);

				Assert.AreEqual(count, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void UpdateOneAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			{

				var item = CreateItem("UpdateOneAndRead");

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				item.Name = "UpdateOneAndRead#2";

				repwrite.Store(item);

				var optread = repread.Get(id);

				Assert.AreEqual(0, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void UpdateOneNoCommandChangeAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			{
				var item = CreateItem("UpdateOneNoCommandChangeAndRead");
				int count = AddItemProperties(item);

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				item.Name = "UpdateOneNoCommandChangeAndRead#2";

				repwrite.Store(item);

				var optread = repread.Get(id);

				Assert.AreEqual(count, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void UpdateOneValuesChangeAndRead()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			{

				var item = CreateItem("UpdateOneValuesChangeAndRead");
				int count = AddItemProperties(item);

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				item.Name = "UpdateOneValuesChangeAndRead#2";
				item.ItemProperties.Add(new ItemProperty() { Name = "Name#1", Value = "New#1", ItemID = id });
				item.ItemProperties.Add(new ItemProperty() { Name = "Name#2", Value = "New#2", ItemID = id });
				item.ItemProperties.Remove(item.ItemProperties.Single(m => m.Value == "Test1"));
				item.ItemProperties.Single(m => m.Value == "Test2").Value = "Test2.Changed";

				int newcount = count + 2 - 1;

				repwrite.Store(item);

				var optread = repread.Get(id);

				Assert.AreEqual(newcount, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void DeleteWithProperties()
		{
			using (var repwrite = RepositoryFactory.Create<IItemRepository>())
			using (var repread = RepositoryFactory.Create<IItemRepository>())
			using (var repdelete = RepositoryFactory.Create<IItemRepository>())
			{

				var item = CreateItem("DeleteWithProperties");
				int count = AddItemProperties(item);

				int id = repwrite.Store(item);

				Assert.AreNotEqual(0, id);

				repdelete.Delete(item);

				var oprread = repread.Get(id);

				//Assert.AreEqual(newcount, machineread.MachineCommands.Count);
			}
		}


		private static Item CreateItem(string name)
		{
			var e = new Item()
			{
				Name = name,
                ClassName = "Dummy"
			};
			return e;
		}

		private static int AddItemProperties(Item e)
		{
			int count = 3;
			e.ItemProperties = new List<ItemProperty>();
            e.ItemProperties.Add(new ItemProperty() { Name = "Name1", Value = "Test1" });
            e.ItemProperties.Add(new ItemProperty() { Name = "Name2", Value = "Test2" });
            e.ItemProperties.Add(new ItemProperty() { Name = "Name3" });
            return count;
		}

		private static void CompareItem(Item e1, Item e2)
		{
			Assert.AreEqual(true, e1.CompareProperties(e2));
            Assert.AreEqual(e1.ItemProperties == null ? 0 : e1.ItemProperties.Count, e2.ItemProperties.Count);

			foreach (ItemProperty mc in e2.ItemProperties)
			{
				Assert.AreEqual(true, mc.CompareProperties(e1.ItemProperties.Single(m => m.Name == mc.Name)));
			}
		}
	}
}
