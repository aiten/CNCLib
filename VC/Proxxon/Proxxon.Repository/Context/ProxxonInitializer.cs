using Proxxon.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository.Context
{
    
//    public class ProxxonInitializer : DropCreateDatabaseAlways<ProxxonContext>
    public class ProxxonInitializer : CreateDatabaseIfNotExists<ProxxonContext>
    {
        protected override void Seed(ProxxonContext context)
        {

			context.Machines.AddRange(new List<Machine>
            {
                new Machine{ Name="Proxxon", ComPort = "com4",  SizeX = 130m, SizeY=45m, SizeZ=81m, BaudRate=115200, BufferSize=63, CommandToUpper=false, Default=false },
                new Machine{ Name="KK100S",  ComPort = "com11", SizeX = 830m, SizeY=500m, SizeZ=100m, BaudRate=115200, BufferSize=63, CommandToUpper=false, Default=true},
            });
/*
            context.Publishers.AddRange(new List<Publisher> { 
                new Publisher { Name="Puffin"},
                new Publisher { Name="Frobisher"}
            });

            var TheWaspFactory = new Book
            {
                Title="The Wasp Factory",
                Publisher = context.Publishers.Local.Single(p => p.Name == "Puffin")
            };

            TheWaspFactory.Authors.Add(context.Authors.Local.Single(a => a.Name=="Banks"));

            var TheDifferenceEngine = new Book
            {
                Title = "The Difference Engine",
                Publisher = context.Publishers.Local.Single(p => p.Name == "Puffin")
            };

            TheDifferenceEngine.Authors.Add(context.Authors.Local.Single(a => a.Name == "Banks"));
            TheDifferenceEngine.Authors.Add(context.Authors.Local.Single(a => a.Name == "Stirling"));

            context.Books.AddRange(new List<Book> { 
                TheWaspFactory,
                TheDifferenceEngine
            });
 */
        }
    }
}
