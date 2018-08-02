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
	public class UserRepository2Tests : CUDRepositoryTests<User,int, IUserRepository>
	{
		[ClassInitialize]
		public new static void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

	    [TestMethod]
	    public async Task GetOK()
	    {
	        using (var ctx = Dependency.Resolve<TestContext<User,int,IUserRepository>>())
	        {
	            await GetId(ctx.Repository,1, (entity) =>
	            {
	                entity.UserID.Should().Be(1);
                });
	        }
	    }
	}
}
