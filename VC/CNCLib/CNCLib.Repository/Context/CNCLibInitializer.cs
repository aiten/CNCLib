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

using CNCLib.Repository.Contracts.Entities;
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
				Rotate = true,
                Laser = false
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
                Rotate = true,
                Laser = false
            };

            var laser = new Machine
            {
                Name = "Laser",
                ComPort = "com13",
                Axis = 2,
                SizeX = 400m,
                SizeY = 380m,
                SizeZ = 100m,
                SizeA = 360m,
                SizeB = 360m,
                SizeC = 360m,
                BaudRate = 250000,
                BufferSize = 63,
                CommandToUpper = false,
                ProbeSizeZ = 25m,
                ProbeDist = 10m,
                ProbeDistUp = 3m,
                ProbeFeed = 100m,
                SDSupport = false,
                Spindle = false,
                Coolant = false,
                Rotate = false,
                Laser = true
            };


            var machines = new List<Machine>
            {
				CNCLib,
				kk1000s,
                laser
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
