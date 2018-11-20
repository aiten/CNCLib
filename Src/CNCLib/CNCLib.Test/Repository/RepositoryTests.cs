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
using CNCLib.Repository.Contract;
using CNCLib.Shared;

using Framework.Contract.Repository;
using Framework.Dependency;
using Framework.Test.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Test.Repository
{
    [TestClass]
    public abstract class RepositoryTests<TDbContext, TEntity, TKey, TIRepository> : CRUDRepositoryTests<TDbContext, TEntity, TKey, TIRepository> where TEntity : class where TIRepository : ICRUDRepository<TEntity, TKey> where TDbContext : DbContext
    {
        public TestContext TestContext { get; set; }
        static bool        _init = false;

        [ClassInitialize]
        public static void ClassInitBase(TestContext testContext)
        {
            if (_init == false)
            {
                //drop and recreate the test Db every time the tests are run. 
 //               string dbDir     = testContext.TestDeploymentDir;
                string dbDir     = System.IO.Path.GetTempPath();
                string pathRoot  = System.IO.Path.GetPathRoot(dbDir);
                var    driveInfo = new System.IO.DriveInfo(pathRoot);

                if (driveInfo.DriveType == System.IO.DriveType.Network)
                {
                    // a db file doesn't work on network-drive 
                    dbDir = System.IO.Path.GetTempPath();
                }

                string dbFile = $@"{dbDir}\CNCLibTest.db";
                CNCLib.Repository.SqLite.MigrationCNCLibContext.InitializeDatabase(dbFile, true, true);

                _init = true;
            }
        }

        protected override void InitializeDependencies()
        {
            base.InitializeDependencies();

            Dependency.Container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            Dependency.Container.RegisterType<IMachineRepository, MachineRepository>();
            Dependency.Container.RegisterType<IItemRepository, ItemRepository>();
            Dependency.Container.RegisterType<IUserRepository, UserRepository>();

            Dependency.Container.RegisterTypeScoped<ICNCLibUserContext, CNCLibUserContext>();
        }
    }
}