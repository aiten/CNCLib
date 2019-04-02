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

using System;
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

namespace CNCLib.UnitTest.Repository
{
    public class UserRepositoryTests : RepositoryTests, IClassFixture<RepositoryTestFixture>
    {
        #region crt and overrides

        public UserRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
        {
        }

        protected CRUDRepositoryTests<CNCLibContext, User, int, IUserRepository> CreateTestContext()
        {
            return new CRUDRepositoryTests<CNCLibContext, User, int, IUserRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = TestFixture.CreateDbContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new UserRepository(context);
                    return new CRUDTestDbContext<CNCLibContext, User, int, IUserRepository>(context, uow, rep);
                },
                GetEntityKey  = (entity) => entity.UserId,
                SetEntityKey  = (entity,  id) => entity.UserId = id,
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