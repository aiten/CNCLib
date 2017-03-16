////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

namespace CNCLib.Repository.Context
{

    //    public class CNCLibInitializer : DropCreateDatabaseAlways<CNCLibContext>
    //public class CNCLibInitializer : CreateDatabaseIfNotExists<CNCLibContext>
    public static class CNCLibDefaultData
    {
        public static void CNCSeed(CNCLibContext context)
        {
            MachineSeed(context);
            ItemSeed(context);
        }

        private static void MachineSeed(CNCLibContext context)
        {
            var proxonMF70 = new Machine
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
                ComPort = "com5",
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
                ProbeSizeZ = 24.8m,
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
                ComPort = "com6",
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

            var minilaser = new Machine
            {
                Name = "MinLaser",
                ComPort = "com4",
                Axis = 2,
                SizeX = 36m,
                SizeY = 36m,
                SizeZ = 1m,
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

            var dck40laser = new Machine
            {
                Name = "DC-K40-Laser",
                ComPort = "com3",
                Axis = 2,
                SizeX = 320m,
                SizeY = 220m,
                SizeZ = 1m,
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
                SDSupport = false,
                Spindle = false,
                Coolant = false,
                Rotate = false,
                Laser = true
            };

			var miniCNC = new Machine
			{
				Name = "MiniCNC",
				ComPort = "com6",
				Axis = 3,
				SizeX = 150m,
				SizeY = 105m,
				SizeZ = 30m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 250000,
				BufferSize = 63,
				CommandToUpper = false,
				ProbeSizeZ = 1.5m,
				ProbeDist = 2m,
				ProbeDistUp = 1m,
				ProbeFeed = 100m,
				SDSupport = false,
				Spindle = true,
				Coolant = true,
				Rotate = false,
				Laser = false
			};


			var machines = new []
            {
                proxonMF70,
                kk1000s,
                laser,
                minilaser,
                dck40laser,
				miniCNC
            };

            context.Machines.AddRange(machines);

            var machinecommands = new []
            {
                //ProxxonMF70
                new MachineCommand{ Machine=proxonMF70, CommandName = "Set XY = 0",   CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0,PosY=0,	JoystickMessage=";btn4" },
				new MachineCommand{ Machine=proxonMF70, CommandName = "Set X = 0",    CommandString =@"g92 x0",    PosX=0, PosY=1,		JoystickMessage=";btn4s" },
                new MachineCommand{ Machine=proxonMF70, CommandName = "Set Y = 0",    CommandString =@"g92 y0",    PosX=0, PosY=2,		JoystickMessage=";btn4c" },
                new MachineCommand{ Machine=proxonMF70, CommandName = "Set Z = 0",    CommandString =@"g92 z0",    PosX=0, PosY=3,		JoystickMessage=";btn3" },
				new MachineCommand{ Machine=proxonMF70, CommandName = "Probe Z",	  CommandString =@";probe:z",  PosX=0, PosY=4,      JoystickMessage=";btn3s" },
				new MachineCommand{ Machine=proxonMF70, CommandName = "Spindle On",   CommandString =@"m3",        PosX=1, PosY=0,      JoystickMessage=";btn1" },
                new MachineCommand{ Machine=proxonMF70, CommandName = "Spindle Off",  CommandString =@"m5",        PosX=1, PosY=1,		JoystickMessage=";btn1s" },
                new MachineCommand{ Machine=proxonMF70, CommandName = "Coolant On",   CommandString =@"m7",        PosX=1, PosY=2,      JoystickMessage=";btn2" },
                new MachineCommand{ Machine=proxonMF70, CommandName = "Coolant Off",  CommandString =@"m9",        PosX=1, PosY=3,      JoystickMessage=";btn2s" },

                //kk1000s
                new MachineCommand{ Machine=kk1000s, CommandName = "Set XY = 0",    CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0, PosY=0 ,  JoystickMessage=";btn4" },
				new MachineCommand{ Machine=kk1000s, CommandName = "Set X = 0",		CommandString =@"g92 x0",    PosX=0, PosY=1,		JoystickMessage=";btn4s" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Set Y = 0",		CommandString =@"g92 y0",    PosX=0, PosY=2,		JoystickMessage=";btn4c" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Set Z = 0",		CommandString =@"g92 z0",    PosX=0, PosY=3,		JoystickMessage=";btn3" },
				new MachineCommand{ Machine=kk1000s, CommandName = "Probe Z",		CommandString = @";probe:z", PosX = 0, PosY = 4,	JoystickMessage=";btn3s" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Spindle On",	CommandString =@"m3",        PosX=1, PosY=0,		JoystickMessage=";btn1" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Spindle Off",	CommandString =@"m5",        PosX=1, PosY=1,		JoystickMessage=";btn1s" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Coolant On",	CommandString =@"m7",        PosX=1, PosY=2,		JoystickMessage=";btn2" },
                new MachineCommand{ Machine=kk1000s, CommandName = "Coolant Off",	CommandString =@"m9",        PosX=1, PosY=3,		JoystickMessage=";btn2s" },   
																																	   
                //laser
                new MachineCommand{ Machine=laser, CommandName = "Set XY = 0",  CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0, PosY=0,		JoystickMessage=";btn4" },
                new MachineCommand{ Machine=laser, CommandName = "Set X = 0",   CommandString =@"g92 x0",       PosX=0, PosY=1,			JoystickMessage=";btn4s" },
                new MachineCommand{ Machine=laser, CommandName = "Set Y = 0",   CommandString =@"g92 y0",       PosX=0, PosY=2,			JoystickMessage=";btn4c" },
                new MachineCommand{ Machine=laser, CommandName = "Laser Off",   CommandString =@"m107",         PosX=1, PosY=0,			JoystickMessage=";btn1" },
                new MachineCommand{ Machine=laser, CommandName = "Laser On",    CommandString =@"m106",         PosX=1, PosY=1,			JoystickMessage=";btn2:0" },
                new MachineCommand{ Machine=laser, CommandName = "Laser Min",   CommandString =@"m106 s1",      PosX=1, PosY=2,			JoystickMessage=";btn2:1" },
                new MachineCommand{ Machine=laser, CommandName = "Laser Max",   CommandString =@"m106 s255",    PosX=1, PosY=3,			JoystickMessage=";btn2:2" },

                //minlaser
                new MachineCommand{ Machine=minilaser, CommandName = "Set XY = 0",  CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0,PosY=0,	JoystickMessage=";btn4" },
                new MachineCommand{ Machine=minilaser, CommandName = "Set X = 0",   CommandString =@"g92 x0\n;beep",PosX=0, PosY=1,			JoystickMessage=";btn4s" },
                new MachineCommand{ Machine=minilaser, CommandName = "Set Y = 0",   CommandString =@"g92 y0\n;beep",PosX=0, PosY=2,			JoystickMessage=";btn4c" },
                new MachineCommand{ Machine=minilaser, CommandName = "Laser Off",   CommandString =@"m107",         PosX=1, PosY=0,			JoystickMessage=";btn1" },
                new MachineCommand{ Machine=minilaser, CommandName = "Laser On",    CommandString =@"m106",         PosX=1, PosY=1,			JoystickMessage=";btn2:0" },
                new MachineCommand{ Machine=minilaser, CommandName = "Laser Min",   CommandString =@"m106 s1",      PosX=1, PosY=2,			JoystickMessage=";btn2:1" },
                new MachineCommand{ Machine=minilaser, CommandName = "Laser Max",   CommandString =@"m106 s255",    PosX=1, PosY=3,			JoystickMessage=";btn2:2" },
                new MachineCommand{ Machine=minilaser, CommandName = "Sqare",       CommandString =@"m106 s2\ng0x0y0\ng0x36\ng0y36\ng0x0\ng0y0m107",    PosX=3, PosY=0 },

                //co2 laser
                new MachineCommand{ Machine=dck40laser, CommandName = "Set XY = 0",  CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0, PosY=0,      JoystickMessage=";btn4" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Set X = 0",   CommandString =@"g92 x0",       PosX=0, PosY=1,		JoystickMessage=";btn4s" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Set Y = 0",   CommandString =@"g92 y0",       PosX=0, PosY=2,		JoystickMessage=";btn4c" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Laser Off",   CommandString =@"m107",         PosX=1, PosY=0,		JoystickMessage=";btn1" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Laser On",    CommandString =@"m106",         PosX=1, PosY=1,		JoystickMessage=";btn2:0" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Laser Min",   CommandString =@"m106 s1",      PosX=1, PosY=2,		JoystickMessage=";btn2:1" },
                new MachineCommand{ Machine=dck40laser, CommandName = "Laser Max",   CommandString =@"m106 s255",    PosX=1, PosY=3,		JoystickMessage=";btn2:2" },

                //MiniCNC
                new MachineCommand{ Machine=miniCNC, CommandName = "Set XY = 0",   CommandString =@"g92 x0\ng92 y0\n;beep", PosX=0,PosY=0,   JoystickMessage=";btn4" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Set X = 0",    CommandString =@"g92 x0",    PosX=0, PosY=1,      JoystickMessage=";btn4s" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Set Y = 0",    CommandString =@"g92 y0",    PosX=0, PosY=2,      JoystickMessage=";btn4c" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Set Z = 0",    CommandString =@"g92 z0",    PosX=0, PosY=3,      JoystickMessage=";btn3" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Probe Z",      CommandString =@";probe:z",  PosX=0, PosY=4,      JoystickMessage=";btn3s" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Spindle On",   CommandString =@"m3",        PosX=1, PosY=0,      JoystickMessage=";btn1" },
				new MachineCommand{ Machine=miniCNC, CommandName = "Spindle Off",  CommandString =@"m5",        PosX=1, PosY=1,      JoystickMessage=";btn1s" },
			};

            var machineinitcommands = new[]
            {
                new MachineInitCommand { Machine = minilaser, SeqNo=0, CommandString = @"g0 x2"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=1, CommandString = @"g28 x0"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=2, CommandString = @"g0 x36"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=3, CommandString = @"g0 x0"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=4, CommandString = @"g0 y2"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=5, CommandString = @"g28 y0",  },
                new MachineInitCommand { Machine = minilaser, SeqNo=6, CommandString = @"g0 y36"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=7, CommandString = @"g0 y0"  }
            };

            context.MachineCommands.AddRange(machinecommands);
            context.MachineInitCommands.AddRange(machineinitcommands);
        }
        private static void ItemSeed(CNCLibContext context)
        {
            var cutItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"cut laser 160mg"
            };
            var cutHoleItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"cut laser hole 130mg black"
            };

            var graveItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"grave"
            };
            var graveIMGItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"grave image"
            };
            var graveIMGG00G01Item = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"grave image (G0G1)"
            };

