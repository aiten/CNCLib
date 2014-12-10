using Proxxon.Repository.Entities;
using Proxxon.Repository.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository.Context
{
    public class ProxxonContext : DbContext
    {
        public ProxxonContext()
        {
            Configure();
        }

        public ProxxonContext(string connectionString):base(connectionString)
        {
            Configure();    
        }

        public DbSet<Machine> Machines { get; set; }
//  public DbSet<Book> Books { get; set; }
//	public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MachineMapping()); 
            base.OnModelCreating(modelBuilder);
        }

        private void Configure()
        {
            System.Data.Entity.Database.SetInitializer<ProxxonContext>(new ProxxonInitializer());
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.ValidateOnSaveEnabled = true; 
        }
    }
}
