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

using System.Linq;
using System.Threading.Tasks;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using FluentAssertions;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    [TestClass]
    public class ConfigurationRepositoryTests : CRUDRepositoryTests<Configuration, ConfigurationPrimary, IConfigurationRepository>
    {
        #region crt and overrides

        [ClassInitialize]
        public new static void ClassInit(TestContext testContext)
        {
            RepositoryTests.ClassInit(testContext);
        }

        protected override CRUDTestContext<Configuration, ConfigurationPrimary, IConfigurationRepository> CreateCRUDTestContext()
        {
            return Dependency.Resolve<CRUDTestContext<Configuration, ConfigurationPrimary, IConfigurationRepository>>();
        }

        protected override ConfigurationPrimary GetEntityKey(Configuration entity)
        {
            return new ConfigurationPrimary() { Group = entity.Group, Name = entity.Name };
        }
        protected override Configuration SetEntityKey(Configuration entity, ConfigurationPrimary key)
        {
            entity.Group = key.Group;
            entity.Name = key.Name;
            return entity;
        }

        protected override bool CompareEntity(Configuration entity1, Configuration entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserID)
            //);
            return Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new string[0]);
        }

        #endregion

        #region CRUD Test

        [TestMethod]
        public async Task GetAllTest()
        {
            var entities = (await GetAll()).OrderBy(cfg => cfg.Name);
            entities.Count().Should().BeGreaterThan(3);
            entities.ElementAt(0).Group.Should().Be("TestGroup");
            entities.ElementAt(0).Name.Should().Be("TestBool");
        }

        [TestMethod]
        public async Task GetOKTest()
        {
            var entity = await GetOK(new ConfigurationPrimary() { Group = "TestGroup", Name = "TestBool" });
            entity.Value.Should().Be(@"True");
        }

        [TestMethod]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(new ConfigurationPrimary() { Group = "TestGroup", Name = "TestDecimal" });
            entity.Value.Should().Be(@"1.2345");
        }

        [TestMethod]
        public async Task GetNotExistTest()
        {
            await GetNotExist(new ConfigurationPrimary() { Group = "NotExist", Name = "NotExist" });
        }

        [TestMethod]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(
                () => CreateConfiguration("TestGroup", "TestName"),
                (entity) => entity.Value = "testValueModified");
        }

        [TestMethod]
        public async Task AddUpdateDeleteBulkTest()
        {
            await AddUpdateDeleteBulk(
                () => new [] { CreateConfiguration(@"AddUpdateDeleteBulk","Test1"), CreateConfiguration(@"AddUpdateDeleteBulk", "Test2"), CreateConfiguration(@"AddUpdateDeleteBulk", "Test3") },
                (entities) => { int i = 0; foreach (var entity in entities) { entity.Value = $"DummyNameValue{i++}"; } });
        }

        [TestMethod]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => new Configuration() { Group = "TestGroup", Name = "TestName", Type = "string", Value = "TestValue" });
        }

        [TestMethod]
        public async Task StoreTest()
        {
            await Store(
                () => new Configuration() { Group = "TestGroup", Name = "TestName", Type = "string", Value = "TestValue" },
                (entity) => entity.Value = "testValueModified");
        }
        private static Configuration CreateConfiguration(string group, string name)
        {
            return new Configuration() { Group = group, Name = name, Type = "string", Value = "TestValue" };
        }

        #endregion

        #region Additiona Tests

        [TestMethod]
        public async Task GetEmptyConfiguration()
        {
            using (var ctx = CreateCRUDTestContext())
            {
                var entity = await ctx.Repository.Get("Test","Test");
				entity.Should().BeNull();
			}
	    }

		[TestMethod]
		public async Task SaveConfiguration()
		{
		    using (var ctx = CreateCRUDTestContext())
		    {
                await ctx.Repository.Store(new Configuration("Test", "TestNew1", "Content"));
				await ctx.UnitOfWork.SaveChangesAsync();
			}
		}

        #endregion
    }
}
