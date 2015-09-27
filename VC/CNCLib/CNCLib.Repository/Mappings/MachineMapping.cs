using CNCLib.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.Mappings
{
    public class MachineMapping : EntityTypeConfiguration<Machine>
    {
		public MachineMapping()
        {
/*
            this.HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .Map(x => {
                    x.MapLeftKey("BookId");
                    x.MapRightKey("AuthorId");
                    x.ToTable("AuthorBooks");
                });

            this.HasRequired(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .WillCascadeOnDelete(false);
 */ 
        }
    }
}
