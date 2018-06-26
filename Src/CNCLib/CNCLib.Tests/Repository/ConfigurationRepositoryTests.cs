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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Contracts.Entities;
using CNCLib.Repository.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System.Threading.Tasks;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using FluentAssertions;
using Framework.EF;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class ConfigurationRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public new static void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

		[TestMethod]
        public async Task GetEmptyConfiguration()
        {
            using (var ctx = new CNCLibContext())
            using (var uow = new UnitOfWork<CNCLibContext>(ctx))
            {
                var rep = new ConfigurationRepository(ctx);
                var entity = await rep.Get("Test","Test");
				entity.Should().BeNull();
			}
	    }

		[TestMethod]
		public async Task SaveConfiguration()
		{
		    using (var ctx = new CNCLibContext())
		    using (var uow = new UnitOfWork<CNCLibContext>(ctx))
            {
                var rep = new ConfigurationRepository(ctx);
                await rep.Save(new Configuration("Test", "TestNew1", "Content"));
				await uow.SaveChangesAsync();
			}
		}

        [TestMethod]
        public async Task SaveAndReadConfiguration()
        {
			await WriteConfiguration("Test", "TestNew2", "Content2");

            using (var ctx = new CNCLibContext())
            using (var uowread = new UnitOfWork<CNCLibContext>(ctx))
            {
                var repread = new ConfigurationRepository(ctx);
                var read = await repread.Get("Test", "TestNew2");
                read.Value.Should().Be("Content2");
            }
        }


        [TestMethod]
		public async Task SaveAndReadAndDeleteConfiguration()
		{
			await WriteConfiguration("Test", "TestNew3", "Content3");

		    using (var ctx = new CNCLibContext())
		    using (var uow = new UnitOfWork<CNCLibContext>(ctx))
            {
                var rep = new ConfigurationRepository(ctx);
				var read = await rep.Get("Test", "TestNew3");
				read.Value.Should().Be("Content3");

				await rep.Delete(read);

				await uow.SaveChangesAsync();

				var readagain = await rep.Get("Test", "TestNew3");
				readagain.Should().BeNull();
			}
		}

		[TestMethod]
		public async Task SaveExistingConfiguration()
		{
            await WriteConfiguration("Test", "TestNew4", "Content4");
			await WriteConfiguration("Test", "TestNew4", "Content5");

		    using (var ctx = new CNCLibContext())
		    using (var uow = new UnitOfWork<CNCLibContext>(ctx))
            {
                var rep = new ConfigurationRepository(ctx);
				var readagain = await rep.Get("Test", "TestNew4");
				readagain.Value.Should().Be("Content5");
			}
		}

        private static async Task WriteConfiguration(string module, string name, string content)
        {
            using (var ctx = new CNCLibContext())
            using (var uowwrite = new UnitOfWork<CNCLibContext>(ctx))
            {
                var repwrite = new ConfigurationRepository(ctx);
				await repwrite.Save(new Configuration(module,name,content));
				await uowwrite.SaveChangesAsync();
            }
        }
    }
}
