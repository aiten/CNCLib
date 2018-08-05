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
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    [TestClass]
	public class UserRepositoryTests : CRUDRepositoryTests<User,int, IUserRepository>
	{
        #region crt and overrides

        protected override CRUDTestContext<User, int, IUserRepository> CreateCRUDTestContext()
	    {
	        return Dependency.Resolve<CRUDTestContext<User, int, IUserRepository>>();
	    }

	    [ClassInitialize]
	    public new static void ClassInit(TestContext testContext)
	    {
	        RepositoryTests.ClassInit(testContext);
	    }

        protected override int GetEntityKey(User entity)
	    {
	        return entity.UserID;
	    }
	    protected override User SetEntityKey(User entity, int key)
	    {
	        entity.UserID = key;
	        return entity;
	    }

        protected override bool CompareEntity(User entity1, User entity2)
	    {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserID)
            //);
            return Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] {@"UserID"});
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
            entity.UserID.Should().Be(1);
	    }

	    [TestMethod]
	    public async Task GetTrackingOKTest()
	    {
	        var entity = await GetTrackingOK(2);
	        entity.UserID.Should().Be(2);
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
	            () => new User() { UserName = "Hallo", UserPassword = "1234"}, 
	            (entity) => entity.UserPassword = "3456");
	    }

	    [TestMethod]
	    public async Task AddRollbackTest()
	    {
	        await AddRollBack(() => new User() {UserName = "Hallo", UserPassword = "1234"});
	    }

        #endregion

        #region Additional Tests

        [TestMethod]
	    public async Task QueryOneUserByNameFound()
	    {
	        using (var ctx = CreateCRUDTestContext())
	        {
	            var users = await ctx.Repository.Get(2);
	            users.UserID.Should().Be(2);

	            var usersbyName = await ctx.Repository.GetUser(users.UserName);
	            usersbyName.UserID.Should().Be(users.UserID);
	        }
	    }

	    [TestMethod]
	    public async Task QueryOneUserByNameNotFound()
	    {
	        using (var ctx = CreateCRUDTestContext())
	        {
	            var users = await ctx.Repository.GetUser("UserNotExist");
	            users.Should().BeNull();
	        }
	    }

	    [TestMethod]
	    [ExpectedException(typeof(DbUpdateException))]
        public async Task InserftDuplicateUserName()
	    {
	        string existingusername;

            using (var ctx = CreateCRUDTestContext())
	        {
	            existingusername = (await ctx.Repository.Get(2)).UserName;
	        }

	        using (var ctx = CreateCRUDTestContext())
	        {
	            User entityToAdd = new User() { UserName = existingusername };
	            ctx.Repository.Add(entityToAdd);

	            await ctx.UnitOfWork.SaveChangesAsync();
	        }
	    }

        #endregion
    }
}
