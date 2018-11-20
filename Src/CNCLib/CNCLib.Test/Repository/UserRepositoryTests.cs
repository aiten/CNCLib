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

using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entity;

using FluentAssertions;

using Framework.Dependency;
using Framework.Test.Repository;
using Framework.Tools;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Test.Repository
{
    [TestClass]
    public class UserRepositoryTests : RepositoryTests<CNCLibContext, User, int, IUserRepository>
    {
        #region crt and overrides

        protected override GetTestDbContext<CNCLibContext, User, int, IUserRepository> CreateTestDbContext()
        {
            return Dependency.Resolve<GetTestDbContext<CNCLibContext, User, int, IUserRepository>>();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            ClassInitBase(testContext);
        }

        protected override int GetEntityKey(User entity)
        {
            return entity.UserId;
        }

        protected override User SetEntityKey(User entity, int key)
        {
            entity.UserId = key;
            return entity;
        }

        protected override bool CompareEntity(User entity1, User entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserId)
            //);
            return CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"UserId" });
        }

        #endregion

        #region CRUD Test

        [TestMethod]
        public async Task GetAllTest()
        {
            var entities = (await GetAll()).OrderBy(u => u.UserName);
            entities.Count().Should().BeGreaterThan(1);
            entities.ElementAt(0).UserName.Should().Be("Edith");
            entities.ElementAt(1).UserName.Should().Be("Herbert");
        }

        [TestMethod]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.UserId.Should().Be(1);
        }

        [TestMethod]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.UserId.Should().Be(2);
        }

        [TestMethod]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [TestMethod]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(() => new User() { UserName = "Hallo", UserPassword = "1234" }, (entity) => entity.UserPassword = "3456");
        }

        [TestMethod]
        public async Task AddRollbackTest()
        {
            await AddRollBack(() => new User() { UserName = "Hallo", UserPassword = "1234" });
        }

        #endregion

        #region Additional Tests

        [TestMethod]
        public async Task QueryOneUserByNameFound()
        {
            using (var ctx = CreateTestDbContext())
            {
                var users = await ctx.Repository.Get(2);
                users.UserId.Should().Be(2);

                var usersByName = await ctx.Repository.GetByName(users.UserName);
                usersByName.UserId.Should().Be(users.UserId);
            }
        }

        [TestMethod]
        public async Task QueryOneUserByNameNotFound()
        {
            using (var ctx = CreateTestDbContext())
            {
                var users = await ctx.Repository.GetByName("UserNotExist");
                users.Should().BeNull();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public async Task InsertDuplicateUserName()
        {
            string existingUserName;

            using (var ctx = CreateTestDbContext())
            {
                existingUserName = (await ctx.Repository.Get(2)).UserName;
            }

            using (var ctx = CreateTestDbContext())
            {
                var entityToAdd = new User() { UserName = existingUserName };
                ctx.Repository.Add(entityToAdd);

                await ctx.UnitOfWork.SaveChangesAsync();
            }
        }

        #endregion
    }
}