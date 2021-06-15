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

    [Collection("RepositoryTests")]
    public class ConfigurationRepositoryTests : RepositoryTests
    {
        #region crt and overrides

        public ConfigurationRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
        {
        }

        protected CrudRepositoryTests<CNCLibContext, ConfigurationEntity, int, IConfigurationRepository> CreateTestContext()
        {
            return new CrudRepositoryTests<CNCLibContext, ConfigurationEntity, int, IConfigurationRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = TestFixture.CreateDbContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new ConfigurationRepository(context);
                    return new CrudTestDbContext<CNCLibContext, ConfigurationEntity, int, IConfigurationRepository>(context, uow, rep);
                },
                GetEntityKey  = (entity) => entity.ConfigurationId,
                SetEntityKey  = (entity,  key) => entity.ConfigurationId = key,
                CompareEntity = (entity1, entity2) => entity1.ArePropertiesEqual(entity2, new[] { @"ConfigurationId", })
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = (await CreateTestContext().GetAll()).OrderBy(cfg => cfg.Name);
            entities.Should().HaveCountGreaterOrEqualTo(3);
            var entity = entities.First();
            entity.Group.Should().Be("TestGroup");
            entity.Name.Should().Be("TestBool");
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await CreateTestContext().GetOK(1);
            entity.ConfigurationId.Should().Be(1);
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await CreateTestContext().GetTrackingOK(1);
            entity.ConfigurationId.Should().Be(1);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await CreateTestContext().GetNotExist(123456);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await CreateTestContext().AddUpdateDelete(
                () => CreateConfiguration("TestGroup", "TestName"),
                (entity) => entity.Value = "testValueModified");
        }

        [Fact]
        public async Task AddUpdateDeleteBulkTest()
        {
            await CreateTestContext().AddUpdateDeleteBulk(
                () => new[]
                {
                    CreateConfiguration(@"AddUpdateDeleteBulk", "Test1"),
                    CreateConfiguration(@"AddUpdateDeleteBulk", "Test2"),
                    CreateConfiguration(@"AddUpdateDeleteBulk", "Test3")
                },
                (entities) =>
                {
                    int i = 0;
                    foreach (var entity in entities)
                    {
                        entity.Value = $"DummyNameValue{i++}";
                    }
                });
        }

        [Fact]
        public async Task AddRollbackTest()
        {
            await CreateTestContext().AddRollBack(
                () => new ConfigurationEntity()
                {
                    Group  = "TestGroup",
                    Name   = "TestName",
                    Type   = "string",
                    Value  = "TestValue",
                    UserId = 1
                });
        }

/*
        [Fact]
        public async Task StoreTest()
        {
            await CreateTestContext().Store(
                () => new ConfigurationEntity()
                {
                    Group  = "TestGroup",
                    Name   = "TestName",
                    Type   = "string",
                    Value  = "TestValue",
                    UserId = 1
                },
                (entity) => entity.Value = "testValueModified");
        }
*/
        private static ConfigurationEntity CreateConfiguration(string group, string name)
        {
            return new ConfigurationEntity() { Group = group, Name = name, Type = "string", Value = "TestValue", UserId = 1 };
        }

        #endregion

        #region Additiona Tests

        [Fact]
        public async Task GetEmptyConfiguration()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var entity = await ctx.Repository.Get(1, "Test", "Test");
                entity.Should().BeNull();
            }
        }

        [Fact]
        public async Task SaveConfiguration()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                await ctx.Repository.Store(new ConfigurationEntity(1, "Test", "TestNew1", "Content"));
                await ctx.UnitOfWork.SaveChangesAsync();
            }
        }

        #endregion
    }
}