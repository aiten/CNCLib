////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

using CNCLib.Repository.Entities;
using CNCLib.Repository.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.Context
{
    public class CNCLibContext : DbContext
    {
        public CNCLibContext()
        {
            Configure();
        }

        public CNCLibContext(string connectionString):base(connectionString)
        {
            Configure();    
        }

        public DbSet<Machine> Machines { get; set; }
		public DbSet<MachineCommand> MachineCommands { get; set; }
		public DbSet<MachineInitCommand> MachineInitCommands { get; set; }
		public DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MachineMapping());

			// Machine -------------------------------------

			modelBuilder.Entity<Machine>()
				.HasKey(m => m.MachineID);
			
			modelBuilder.Entity<Machine>().Property((m) => m.Name).
                IsRequired().
                HasMaxLength(64);

            modelBuilder.Entity<Machine>().Property((m) => m.ComPort).
                IsRequired().
                HasMaxLength(32);

			modelBuilder.Entity<Machine>().Property((m) => m.Axis).IsRequired();
			
			modelBuilder.Entity<Machine>().Property((m) => m.SizeX).IsRequired();
			modelBuilder.Entity<Machine>().Property((m) => m.SizeY).IsRequired();
			modelBuilder.Entity<Machine>().Property((m) => m.SizeZ).IsRequired();

			// MachineCommand -------------------------------------

			modelBuilder.Entity<MachineCommand>()
				.HasKey(mc => mc.MachineCommandID);

			modelBuilder.Entity<MachineCommand>().Property((m) => m.CommandString).
				IsRequired().
				HasMaxLength(64);
			modelBuilder.Entity<MachineCommand>().Property((m) => m.CommandName).
				IsRequired().
				HasMaxLength(64);

			// MachineInitCommand -------------------------------------

			modelBuilder.Entity<MachineInitCommand>()
				.HasKey(mc => mc.MachineInitCommandID);

			modelBuilder.Entity<MachineInitCommand>().Property((m) => m.CommandString).
				IsRequired().
				HasMaxLength(64);

			// Configuration -------------------------------------

			modelBuilder.Entity<Configuration>()
				.HasKey(m => new { m.Group, m.Name });

			modelBuilder.Entity<Configuration>().Property((m) => m.Group).
				IsRequired().
				HasMaxLength(256);

			modelBuilder.Entity<Configuration>().Property((m) => m.Name).
				IsRequired().
				HasMaxLength(256);

			modelBuilder.Entity<Configuration>().Property((m) => m.Type).
				IsRequired().
				HasMaxLength(256);

			modelBuilder.Entity<Configuration>().Property((m) => m.Value).
				HasMaxLength(4000);

			// -------------------------------------
			
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
           
            base.OnModelCreating(modelBuilder);

        }

        private void Configure()
        {
            System.Data.Entity.Database.SetInitializer<CNCLibContext>(new CNCLibInitializer());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true; 
        }
    }
}
