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

    public class UserFileRepositoryTests : RepositoryTests
    {
        #region crt and overrides

        public UserFileRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
        {
        }

        protected CrudRepositoryTests<CNCLibContext, UserFileEntity, int, IUserFileRepository> CreateTestContext()
        {
            return new CrudRepositoryTests<CNCLibContext, UserFileEntity, int, IUserFileRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = TestFixture.CreateDbContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new UserFileRepository(context);
                    return new CrudTestDbContext<CNCLibContext, UserFileEntity, int, IUserFileRepository>(context, uow, rep);
                },
                GetEntityKey  = (entity) => entity.UserFileId,
                SetEntityKey  = (entity,  id) => entity.UserFileId = id,
                CompareEntity = (entity1, entity2) => entity1.ArePropertiesEqual(entity2, new[] { @"UserFileId" })
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = (await CreateTestContext().GetAll()).OrderBy(u => u.FileName).ToList();
            entities.Should().HaveCountGreaterThan(0);
            entities.Should().Contain(f => f.FileName == @"Examples\cat.hpgl");
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
            var entity = await CreateTestContext().GetTrackingOK(1);
            entity.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await CreateTestContext().GetNotExist(2342341);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await CreateTestContext().AddUpdateDelete(() => new UserFileEntity() { FileName = "Hallo", UserId = 1, Content = new byte[] { 10 } }, (entity) => entity.FileName = "Hallo2");
        }

        [Fact]
        public async Task AddRollbackTest()
        {
            await CreateTestContext().AddRollBack(() => new UserFileEntity() { FileName = "Hallo", UserId = 1 });
        }

        #endregion

        #region Additional Tests

        [Fact]
        public async Task QueryTotalFileSize()
        {
            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var totalFileSize = (await ctx.Repository.GetTotalUserFileSize(1));
                totalFileSize.Should().BeGreaterThan(1);
            }
        }

        [Fact]
        public async Task InsertDuplicateUserName()
        {
            string existingUserName;

            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                existingUserName = (await ctx.Repository.Get(1)).FileName;
            }

            using (var ctx = CreateTestContext().CreateTestDbContext())
            {
                var entityToAdd = new UserFileEntity() { FileName = existingUserName, UserId = 1, Content = new byte[] { 1 } };
                ctx.Repository.Add(entityToAdd);

                //[SkippableFact(typeof(DbUpdateException))]

                Func<Task> act = async () => await ctx.UnitOfWork.SaveChangesAsync();

                //assert

                await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(act);
            }
        }

        #endregion
    }
}