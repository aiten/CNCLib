/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using CNCLib.Repository.Context;
using CNCLib.Repository.SqLite;

using Framework.UnitTest.Repository;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace CNCLib.UnitTest.Repository
{
    public class RepositoryTestFixture : RepositoryTestFixtureBase<CNCLibContext>
    {
        public RepositoryTestFixture()
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
            SqliteDatabaseTools.DatabaseFile = dbFile;

            var dbContext = CreateDbContext();

            dbContext.Database.EnsureDeleted();
            dbContext.InitializeDatabase();

            new TestDataImporter(dbContext).Import();
            dbContext.SaveChanges();
        }

        public override CNCLibContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CNCLibContext>();
            SqliteDatabaseTools.OptionBuilder(optionsBuilder);
            return new CNCLibContext(optionsBuilder.Options);
        }
    }
}