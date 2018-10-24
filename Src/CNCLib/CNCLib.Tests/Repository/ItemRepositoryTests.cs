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
using CNCLib.Repository.Contracts;
using FluentAssertions;
using Framework.Dependency;

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
            return entity.ItemId;
        }

        protected override Item SetEntityKey(Item entity, int key)
        {
            entity.ItemId = key;
            return entity;
        }

        protected override bool CompareEntity(Item entity1, Item entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserId)
            //);
            return Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"ItemId" });
        }

        #endregion

        #region CRUD Test

        [TestMethod]
        public async Task GetAllTest()
        {
            var entities = await GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Count(i => i.Name == "laser cut 160mg paper").Should().Be(1);
            entities.Count(i => i.Name == "laser cut hole 130mg black").Should().Be(1);
        }

        [TestMethod]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.ItemId.Should().Be(1);
        }

        [TestMethod]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.ItemId.Should().Be(2);
        }

        [TestMethod]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [TestMethod]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(() => CreateItem(@"AddUpdateDeleteTest"), (entity) => entity.ClassName = "DummyClassUpdate");
        }

        [TestMethod]
        public async Task AddUpdateDeleteWithItemPropertiesTest()
        {
            await AddUpdateDelete(() => AddItemProperties(CreateItem(@"AddUpdateDeleteWithItemPropertiesTest")), (entity) =>
            {
                entity.ClassName = "DummyClassUpdate";
                entity.ItemProperties.Remove(entity.ItemProperties.First());
                entity.ItemProperties.Add(new ItemProperty()
                {
                    Name  = @"NewItemProperty",
                    Value = @"Hallo"
                });
            });
        }

        [TestMethod]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => CreateItem(@"AddRollbackTest"));
        }

        #endregion

        private static Item CreateItem(string name)
        {
            var e = new Item
            {
                Name           = name,
                ClassName      = "Dummy",
                ItemProperties = new List<ItemProperty>()
            };
            return e;
        }

        private static Item AddItemProperties(Item e)
        {
            e.ItemProperties = new List<ItemProperty>
            {
                new ItemProperty { Name = "Name1", Value = "Test1", Item = e },
                new ItemProperty { Name = "Name2", Value = "Test2", Item = e },
                new ItemProperty { Name = "Name3", Item  = e }
            };
            return e;
        }
    }
}