////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Contracts.Entities;
using System.Collections.Generic;
using System.Linq;
using CNCLib.Repository.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System.Threading.Tasks;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class UserRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public static new void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

        [TestMethod]
        public async Task QueryAllUsers()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var users = await rep.GetUsers();
				Assert.AreEqual(true, users.Length >= 2);
			}
	    }

		[TestMethod]
		public async Task QueryOneUserFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var users = await rep.GetUser(1);
				Assert.AreEqual(1, users.UserID);
			}
		}

        [TestMethod]
        public async Task QueryOneUserByNameFound()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var users = await rep.GetUser(1);
                Assert.AreEqual(1, users.UserID);

                var usersbyName = await rep.GetUser(users.UserName);
                Assert.AreEqual(users.UserID, usersbyName.UserID);
            }
        }

        [TestMethod]
		public async Task QueryOneUserNotFound()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var users = await rep.GetUser(1000);
				Assert.IsNull(users);
			}
		}

        [TestMethod]
        public async Task QueryOneUserByNameNotFound()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var users = await rep.GetUser("UserNotExist");
                Assert.IsNull(users);
            }
        }

        [TestMethod]
		public async Task AddOneUser()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                var user = CreateUser("AddOneUser");
				await rep.Store(user);
				await uow.Save();
				Assert.AreNotEqual(0, user.UserID);
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

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
				await rep.Store(user);
				await uow.Save();
                id = user.UserID;
                Assert.AreNotEqual(0, id);

                user.UserName = "UpdateOneUserAndRead#2";

				await rep.Store(user);
				await uow.Save();
            }

            var userread = await ReadUser(id);
            CompareUser(user, userread);
       }

        private static async Task<int> WriteUser(User user)
        {
            int id;

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
				await rep.Store(user);
				await uow.Save();
                id = user.UserID;
                Assert.AreNotEqual(0, id);
            }

            return id;
        }

        private static async Task<User> ReadUser(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IUserRepository>(uow))
            {
                return await rep.GetUser(id);
            }
        }


        private static User CreateUser(string name)
		{
            var user = new User()
            {
                UserName = name,
                UserPassword = name.Reverse().ToString()
			};
			return user;
		}

		private static void CompareUser(User user, User userread)
		{
			Assert.AreEqual(true, userread.CompareProperties(user));
 		}
	}
}
