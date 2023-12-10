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

namespace CNCLib.Repository.Context;

using System;
using System.Linq;

using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Mappings;

using Framework.Dependency;
using Framework.Repository.Abstraction.Entities;
using Framework.Repository.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CNCLibContext : DbContext
{
    public CNCLibContext(DbContextOptions<CNCLibContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MachineEntity -------------------------------------
        // MachineCommandEntity -------------------------------------
        // MachineInitCommandEntity -------------------------------------

        modelBuilder.Entity<MachineEntity>().Map();
        modelBuilder.Entity<MachineCommandEntity>().Map();
        modelBuilder.Entity<MachineInitCommandEntity>().Map();

        // ConfigurationEntity -------------------------------------

        modelBuilder.Entity<ConfigurationEntity>().Map();

        // ItemEntity -------------------------------------
        // ItemPropertyEntity -------------------------------------

        modelBuilder.Entity<ItemEntity>().Map();
        modelBuilder.Entity<ItemPropertyEntity>().Map();

        // UserEntity -------------------------------------

        modelBuilder.Entity<UserEntity>().Map();
        modelBuilder.Entity<UserFileEntity>().Map();

        // -------------------------------------

        // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        // -------------------------------------

        modelBuilder.Entity<Log>().Map();

        base.OnModelCreating(modelBuilder);
    }

    protected void InitOrUpdateDatabase()
    {
        if (Set<MachineEntity>().Any())
        {
            ModifyWrongData();
            SaveChanges();
        }
        else
        {
            new CNCLibDefaultData(this).Import();
            SaveChanges();
        }
    }

    private void ModifyWrongData()
    {
        // Contracts => Contract
        foreach (var item in Set<ItemEntity>().Where(i => i.ClassName == @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO"))
        {
            item.ClassName = @"CNCLib.Logic.Contract.DTO.LoadOptions,CNCLib.Logic.Contract.DTO";
        }

        // Contract => Abstraction
        foreach (var item in Set<ItemEntity>().Where(i => i.ClassName == @"CNCLib.Logic.Contract.DTO.LoadOptions,CNCLib.Logic.Contract.DTO"))
        {
            item.ClassName = @"CNCLib.Logic.Abstraction.DTO.LoadOptions,CNCLib.Logic.Abstraction.DTO";
        }
    }

    public static void InitializeDatabase(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<CNCLibContext>();
            ctx.InitializeDatabase();
        }
    }

    public void InitializeDatabase()
    {
        Database.Migrate();
        InitOrUpdateDatabase();
    }
}