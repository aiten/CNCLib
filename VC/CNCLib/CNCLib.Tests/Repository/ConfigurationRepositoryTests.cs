////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class ConfigurationRepositoryTests : RepositoryTests
	{
		[ClassInitialize]
		public static new void ClassInit(TestContext testContext)
		{
			RepositoryTests.ClassInit(testContext);
		}

		[TestMethod]
        public void GetEmptyConfiguration()
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
            {
                var entity = rep.Get("Test","Test");
				Assert.AreEqual(null, entity);
			}
	    }

		[TestMethod]
		public void SaveConfiguration()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
            {
                rep.Save(new Configuration("Test", "TestNew1", "Content"));
                uow.Save();
			}
		}

        [TestMethod]
        public void SaveAndReadConfiguration()
        {
            WriteConfiguration("Test", "TestNew2", "Content2");

            using (var uowread = Dependency.Resolve<IUnitOfWork>())
            using (var repread = Dependency.ResolveRepository<IConfigurationRepository>(uowread))
            {
                var read = repread.Get("Test", "TestNew2");
                Assert.AreEqual("Content2", read.Value);
            }
        }


        [TestMethod]
		public void SaveAndReadAndDeleteConfiguration()
		{
            WriteConfiguration("Test", "TestNew3", "Content3");

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
            {
				var read = rep.Get("Test", "TestNew3");
				Assert.AreEqual("Content3", read.Value);

				rep.Delete(read);

                uow.Save();

				var readagain = rep.Get("Test", "TestNew3");
				Assert.AreEqual(null, readagain);
			}
		}

		[TestMethod]
		public void SaveExistingConfiguration()
		{
            WriteConfiguration("Test", "TestNew4", "Content4");
            WriteConfiguration("Test", "TestNew4", "Content5");

            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
            {
				var readagain = rep.Get("Test", "TestNew4");
				Assert.AreEqual("Content5", readagain.Value);
			}
		}

        private static void WriteConfiguration(string module, string name, string content)
        {
            using (var uowwrite = Dependency.Resolve<IUnitOfWork>())
            using (var repwrite = Dependency.ResolveRepository<IConfigurationRepository>(uowwrite))
            {
                repwrite.Save(new Configuration(module,name,content));
                uowwrite.Save();
            }
        }
    }
}
