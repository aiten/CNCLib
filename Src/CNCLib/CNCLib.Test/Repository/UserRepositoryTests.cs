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

using System;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using FluentAssertions;

using Framework.Repository;
using Framework.Test.Repository;
using Framework.Tools;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace CNCLib.Test.Repository
{
    public class UserRepositoryTests : RepositoryTests<CNCLibContext>
    {
        #region crt and overrides

        protected CRUDRepositoryTests<CNCLibContext, User, int, IUserRepository> CreateTestContext()
        {
            return new CRUDRepositoryTests<CNCLibContext, User, int, IUserRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = new CNCLibContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new UserRepository(context);
                    return new CRUDTestDbContext<CNCLibContext, User, int, IUserRepository>(context, uow, rep);
                },
                GetEntityKey  = (entity) => entity.UserId,
                SetEntityKey  = (entity,    id) =>
                {
                    entity.UserId = id;
                    return entity;
                },
                CompareEntity = (entity1, entity2) => CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"UserId" })
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = (await CreateTestContext().GetAll()).OrderBy(u => u.UserName);
            entities.Count().Should().BeGreaterThan(1);
            entities.ElementAt(0).UserName.Should().Be("Edith");
            entities.ElementAt(1).UserName.Should().Be("Herbert");
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await CreateTestContext().GetOK(1);
            entity.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await CreateTestContext().GetTrackingOK(2);
            entity.UserId.Should().Be(2);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await CreateTestContext().GetNotExist(2342341);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await CreateTestContext().AddUpdateDelete(() => new User() { UserName = "Hallo", UserPassword = "1234" }, (entity) => entity.UserPassword = "3456");
        }

        [Fact]
        public async Task AddRollbackTest()
        {
            await CreateTestContext().AddRollBack(() => new User() { UserName = "Hallo", UserPassword = "1234" });
        }

        #endregion

        #region Additional Tests

        [Fact]
        public async Task QueryOneUserByNameFound()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var users = await ctx.Repository.Get(2);
                users.UserId.Should().Be(2);

                var usersByName = await ctx.Repository.GetByName(users.UserName);
                usersByName.UserId.Should().Be(users.UserId);
            }
        }

        [Fact]
        public async Task QueryOneUserByNameNotFound()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var users = await ctx.Repository.GetByName("UserNotExist");
                users.Should().BeNull();
            }
        }

        [Fact]
        public async Task InsertDuplicateUserName()
        {
            string existingUserName;

            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                existingUserName = (await ctx.Repository.Get(2)).UserName;
            }

            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var entityToAdd = new User() { UserName = existingUserName };
                ctx.Repository.Add(entityToAdd);

                //[SkippableFact(typeof(DbUpdateException))]

                Func<Task> act = async () => await ctx.UnitOfWork.SaveChangesAsync();

                //assert

                ;

                await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(act);
            }
        }

        #endregion
    }
}