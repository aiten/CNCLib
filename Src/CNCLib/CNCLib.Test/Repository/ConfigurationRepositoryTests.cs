////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using FluentAssertions;

using Framework.Dependency;
using Framework.Repository;
using Framework.Test.Repository;
using Framework.Tools;

using Xunit;

namespace CNCLib.Test.Repository
{
    public class ConfigurationRepositoryTests : RepositoryTests<CNCLibContext>
    {
        #region crt and overrides

        protected CRUDRepositoryTests<CNCLibContext, Configuration, ConfigurationPrimary, IConfigurationRepository> CreateTestContext()
        {
            return new CRUDRepositoryTests<CNCLibContext, Configuration, ConfigurationPrimary, IConfigurationRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = new CNCLibContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new ConfigurationRepository(context, UserContext);
                    return new CRUDTestDbContext<CNCLibContext, Configuration, ConfigurationPrimary, IConfigurationRepository>(context, uow, rep);
                },
                GetEntityKey  = (entity) => new ConfigurationPrimary() { Group = entity.Group, Name = entity.Name },
                SetEntityKey  = (entity,    key) =>
                {
                    entity.Group = key.Group;
                    entity.Name = key.Name;
                },
                CompareEntity = (entity1, entity2) => CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new string[0])
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = (await CreateTestContext().GetAll()).OrderBy(cfg => cfg.Name);
            entities.Count().Should().BeGreaterThan(3);
            entities.ElementAt(0).Group.Should().Be("TestGroup");
            entities.ElementAt(0).Name.Should().Be("TestBool");
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await CreateTestContext().GetOK(new ConfigurationPrimary() { Group = "TestGroup", Name = "TestBool" });
            entity.Value.Should().Be(@"True");
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await CreateTestContext().GetTrackingOK(new ConfigurationPrimary() { Group = "TestGroup", Name = "TestDecimal" });
            entity.Value.Should().Be(@"1.2345");
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await CreateTestContext().GetNotExist(new ConfigurationPrimary() { Group = "NotExist", Name = "NotExist" });
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await CreateTestContext().AddUpdateDelete(() => CreateConfiguration("TestGroup", "TestName"), (entity) => entity.Value = "testValueModified");
        }

        [Fact]
        public async Task AddUpdateDeleteBulkTest()
        {
            await CreateTestContext().AddUpdateDeleteBulk(
                () => new[]
                {
                    CreateConfiguration(@"AddUpdateDeleteBulk", "Test1"), CreateConfiguration(@"AddUpdateDeleteBulk", "Test2"), CreateConfiguration(@"AddUpdateDeleteBulk", "Test3")
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
                () => new Configuration()
                {
                    Group = "TestGroup",
                    Name  = "TestName",
                    Type  = "string",
                    Value = "TestValue"
                });
        }

        [Fact]
        public async Task StoreTest()
        {
            await CreateTestContext().Store(
                () => new Configuration()
                {
                    Group = "TestGroup",
                    Name  = "TestName",
                    Type  = "string",
                    Value = "TestValue"
                },
                (entity) => entity.Value = "testValueModified");
        }

        private static Configuration CreateConfiguration(string group, string name)
        {
            return new Configuration() { Group = group, Name = name, Type = "string", Value = "TestValue" };
        }

        #endregion

        #region Additiona Tests

        [Fact]
        public async Task GetEmptyConfiguration()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var entity = await ctx.Repository.Get("Test", "Test");
                entity.Should().BeNull();
            }
        }

        [Fact]
        public async Task SaveConfiguration()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                await ctx.Repository.Store(new Configuration("Test", "TestNew1", "Content"));
                await ctx.UnitOfWork.SaveChangesAsync();
            }
        }

        #endregion
    }
}