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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Contracts.Entities;
using System.Collections.Generic;
using System.Linq;
using CNCLib.Repository.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

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
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var item = rep.Get(1000);
				Assert.IsNull(item);
			}
		}

		[TestMethod]
		public void AddOne()
		{
            var item = CreateItem("AddOne");

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				rep.Store(item);
                uow.Save();
				Assert.AreNotEqual(0, (int)item.ItemID);
			}
		}

		[TestMethod]
		public void AddOneWithValues()
		{
            var item = CreateItem("AddOneWithValues");
            AddItemProperties(item);

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				rep.Store(item);
                uow.Save();
            }
            Assert.AreNotEqual(0, (int)item.ItemID);
        }

        [TestMethod]
        public void AddOneAndRead()
        {
            var item = CreateItem("AddOneAndRead");
            int id = WriteItem(item);

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                var optread = repread.Get(id);
                Assert.AreEqual(0, optread.ItemProperties.Count);
                CompareItem(item, optread);
            }
        }

        private static int WriteItem(Item item)
        {
            int id;
            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IItemRepository>(uowwrite))
            {
                repwrite.Store(item);
                uowwrite.Save();
                id = item.ItemID;
                Assert.AreNotEqual(0, id);
            }

            return id;
        }

        [TestMethod]
		public void AddOneWithValuesAndRead()
		{
            var item = CreateItem("AddOneMachineWithCommandsAndRead");
            int count = AddItemProperties(item);
            int id = WriteItem(item);

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                Assert.AreNotEqual(0, id);
                var optread = repread.Get(id);
                Assert.AreEqual(count, optread.ItemProperties.Count);
                CompareItem(item, optread);
            }
		}

		[TestMethod]
		public void UpdateOneAndRead()
		{
            var item = CreateItem("UpdateOneAndRead");
            int id;

            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IItemRepository>(uowwrite))
            {
                repwrite.Store(item);
                uowwrite.Save();

                id = item.ItemID;
                Assert.AreNotEqual(0, id);

                item.Name = "UpdateOneAndRead#2";
                repwrite.Store(item);
                uowwrite.Save();
            }

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                var optread = repread.Get(id);
                Assert.AreEqual(0, optread.ItemProperties.Count);
                CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void UpdateOneNoCommandChangeAndRead()
		{
            int id;
            var item = CreateItem("UpdateOneNoCommandChangeAndRead");
            int count = AddItemProperties(item);

            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IItemRepository>(uowwrite))
            {
                repwrite.Store(item);
                uowwrite.Save();

                id = item.ItemID;
                Assert.AreNotEqual(0, id);

                item.Name = "UpdateOneNoCommandChangeAndRead#2";
                repwrite.Store(item);
                uowwrite.Save();
            }

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                var optread = repread.Get(id);
				Assert.AreEqual(count, optread.ItemProperties.Count);
				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void UpdateOneValuesChangeAndRead()
		{
            int id;
            var item = CreateItem("UpdateOneValuesChangeAndRead");
            int count = AddItemProperties(item);
            int newcount;

            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IItemRepository>(uowwrite))
            {
                repwrite.Store(item);
                uowwrite.Save();

                id = item.ItemID;
                Assert.AreNotEqual(0, id);


                item.Name = "UpdateOneValuesChangeAndRead#2";
                item.ItemProperties.Add(new ItemProperty() { Name = "Name#1", Value = "New#1", ItemID = id });
                item.ItemProperties.Add(new ItemProperty() { Name = "Name#2", Value = "New#2", ItemID = id });
                item.ItemProperties.Remove(item.ItemProperties.Single(m => m.Value == "Test1"));
                item.ItemProperties.Single(m => m.Value == "Test2").Value = "Test2.Changed";

                newcount = count + 2 - 1;

                repwrite.Store(item);
                uowwrite.Save();
            }

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                var optread = repread.Get(id);

				Assert.AreEqual(newcount, optread.ItemProperties.Count);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public void DeleteWithProperties()
		{
            var item = CreateItem("DeleteWithProperties");
            int count = AddItemProperties(item);
            int id;

            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IItemRepository>(uowwrite))
            {
                repwrite.Store(item);
                uowwrite.Save();

                id = item.ItemID;
                Assert.AreNotEqual(0, id);
            }

            using (var uowdelete = Dependency.Resolve<IUnitOfWork>())
            using (var repdelete = Dependency.ResolveRepository<IItemRepository>(uowdelete))
            {
                repdelete.Delete(item);
                uowdelete.Save();
            }

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IItemRepository>(uowread))
            {
                var itemread = repread.Get(id);
                Assert.IsNull(itemread);
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
