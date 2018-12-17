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

using Framework.Dependency;
using Framework.Repository.Abstraction;
using Framework.Test;
using Framework.Test.Repository;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace CNCLib.Test.Repository
{
    [Collection("RepositoryTests")]
    public abstract class RepositoryTests<TDbContext> : UnitTestBase
        where TDbContext : DbContext
    {
        static bool _init = false;

        public RepositoryTests()
        {
            ClassInitBase();
        }

        public static void ClassInitBase()
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

        private   ICNCLibUserContext _userContext = new CNCLibUserContext();
        protected ICNCLibUserContext UserContext => _userContext;
    }
}