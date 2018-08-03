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

using System.Globalization;
using CNCLib.Repository.Contracts.Entities;

namespace CNCLib.Repository.Context
{
    public class CNCLibDefaultData
    {
        public void CNCSeed(CNCLibContext context, bool isTest)
        {
            var users = UserSeed(context);
            MachineSeed(context,users);
            ItemSeed(context);
            ConfigurationSeed(context, isTest);
        }

        private User[] UserSeed(CNCLibContext context)
        {
           var user1 = new User
            {
                UserName = "Herbert"
            };

            var user2 = new User
            {
                UserName = "Edith"
            };

            var users = new []
            {
                user1,
                user2
            };

            context.Users.AddRange(users);

            return users;
        }

        private void MachineSeed(CNCLibContext context, User[] users)
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
                DtrIsReset = true,
                BufferSize = 63,
                CommandToUpper = false,
                CommandSyntax = 1, // CommandSyntax.GCode
                ProbeSizeZ = 25m,
                ProbeDist = 10m,
                ProbeDistUp = 3m,
                ProbeFeed = 100m,
                SDSupport = true,
                Spindle = true,
                Coolant = true,
                Rotate = true,
                Laser = false,

                User = users[0]
            };

            var kk1000s = new Machine
            {
                Name = "KK1000S",
                ComPort = "com5",
                Axis = 3,
                SizeX = 800m,
                SizeY = 500m,
                SizeZ = 100m,
                SizeA = 360m,
                SizeB = 360m,
                SizeC = 360m,
                BaudRate = 115200,
                DtrIsReset = true,
                BufferSize = 63,
                CommandToUpper = false,
                CommandSyntax = 1, // CommandSyntax.GCode
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
                DtrIsReset = true,
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
                DtrIsReset = true,
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
                DtrIsReset = true,
                BufferSize = 63,
                CommandToUpper = false,
                CommandSyntax = 1, // CommandSyntax.GCode
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
                Name = "MiniCNC(remove)",
                SerialServer = "localhost",
                SerialServerPort = 5000,
				ComPort = "com6",
				Axis = 3,
				SizeX = 150m,
				SizeY = 105m,
				SizeZ = 30m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 250000,
                DtrIsReset = true,
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
                new MachineCommand{ Machine=minilaser, CommandName = "Square",      CommandString =@"m106 s2\ng0x0y0\ng0x36\ng0y36\ng0x0\ng0y0m107",    PosX=3, PosY=0 },

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
				new MachineCommand{ Machine=miniCNC, CommandName = "Spindle Off",  CommandString =@"m5",        PosX=1, PosY=1,      JoystickMessage=";btn1s" }
			};

            var machineinitcommands = new[]
            {
                new MachineInitCommand { Machine = minilaser, SeqNo=0, CommandString = @"g0 x2"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=1, CommandString = @"g28 x0"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=2, CommandString = @"g0 x36"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=3, CommandString = @"g0 x0"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=4, CommandString = @"g0 y2"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=5, CommandString = @"g28 y0"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=6, CommandString = @"g0 y36"  },
                new MachineInitCommand { Machine = minilaser, SeqNo=7, CommandString = @"g0 y0"  }
            };

