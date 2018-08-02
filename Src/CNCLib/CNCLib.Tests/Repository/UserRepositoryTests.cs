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
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using FluentAssertions;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    [TestClass]
	public class UserRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public new static void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

        [TestMethod]
        public async Task QueryAllUsers()
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var users = await rep.GetUsers();
				users.Length.Should().BeGreaterOrEqualTo(2);
			}
	    }

		[TestMethod]
		public async Task QueryOneUserFound()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var users = await rep.GetUser(1);
				users.UserID.Should().Be(1);
			}
		}

        [TestMethod]
        public async Task QueryOneUserByNameFound()
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var users = await rep.GetUser(1);
                users.UserID.Should().Be(1);

                var usersbyName = await rep.GetUser(users.UserName);
                usersbyName.UserID.Should().Be(users.UserID);
            }
        }

        [TestMethod]
		public async Task QueryOneUserNotFound()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var users = await rep.GetUser(1000);
				users.Should().BeNull();
			}
		}

        [TestMethod]
        public async Task QueryOneUserByNameNotFound()
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var users = await rep.GetUser("UserNotExist");
                users.Should().BeNull();
            }
        }

        [TestMethod]
		public async Task AddOneUser()
		{
		    using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                var user = CreateUser("AddOneUser");
				await rep.Store(user);
				await uow.SaveChangesAsync();
				user.UserID.Should().NotBe(0);
			}
		}

		[TestMethod]
		public async Task AddOneUserAndRead()
        {
            var user = CreateUser("AddOneUserAndRead");
            int id = await WriteUser(user);

            var userread = await ReadUser(id);

            CompareUser(user, userread);
        }

       [TestMethod]
       public async Task UpdateOneUserAndRead()
       {
            var user = CreateUser("UpdateOneUserAndRead");
            int id;

           using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                await rep.Store(user);
				await uow.SaveChangesAsync();
                id = user.UserID;
                id.Should().NotBe(0);

                user.UserName = "UpdateOneUserAndRead#2";

				await rep.Store(user);
				await uow.SaveChangesAsync();
            }

            var userread = await ReadUser(id);
            CompareUser(user, userread);
       }

        private static async Task<int> WriteUser(User user)
        {
            int id;

            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                await rep.Store(user);
				await uow.SaveChangesAsync();
                id = user.UserID;
                id.Should().NotBe(0);
            }

            return id;
        }

        private static async Task<User> ReadUser(int id)
        {
            using (var ctx = new CNCLibContext())
            {
                var uow = new UnitOfWork<CNCLibContext>(ctx);
                var rep = new UserRepository(ctx);
                return await rep.GetUser(id);
            }
        }


        private static User CreateUser(string name)
		{
            var user = new User
            {
                UserName = name,
                UserPassword = name.Reverse().ToString()
			};
			return user;
		}

		private static void CompareUser(User user, User userread)
		{
			userread.CompareProperties(user).Should().Be(true);
 		}
	}
}
