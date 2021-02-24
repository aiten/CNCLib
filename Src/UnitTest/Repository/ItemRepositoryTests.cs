/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.UnitTest.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CNCLib.Repository;
    using CNCLib.Repository.Abstraction;
    using CNCLib.Repository.Abstraction.Entities;
    using CNCLib.Repository.Context;

    using FluentAssertions;

    using Framework.Repository;
    using Framework.Tools;
    using Framework.UnitTest.Repository;

    using Xunit;

    public class ItemRepositoryTests : RepositoryTests
    {
        #region crt and overrides

        public ItemRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
        {
        }

        protected CrudRepositoryTests<CNCLibContext, ItemEntity, int, IItemRepository> CreateTestContext()
        {
            return new CrudRepositoryTests<CNCLibContext, ItemEntity, int, IItemRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = TestFixture.CreateDbContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new ItemRepository(context);
                    return new CrudTestDbContext<CNCLibContext, ItemEntity, int, IItemRepository>(context, uow, rep);
                },
                GetEntityKey = (entity) => entity.ItemId,
                SetEntityKey = (entity, key) =>
                {
                    entity.ItemId = key;
                    foreach (var itemProp in entity.ItemProperties)
                    {
                        itemProp.ItemId = key;
                    }
                },
                CompareEntity = (entity1, entity2) => CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"ItemId", @"User" })
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = await CreateTestContext().GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Count(i => i.Name == "laser cut 160mg paper").Should().Be(1);
            entities.Count(i => i.Name == "laser cut hole 130mg black").Should().Be(1);
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await CreateTestContext().GetOK(1);
            entity.ItemId.Should().Be(1);
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await CreateTestContext().GetTrackingOK(2);
            entity.ItemId.Should().Be(2);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await CreateTestContext().GetNotExist(2342341);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await CreateTestContext().AddUpdateDelete(() => CreateItem(@"AddUpdateDeleteTest"), (entity) => entity.ClassName = "DummyClassUpdate");
        }

        [Fact]
        public async Task AddUpdateDeleteWithItemPropertiesTest()
        {
            await CreateTestContext().AddUpdateDelete(
                () => AddItemProperties(CreateItem(@"AddUpdateDeleteWithItemPropertiesTest")),
                (entity) =>
                {
                    entity.ClassName = "DummyClassUpdate";
                    entity.ItemProperties.Remove(entity.ItemProperties.First());
                    entity.ItemProperties.First().Value = "NewValue";
                    entity.ItemProperties.Add(
                        new ItemPropertyEntity()
                        {
                            Name  = @"NewItemProperty",
                            Value = @"Hallo"
                        });
                });
        }

        [Fact]
        public async Task AddRollbackTest()
        {
            await CreateTestContext().AddRollBack(() => CreateItem(@"AddRollbackTest"));
        }

        #endregion

        private static ItemEntity CreateItem(string name)
        {
            var e = new ItemEntity
            {
                Name           = name,
                ClassName      = "Dummy",
                ItemProperties = new List<ItemPropertyEntity>(),
                UserId         = 1
            };
            return e;
        }

        private static ItemEntity AddItemProperties(ItemEntity e)
        {
            e.ItemProperties = new List<ItemPropertyEntity>
            {
                new ItemPropertyEntity { Name = "Name1", Value = "Test1", Item = e },
                new ItemPropertyEntity { Name = "Name2", Value = "Test2", Item = e },
                new ItemPropertyEntity { Name = "Name3", Item  = e }
            };
            return e;
        }
    }
}