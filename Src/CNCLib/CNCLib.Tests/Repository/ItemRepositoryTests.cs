////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Threading.Tasks;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using FluentAssertions;
using Framework.EF;
using Framework.Tools.Dependency;

namespace CNCLib.Tests.Repository
{
    [TestClass]
    public class ItemRepositoryTests : CRUDRepositoryTests<Item, int, IItemRepository>
	{
        #region crt and overrides

        protected override CRUDTestContext<Item, int, IItemRepository> CreateCRUDTestContext()
        {
            return Dependency.Resolve<CRUDTestContext<Item, int, IItemRepository>>();
        }

        [ClassInitialize]
        public new static void ClassInit(TestContext testContext)
        {
            RepositoryTests.ClassInit(testContext);
        }

        protected override int GetEntityKey(Item entity)
        {
            return entity.ItemID;
        }
        protected override Item SetEntityKey(Item entity, int key)
        {
            entity.ItemID = key;
            return entity;
        }

        protected override bool CompareEntity(Item entity1, Item entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserID)
            //);
            return Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"ItemID" });
        }

        #endregion

        #region CRUD Test

        [TestMethod]
        public async Task GetAllTest()
        {
            var entities = await GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Where(i => i.Name == "laser cut 160mg paper").Count().Should().Be(1);
            entities.Where(i => i.Name == "laser cut hole 130mg black").Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.ItemID.Should().Be(1);
        }

        [TestMethod]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.ItemID.Should().Be(2);
        }

        [TestMethod]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [TestMethod]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(
                () => CreateItem(@"AddUpdateDeleteTest"),
                (entity) => entity.ClassName = "DummyClassUpdate");
        }

	    [TestMethod]
	    public async Task AddUpdateDeleteWithItemPropertiesTest()
	    {
	        await AddUpdateDelete(
	            () => AddItemProperties(CreateItem(@"AddUpdateDeleteWithItemPropertiesTest")),
	            (entity) => entity.ClassName = "DummyClassUpdate");
	    }

        [TestMethod]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => CreateItem(@"AddRollbackTest"));
        }

        #endregion

		[TestMethod]
		public async Task AddOne()
		{
            var item = CreateItem("AddOne");

		    using (var ctx = new CNCLibContext())
			{
			    var uow = new UnitOfWork<CNCLibContext>(ctx);
			    var rep = new ItemRepository(ctx);
                await rep.Store(item);
				await uow.SaveChangesAsync();
				item.ItemID.Should().NotBe(0);
			}
		}

		[TestMethod]
		public async Task AddOneWithValues()
		{
            var item = CreateItem("AddOneWithValues");
            AddItemProperties(item);

		    using (var ctx = new CNCLibContext())
			{
			    var uow = new UnitOfWork<CNCLibContext>(ctx);
			    var rep = new ItemRepository(ctx);
                await rep.Store(item);
				await uow.SaveChangesAsync();
            }
            item.ItemID.Should().NotBe(0);
        }

        [TestMethod]
        public async Task AddOneAndRead()
        {
            var item = CreateItem("AddOneAndRead");
            int id = await WriteItem(item);

            using (var ctx = new CNCLibContext())
            {
                var uowread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                var optread = await repread.Get(id);
                optread.ItemProperties.Count.Should().Be(0);
                CompareItem(item, optread);
            }
        }

        private static async Task<int> WriteItem(Item item)
        {
            int id;
            using (var ctx = new CNCLibContext())
            {
                var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
                var repwrite = new ItemRepository(ctx);
                await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();
                id = item.ItemID;
                id.Should().NotBe(0);
            }

            return id;
        }

        [TestMethod]
		public async Task AddOneWithValuesAndRead()
		{
            var item = CreateItem("AddOneMachineWithCommandsAndRead");
            int count = AddItemProperties(item).ItemProperties.Count();
            int id = await WriteItem(item);

		    using (var ctx = new CNCLibContext())
		    {
		        var uowread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                id.Should().NotBe(0);
                var optread = await repread.Get(id);
                optread.ItemProperties.Count.Should().Be(count);
                CompareItem(item, optread);
            }
		}

		[TestMethod]
		public async Task UpdateOneAndRead()
		{
            var item = CreateItem("UpdateOneAndRead");
            int id;

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
                var repwrite = new ItemRepository(ctx);
                await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();

                id = item.ItemID;
                id.Should().NotBe(0);

                item.Name = "UpdateOneAndRead#2";
				await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();
            }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                var optread = await repread.Get(id);
                optread.ItemProperties.Count.Should().Be(0);
                CompareItem(item, optread);
			}
		}

		[TestMethod]
		public async Task UpdateOneNoCommandChangeAndRead()
		{
            int id;
            var item = CreateItem("UpdateOneNoCommandChangeAndRead");
            int count = AddItemProperties(item).ItemProperties.Count();

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
                var repwrite = new ItemRepository(ctx);
                await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();

                id = item.ItemID;
                id.Should().NotBe(0);

                item.Name = "UpdateOneNoCommandChangeAndRead#2";
				await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();
            }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                var optread = await repread.Get(id);
				optread.ItemProperties.Count.Should().Be(count);
				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public async Task UpdateOneValuesChangeAndRead()
		{
            int id;
            var item = CreateItem("UpdateOneValuesChangeAndRead");
            int count = AddItemProperties(item).ItemProperties.Count(); 
            int newcount;

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
		        var repwrite = new ItemRepository(ctx);
                await repwrite.Store(item);
		        await uowwrite.SaveChangesAsync();

		        id = item.ItemID;
		        id.Should().NotBe(0);
		    }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
                var repwrite = new ItemRepository(ctx);
                item.Name = "UpdateOneValuesChangeAndRead#2";
                item.ItemProperties.Add(new ItemProperty { Name = "Name#1", Value = "New#1", ItemID = id });
                item.ItemProperties.Add(new ItemProperty { Name = "Name#2", Value = "New#2", ItemID = id });
                item.ItemProperties.Remove(item.ItemProperties.Single(m => m.Value == "Test1"));
                item.ItemProperties.Single(m => m.Value == "Test2").Value = "Test2.Changed";

                newcount = count + 2 - 1;

				await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();
            }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                var optread = await repread.Get(id);

				optread.ItemProperties.Count.Should().Be(newcount);

				CompareItem(item, optread);
			}
		}

		[TestMethod]
		public async Task DeleteWithProperties()
		{
            var item = CreateItem("DeleteWithProperties");
            int count = AddItemProperties(item).ItemProperties.Count(); 
            int id;

		    using (var ctx = new CNCLibContext())
		    {
		        var uowwrite = new UnitOfWork<CNCLibContext>(ctx);
                var repwrite = new ItemRepository(ctx);
                await repwrite.Store(item);
				await uowwrite.SaveChangesAsync();

                id = item.ItemID;
                id.Should().NotBe(0);
            }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowdelete = new UnitOfWork<CNCLibContext>(ctx);
                var repdelete = new ItemRepository(ctx);
                repdelete.Delete(item);
				await uowdelete.SaveChangesAsync();
            }

		    using (var ctx = new CNCLibContext())
		    {
		        var uowread = new UnitOfWork<CNCLibContext>(ctx);
                var repread = new ItemRepository(ctx);
                var itemread = await repread.Get(id);
                itemread.Should().BeNull();
            }
        }


		private static Item CreateItem(string name)
		{
			var e = new Item
			{
				Name = name,
                ClassName = "Dummy",
                ItemProperties = new List<ItemProperty>()
			};
			return e;
		}

		private static Item AddItemProperties(Item e)
		{
			int count = 3;
		    e.ItemProperties = new List<ItemProperty>
		    {
		        new ItemProperty {Name = "Name1", Value = "Test1", Item = e },
		        new ItemProperty {Name = "Name2", Value = "Test2", Item = e},
		        new ItemProperty {Name = "Name3", Item = e}
		    };
		    return e;
		}

		private static void CompareItem(Item e1, Item e2)
		{
			e1.CompareProperties(e2).Should().Be(true);
            (e1.ItemProperties?.Count ?? 0).Should().Be(e2.ItemProperties.Count);

			foreach (ItemProperty mc in e2.ItemProperties)
			{
				mc.CompareProperties(e1.ItemProperties.Single(m => m.Name == mc.Name)).Should().Be(true);
			}
		}
	}
}
