////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using CNCLib.Repository.Contracts.Entities;
using CNCLib.Repository.Mappings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

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
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemProperty> ItemProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Machine -------------------------------------
            // MachineCommand -------------------------------------
            // MachineInitCommand -------------------------------------

            modelBuilder.Configurations.Add(new MachineMapping());
            modelBuilder.Configurations.Add(new MachineCommandMapping());
            modelBuilder.Configurations.Add(new MachineInitCommandMapping());

            // Configuration -------------------------------------

            modelBuilder.Configurations.Add(new ConfigurationMapping());

            // Item -------------------------------------
            // ItemProperty -------------------------------------

            modelBuilder.Configurations.Add(new ItemMapping());
            modelBuilder.Configurations.Add(new ItemPropertyMapping());

            // -------------------------------------

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // -------------------------------------

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
