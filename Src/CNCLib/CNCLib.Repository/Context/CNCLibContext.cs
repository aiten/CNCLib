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
using CNCLib.Repository.Contracts.Entities;
using CNCLib.Repository.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CNCLib.Repository.Context
{
	public class CNCLibContext : DbContext
	{
	    public static Action<DbContextOptionsBuilder> OnConfigure;

	    public CNCLibContext()
	    {

	    }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO Think about how to handle CRUDRepo correctly.
            //optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            OnConfigure?.Invoke(optionsBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Machine> Machines { get; set; }
		public DbSet<MachineCommand> MachineCommands { get; set; }
		public DbSet<MachineInitCommand> MachineInitCommands { get; set; }
		public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemProperty> ItemProperties { get; set; }

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

            base.OnModelCreating(modelBuilder);

        }
    }
}
