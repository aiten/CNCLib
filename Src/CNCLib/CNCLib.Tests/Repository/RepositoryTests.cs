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

using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Shared;
using Framework.Contracts.Repository;
using Framework.Repository;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    [TestClass]
    public class RepositoryTests : CNCUnitTest
    {
        public TestContext TestContext { get; set; }
        static bool        _init = false;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            if (_init == false)
            {
                //drop and recreate the test Db everytime the tests are run. 
                string dbdir     = testContext.TestDeploymentDir;
                string pathRoot  = System.IO.Path.GetPathRoot(dbdir);
                var    driveinfo = new System.IO.DriveInfo(pathRoot);

                if (driveinfo.DriveType == System.IO.DriveType.Network)
                {
                    // a db file doesn't work on network-drive 
                    dbdir = System.IO.Path.GetTempPath();
                }

                string dbfile = $@"{dbdir}\CNCLibTest.db";
                CNCLib.Repository.SqLite.MigrationCNCLibContext.InitializeDatabase(dbfile, true, true);

                _init = true;
            }
        }

        [TestInitialize]
        public void Init()
        {
            Dependency.Container.ResetContainer();
            Dependency.Container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            Dependency.Container.RegisterType<IMachineRepository, MachineRepository>();
            Dependency.Container.RegisterType<IItemRepository, ItemRepository>();
            Dependency.Container.RegisterType<IUserRepository, UserRepository>();

            Dependency.Container.RegisterTypeScoped<ICNCLibUserContext, CNCLibUserContext>();

            Dependency.Container.RegisterTypeScoped<CNCLibContext, CNCLibContext>();
            Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<CNCLibContext>>();

            Dependency.Container.RegisterType(typeof(CRUDTestContext<,,>), typeof(CRUDTestContext<,,>));
        }
    }
}