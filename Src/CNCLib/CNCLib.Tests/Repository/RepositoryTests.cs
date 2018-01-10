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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using Framework.EF;
using System.Linq;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

namespace CNCLib.Tests.Repository
{
	[TestClass]
	public class RepositoryTests : CNCUnitTest
	{
		public TestContext TestContext { get; set; }
		static bool _init = false;

		[ClassInitialize]
		public static void ClassInit(TestContext testContext)
		{
			if (_init == false)
			{
                //drop and recreate the test Db everytime the tests are run. 
                string sdfdir = testContext.TestDeploymentDir;
                string sdfroot = System.IO.Path.GetPathRoot(sdfdir);
                var driveinfo = new System.IO.DriveInfo(sdfroot);

                if (driveinfo.DriveType == System.IO.DriveType.Network)
                {
                    // a sdf file doesn't work on network-drive 
                    sdfdir = System.IO.Path.GetTempPath();
                }

                AppDomain.CurrentDomain.SetData("DataDirectory", sdfdir);

                using (var uow = new UnitOfWork<CNCLibContext>())
				{
                    CNCLibContext x = uow.Context; // ref to get loaded
					Microsoft.EntityFrameworkCore.Database.SetInitializer<CNCLibContext>(new CNCLibInitializerTest());
					uow.InitializeDatabase();
                    Item o = uow.Context.Items.FirstOrDefault(i => i.ItemID == 0);
                    // force init
                }
                _init = true;
			}
		}

		[TestInitialize]
		public void Init()
		{
            Dependency.Container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            Dependency.Container.RegisterType<IMachineRepository, MachineRepository>();
            Dependency.Container.RegisterType<IItemRepository, ItemRepository>();
            Dependency.Container.RegisterType<IUserRepository, UserRepository>();
            Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();
        }
    }
}
