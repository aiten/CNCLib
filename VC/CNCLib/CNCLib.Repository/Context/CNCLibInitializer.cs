using CNCLib.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.Context
{
    
//    public class CNCLibInitializer : DropCreateDatabaseAlways<CNCLibContext>
    public class CNCLibInitializer : CreateDatabaseIfNotExists<CNCLibContext>
    {
        protected override void Seed(CNCLibContext context)
        {
			var CNCLib = new Machine
			{
				Name = "Proxxon MF70",
				ComPort = "com4",
				Axis = 5,
				SizeX = 130m,
				SizeY = 45m,
				SizeZ = 81m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 115200,
				BufferSize = 63,
				CommandToUpper = false,
				ProbeSizeZ = 25m,
				ProbeDist = 10m,
				ProbeDistUp = 3m,
				ProbeFeed = 100m,
				SDSupport = true,
				Spindle = true,
				Coolant = true,
				Rotate = true
			};

			var kk1000s = new Machine
			{
				Name = "KK1000S",
				ComPort = "com11",
				Axis = 3,
				SizeX = 830m,
				SizeY = 500m,
				SizeZ = 100m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 115200,
				BufferSize = 63,
				CommandToUpper = false,
				ProbeSizeZ = 25m,
				ProbeDist = 10m,
				ProbeDistUp = 3m,
				ProbeFeed = 100m,
                SDSupport = true,
				Spindle = true,
				Coolant = true,
                Rotate = true
			};

 			var machines = new List<Machine>
            {
				CNCLib,
				kk1000s
            };

			context.Machines.AddRange(machines);

			var machinecommands = new List<MachineCommand>
            {
                new MachineCommand{ Machine=CNCLib, CommandName = "SD Dir", CommandString ="m20" },
                new MachineCommand{ Machine=machines[0], CommandName = "Test", CommandString ="m21" },
                new MachineCommand{ Machine=kk1000s, CommandName = "SD Dir", CommandString ="m20" },
                new MachineCommand{ Machine=machines[1], CommandName = "Test", CommandString ="m211" }
			};

			context.MachineCommands.AddRange(machinecommands);
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
