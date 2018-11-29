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

using Microsoft.EntityFrameworkCore;

using CNCLib.Repository.Context;

namespace CNCLib.Repository.SqLite
{
    public class MigrationCNCLibContext : CNCLibContext
    {
        public static string DatabaseFile { get; set; } = $"{System.IO.Path.GetTempPath()}\\CNCLib.db";

        static MigrationCNCLibContext()
        {
            OnConfigure = (optionsBuilder) => { optionsBuilder.UseSqlite($"Data Source={DatabaseFile}"); };
        }

        public static void InitializeDatabase(string databaseFile, bool dropDatabase, bool isTest)
        {
            DatabaseFile = databaseFile;

            using (var ctx = new MigrationCNCLibContext())
            {
                if (dropDatabase)
                {
                    ctx.Database.EnsureDeleted();
                }

                ctx.Database.Migrate();

                ctx.InitOrUpdateDatabase(isTest);
            }
        }
    }
}