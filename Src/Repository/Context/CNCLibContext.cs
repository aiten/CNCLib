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

using System.Linq;

using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Mappings;

using Framework.Dependency;
using Framework.Repository.Abstraction.Entities;
using Framework.Repository.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CNCLib.Repository.Context
{
    public class CNCLibContext : DbContext
    {
        public CNCLibContext(DbContextOptions<CNCLibContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Machine -------------------------------------
            // MachineCommand -------------------------------------
            // MachineInitCommand -------------------------------------

            modelBuilder.Entity<Machine>().Map();
            modelBuilder.Entity<MachineCommand>().Map();
            modelBuilder.Entity<MachineInitCommand>().Map();

            // Configuration -------------------------------------

            modelBuilder.Entity<Configuration>().Map();

            // Item -------------------------------------
            // ItemProperty -------------------------------------

            modelBuilder.Entity<Item>().Map();
            modelBuilder.Entity<ItemProperty>().Map();

            // User -------------------------------------

            modelBuilder.Entity<User>().Map();

            // -------------------------------------

            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // -------------------------------------

            modelBuilder.Entity<Log>().Map();

            base.OnModelCreating(modelBuilder);
        }

        protected void InitOrUpdateDatabase(bool isTest)
        {
            if (Set<Machine>().Any())
            {
                ModifyWrongData();
                SaveChanges();
            }
            else
            {
                new CNCLibDefaultData().CNCSeed(this, isTest);
                SaveChanges();
            }
        }

        private void ModifyWrongData()
        {
            // Contracts => Contract
            foreach (var item in Set<Item>().Where(i => i.ClassName == @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO"))
            {
                item.ClassName = @"CNCLib.Logic.Contract.DTO.LoadOptions,CNCLib.Logic.Contract.DTO";
            }

            // Contract => Abstraction
            foreach (var item in Set<Item>().Where(i => i.ClassName == @"CNCLib.Logic.Contract.DTO.LoadOptions,CNCLib.Logic.Contract.DTO"))
            {
                item.ClassName = @"CNCLib.Logic.Abstraction.DTO.LoadOptions,CNCLib.Logic.Abstraction.DTO";
            }
        }

        public static void InitializeDatabase2(bool dropDatabase, bool isTest)
        {
            using (var ctx = GlobalServiceCollection.Instance.Resolve<CNCLibContext>())
            {
                ctx.InitializeDatabase(dropDatabase, isTest);
            }
        } 

        public void InitializeDatabase(bool dropDatabase, bool isTest)
        {
            if (dropDatabase)
            {
                Database.EnsureDeleted();
            }

            Database.Migrate();

            InitOrUpdateDatabase(isTest);
        }
    }
}