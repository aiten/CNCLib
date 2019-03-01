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

using System.Reflection;

namespace Framework.UnitTest.Repository
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryTestFixtureBase<TDbContext> : IDisposable  where TDbContext : DbContext
    {
        public RepositoryTestFixtureBase()
        {
            ScriptDir = $"{ Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }\\Repository\\SQL\\";
        }

        public void InitializeDatabase()
        {
            Context = CreateDbContext();
            Task.Run(() => RestoreTestDb()).Wait();

            Context = null;
        }

        public string ScriptDir { get; set; }

        private TDbContext Context { get; set; }

        public void Dispose()
        {
            Context?.Dispose();
            Context = null;
        }

        public abstract TDbContext CreateDbContext();
        
        private async Task RestoreTestDb()
        {
            await DropAllTablesAsync();
            await CreateAllTablesAsync();
            await InsertDefaultDataAsync();
        }

        private async Task CreateAllTablesAsync()
        {
            await ExecSqlScriptAsync($"{ScriptDir}CreateAllTables.sql");
        }

        private async Task DropAllTablesAsync()
        {
            await ExecSqlScriptAsync($"{ScriptDir}DropOrDeleteAllTables.sql", "DROP TABLE");
        }

        private async Task InsertDefaultDataAsync()
        {
            await ExecSqlScriptAsync($"{ScriptDir}InsertDefaultData.sql");
        }

        private async Task ExecSqlScriptAsync(string filePath, string replace = null)
        {
            var sql = await File.ReadAllTextAsync(filePath);

            if (!string.IsNullOrEmpty(replace))
            {
                sql = sql.Replace(@"[REPLACE]", replace);
            }

            await ExecSqlAsync(sql);
        }

        private async Task ExecSqlAsync(string sql)
        {
            if (Context == null)
            {
                throw new NullReferenceException(@"DB context is null!");
            }

            await Context.Database.ExecuteSqlCommandAsync(sql);
        }
    }
}