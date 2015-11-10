////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using CNCLib.Repository.Entities;
using System.Threading.Tasks;
using Framework.EF;
using System.Collections.Generic;
using System.Linq;
using Framework.Tools;
using CNCLib.Repository.Interface;

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
			using (var rep = new ConfigurationRepository())
			{
				var entity = rep.Get("Test","Test");
				Assert.AreEqual(null, entity);
			}
	    }

		[TestMethod]
		public void SaveConfiguration()
		{
			using (var rep = new ConfigurationRepository())
			{
				rep.Save(new Configuration("Test", "TestNew1", "Content"));
			}
		}

		[TestMethod]
		public void SaveAndReadConfiguration()
		{
			using (var rep = new ConfigurationRepository())
			{
				rep.Save(new Configuration("Test", "TestNew2", "Content2"));
				var read = rep.Get("Test", "TestNew2");
				Assert.AreEqual("Content2", read.Value);
			}
		}

		[TestMethod]
		public void SaveAndReadAndDeleteConfiguration()
		{
			using (var rep = new ConfigurationRepository())
			{
				rep.Save(new Configuration("Test", "TestNew3", "Content2"));
				var read = rep.Get("Test", "TestNew3");
				Assert.AreEqual("Content2", read.Value);

				rep.Delete(read);

				var readagain = rep.Get("Test", "TestNew3");
				Assert.AreEqual(null, readagain);
			}
		}

		[TestMethod]
		public void SaveExistingConfiguration()
		{
			using (var rep = new ConfigurationRepository())
			{
				rep.Save(new Configuration("Test", "TestNew4", "Content4"));
				var read = rep.Get("Test", "TestNew4");
				Assert.AreEqual("Content4", read.Value);

				rep.Save(new Configuration("Test", "TestNew4", "Content5"));

				var readagain = rep.Get("Test", "TestNew4");
				Assert.AreEqual("Content5", readagain.Value);
			}
		}
	}
}