            var items = new[] { cutItem, cutHoleItem, graveItem, graveIMGItem, graveIMGG00G01Item };

            context.Items.AddRange(items);

            var itemproperties = new[]
            {
                //cut
                new ItemProperty() { Item = cutItem, Name = @"SettingName",         Value=cutItem.Name    },
                new ItemProperty() { Item = cutItem, Name = @"LaserFirstOnCommand", Value=@"M106 S255\ng4 P0.3"    },
                new ItemProperty() { Item = cutItem, Name = @"LaserOnCommand",      Value=@"M106\ng4 P0.3"    },
                new ItemProperty() { Item = cutItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty() { Item = cutItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty() { Item = cutItem, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty() { Item = cutItem, Name = @"AutoScaleSizeY",      Value=@"150"    },
                new ItemProperty() { Item = cutItem, Name = @"MoveSpeed",           Value=@"450"    },
				new ItemProperty() { Item = cutItem, Name = @"LoadType",            Value=@"HPGL"    },

                //cut-image
                new ItemProperty() { Item = cutHoleItem, Name = @"SettingName",         Value=cutHoleItem.Name    },
                new ItemProperty() { Item = cutHoleItem, Name = @"LaserFirstOnCommand", Value=@"M106 S255\ng4 P0.25"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"LaserOnCommand",      Value=@"M106\ng4 P0.25"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"AutoScaleSizeX",      Value=@"200"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"AutoScaleSizeY",      Value=@"290"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"MoveSpeed",           Value=@"500"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"ImageDPIX",           Value=@"12"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"ImageDPIY",           Value=@"12"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"DotDistX",            Value=@"0.7"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"DotDistY",            Value=@"0.7"    },
                new ItemProperty() { Item = cutHoleItem, Name = @"HoleType",            Value=@"Diamond"    },
				new ItemProperty() { Item = cutHoleItem, Name = @"LoadType",            Value=@"ImageHole"    },

                //grave
                new ItemProperty() { Item = graveItem, Name = @"SettingName",         Value=graveItem.Name    },
                new ItemProperty() { Item = graveItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty() { Item = graveItem, Name = @"MoveSpeed",           Value=@"450"    },
				new ItemProperty() { Item = graveItem, Name = @"LoadType",            Value=@"HPGL"    },

                //grave Image
                new ItemProperty() { Item = graveIMGItem, Name = @"SettingName",         Value=graveIMGItem.Name    },
                new ItemProperty() { Item = graveIMGItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"ImageDPIX",           Value=@"66.7"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"ImageDPIY",           Value=@"66.7"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty() { Item = graveIMGItem, Name = @"AutoScaleSizeY",      Value=@"150"    },
				new ItemProperty() { Item = graveIMGItem, Name = @"LoadType",            Value=@"Image"    },

                //grave Image (G0G1)
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"SettingName",         Value=graveIMGG00G01Item.Name    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"LaserFirstOnCommand", Value=@"M106 S255"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"LaserOnCommand",      Value=@""    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"LaserOffCommand",      Value=@""    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"ImageDPIX",           Value=@"66.7"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"ImageDPIY",           Value=@"66.7"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"AutoScaleSizeY",      Value=@"150"    },
                new ItemProperty() { Item = graveIMGG00G01Item, Name = @"LoadType",            Value=@"Image"    }
            };

            context.ItemProperties.AddRange(itemproperties);
        }
    }
}