            context.MachineCommands.AddRange(machinecommands);
            context.MachineInitCommands.AddRange(machineinitcommands);
        }

        private void ItemSeed(CNCLibContext context)
        {
            var cutItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"laser cut 160mg paper"
            };
            var cutHoleItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"laser cut hole 130mg black"
            };

            var graveItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"laser grave"
            };
            var graveIMGItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"laser grave image"
            };
            var graveIMGG00G01Item = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"laser grave image (G0G1)"
            };

            var graveMillItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"mill grave"
            };
            var cutMillItem = new Item
            {
                ClassName = @"CNCLib.Logic.Contracts.DTO.LoadOptions,CNCLib.Logic.Contracts.DTO",
                Name = @"mill cut"
            };

            var items = new[] { cutItem, cutHoleItem, graveItem, graveIMGItem, graveIMGG00G01Item, graveMillItem, cutMillItem };

            context.Items.AddRange(items);

            var itemproperties = new[]
            {
                //cut
                new ItemProperty { Item = cutItem, Name = @"SettingName",         Value=cutItem.Name    },
                new ItemProperty { Item = cutItem, Name = @"LaserFirstOnCommand", Value=@"M3 S255\ng4 P0.3"    },
                new ItemProperty { Item = cutItem, Name = @"LaserOnCommand",      Value=@"M3\ng4 P0.3"    },
                new ItemProperty { Item = cutItem, Name = @"LaserOffCommand",     Value=@"M5"    },
                new ItemProperty { Item = cutItem, Name = @"LaserLastOffCommand", Value=@"M5"    },
                new ItemProperty { Item = cutItem, Name = @"LaserSize",           Value=@"0.3"    },
                new ItemProperty { Item = cutItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty { Item = cutItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty { Item = cutItem, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty { Item = cutItem, Name = @"AutoScaleSizeY",      Value=@"150"    },
                new ItemProperty { Item = cutItem, Name = @"AutoScaleCenter",     Value=@"true"    },
                new ItemProperty { Item = cutItem, Name = @"ConvertType",         Value=@"InvertLineSequence"    },
                new ItemProperty { Item = cutItem, Name = @"MoveSpeed",           Value=@"450"    },
				new ItemProperty { Item = cutItem, Name = @"LoadType",            Value=@"HPGL"    },
                new ItemProperty { Item = cutItem, Name = @"FileName",            Value=@"Examples\Ghost.hpgl"    },


                //cut-image
                new ItemProperty { Item = cutHoleItem, Name = @"SettingName",         Value=cutHoleItem.Name    },
                new ItemProperty { Item = cutHoleItem, Name = @"LaserFirstOnCommand", Value=@"M3 S255\ng4 P0.25"    },
                new ItemProperty { Item = cutHoleItem, Name = @"LaserOnCommand",      Value=@"M3\ng4 P0.25"    },
                new ItemProperty { Item = cutHoleItem, Name = @"LaserOffCommand",     Value=@"M5"    },
                new ItemProperty { Item = cutHoleItem, Name = @"LaserLastOffCommand", Value=@"M5"    },
                new ItemProperty { Item = cutHoleItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty { Item = cutHoleItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty { Item = cutHoleItem, Name = @"AutoScaleSizeX",      Value=@"200"    },
                new ItemProperty { Item = cutHoleItem, Name = @"AutoScaleSizeY",      Value=@"290"    },
                new ItemProperty { Item = cutHoleItem, Name = @"MoveSpeed",           Value=@"500"    },
                new ItemProperty { Item = cutHoleItem, Name = @"ImageDPIX",           Value=@"12"    },
                new ItemProperty { Item = cutHoleItem, Name = @"ImageDPIY",           Value=@"12"    },
                new ItemProperty { Item = cutHoleItem, Name = @"DotDistX",            Value=@"0.7"    },
                new ItemProperty { Item = cutHoleItem, Name = @"DotDistY",            Value=@"0.7"    },
                new ItemProperty { Item = cutHoleItem, Name = @"HoleType",            Value=@"Diamond"    },
				new ItemProperty { Item = cutHoleItem, Name = @"LoadType",            Value=@"ImageHole"    },
                new ItemProperty { Item = cutHoleItem, Name = @"FileName",            Value=@"Examples\girl1.jpg"    },

                //grave laser
                new ItemProperty { Item = graveItem, Name = @"SettingName",         Value=graveItem.Name    },
                new ItemProperty { Item = graveItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty { Item = graveItem, Name = @"MoveSpeed",           Value=@"450"    },
				new ItemProperty { Item = graveItem, Name = @"LoadType",            Value=@"HPGL"    },
                new ItemProperty { Item = graveItem, Name = @"LaserFirstOnCommand", Value=@"M3 S255"    },
                new ItemProperty { Item = graveItem, Name = @"LaserOnCommand",      Value=@"M3"    },
                new ItemProperty { Item = graveItem, Name = @"LaserOffCommand",     Value=@"M5"    },
                new ItemProperty { Item = graveItem, Name = @"LaserLastOffCommand", Value=@"M5"    },
                new ItemProperty { Item = graveItem, Name = @"FileName",            Value=@"Examples\snoopy.plt"    },

                //grave Image
                new ItemProperty { Item = graveIMGItem, Name = @"SettingName",         Value=graveIMGItem.Name    },
                new ItemProperty { Item = graveIMGItem, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty { Item = graveIMGItem, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty { Item = graveIMGItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty { Item = graveIMGItem, Name = @"ImageDPIX",           Value=@"66.7"    },
                new ItemProperty { Item = graveIMGItem, Name = @"ImageDPIY",           Value=@"66.7"    },
                new ItemProperty { Item = graveIMGItem, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty { Item = graveIMGItem, Name = @"AutoScaleSizeY",      Value=@"150"    },
				new ItemProperty { Item = graveIMGItem, Name = @"LoadType",            Value=@"Image"    },
                new ItemProperty { Item = graveIMGItem, Name = @"FileName",            Value=@"Examples\girl2.png"    },

                //grave Image (G0G1)
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"SettingName",         Value=graveIMGG00G01Item.Name    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"LaserFirstOnCommand", Value=@"M3 S255"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"LaserOnCommand",      Value=@""    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"LaserOffCommand",      Value=@""    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"PenMoveType",         Value=@"CommandString"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"ImageDPIX",           Value=@"66.7"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"ImageDPIY",           Value=@"66.7"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"AutoScaleSizeY",      Value=@"150"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"LoadType",            Value=@"Image"    },
                new ItemProperty { Item = graveIMGG00G01Item, Name = @"FileName",            Value=@"Examples\girl2.png"    },

                //grave mill
                new ItemProperty { Item = graveMillItem, Name = @"SettingName",         Value=graveMillItem.Name    },
                new ItemProperty { Item = graveMillItem, Name = @"PenMoveType",         Value=@"ZMove"    },
                new ItemProperty { Item = graveMillItem, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty { Item = graveMillItem, Name = @"LoadType",            Value=@"HPGL"    },
                new ItemProperty { Item = graveMillItem, Name = @"EngravePosUp",        Value=@"1.5"    },
                new ItemProperty { Item = graveMillItem, Name = @"EngravePosDown",      Value=@"-0.75"    },
                new ItemProperty { Item = graveMillItem, Name = @"EngravePosInParameter",Value=@"false"    },
                new ItemProperty { Item = graveMillItem, Name = @"FileName",            Value=@"Examples\snoopy.plt"    },

                //cut mill
                new ItemProperty { Item = cutMillItem, Name = @"SettingName",         Value=cutMillItem.Name    },
                new ItemProperty { Item = cutMillItem, Name = @"LaserSize",           Value=@"1"    },
                new ItemProperty { Item = cutMillItem, Name = @"PenMoveType",         Value=@"ZMove"    },
                new ItemProperty { Item = cutMillItem, Name = @"AutoScale",           Value=@"true"    },
                new ItemProperty { Item = cutMillItem, Name = @"AutoScaleSizeX",      Value=@"150"    },
                new ItemProperty { Item = cutMillItem, Name = @"AutoScaleSizeY",      Value=@"150"    },
                new ItemProperty { Item = cutMillItem, Name = @"AutoScaleCenter",     Value=@"true"    },
                new ItemProperty { Item = cutMillItem, Name = @"ConvertType",         Value=@"InvertLineSequence"    },
                new ItemProperty { Item = cutMillItem, Name = @"MoveSpeed",           Value=@"450"    },
                new ItemProperty { Item = cutMillItem, Name = @"LoadType",            Value=@"HPGL"    },
                new ItemProperty { Item = cutMillItem, Name = @"EngravePosUp",        Value=@"1.5"    },
                new ItemProperty { Item = cutMillItem, Name = @"EngravePosDown",      Value=@"-4"    },
                new ItemProperty { Item = cutMillItem, Name = @"EngravePosInParameter",Value=@"false"    },
                new ItemProperty { Item = cutMillItem, Name = @"FileName",            Value=@"Examples\witch.hpgl"    }

             };

            context.ItemProperties.AddRange(itemproperties);
        }

        private void ConfigurationSeed(CNCLibContext context, bool isTest)
        {
            if (isTest)
            {
                var cfgs = new Configuration[]
                {
                    new Configuration() { Group = "TestGroup", Name = "TestInt", Type = typeof(int).FullName, Value = 1.ToString() },
                    new Configuration() { Group = "TestGroup", Name = "TestBool", Type = typeof(bool).FullName, Value = true.ToString() },
                    new Configuration() { Group = "TestGroup", Name = "TestString", Type = typeof(string).FullName, Value = "String" },
                    new Configuration() { Group = "TestGroup", Name = "TestDecimal", Type = typeof(decimal).FullName, Value = 1.2345m.ToString(CultureInfo.InvariantCulture) },
                };

                context.Configurations.AddRange(cfgs);
            }
        }
    }
}
