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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Logic.Interfaces;
using CNCLib.Logic;
using Framework.EF;
using Framework.Wpf;
using Framework.Logic;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Framework.Tools.Pattern;
using Framework.Wpf.ViewModels;
using CNCLib.Wpf.ViewModels;
using CNCLib.Logic.DTO;

namespace CNCLib.Tests.Wpf
{
	[TestClass]
	public class WpfTests
	{
		/*
				[ClassInitialize]
				public static void ClassInit(TestContext testContext)
				{
				}

				[TestInitialize]
				public void Init()
				{
				}
		*/

		private FactoryType2Obj CreateMock()
		{
			var mockfactory = new FactoryType2Obj();
			BaseViewModel.LogicFactory = mockfactory;
			return mockfactory;
        }

		private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
			var mockfactory = CreateMock();
			TInterface rep = Substitute.For<TInterface>();
			mockfactory.Register(typeof(TInterface), rep);
			return rep;
		}

		[TestMethod]
		public void GetMachines()
		{
			var rep = CreateMock<IMachineControler>();

			var machinecommand = new MachineCommand[] 
				{ new MachineCommand() { MachineID = 1, CommandName = "Test1", CommandString = "G20",MachineCommandID =10 },
                  new MachineCommand() { MachineID = 1, CommandName = "Test2", CommandString = "G21",MachineCommandID =11 }
				};
			var machineinitcommand = new MachineInitCommand[]
				{ new MachineInitCommand() { MachineID = 1, SeqNo = 1, CommandString = "G20",MachineInitCommandID =20 },
				  new MachineInitCommand() { MachineID = 1, SeqNo = 2, CommandString = "G21",MachineInitCommandID =21 }
				};

			var machineEntity = new Machine()
			{
				MachineID = 1,
				Name = "Maxi",
				ComPort = "Com7",
				Axis = 3,
				BaudRate = 115200,
				CommandToUpper = true,
				SizeX = 1234,
				SizeY = 5678,
				SizeZ = 987,
				SizeA = 1,
				SizeB = 2,
				SizeC = 3,
				BufferSize = 63,
				ProbeSizeX = 0,
				ProbeSizeY = 0,
				ProbeSizeZ = 25,
				ProbeDist = 3,
				ProbeDistUp = 10,
				ProbeFeed = 300,
				SDSupport = true,
				Spindle = true,
				Coolant = true,
				Rotate = true,
				MachineCommands = machinecommand,
				MachineInitCommands = machineinitcommand
				};

			rep.GetMachine(1).Returns(machineEntity);

			MachineViewModel mv = new MachineViewModel();
			mv.LoadMachine(1);

			Assert.AreEqual(machineEntity.Name, mv.MachineName);

			Assert.AreEqual(machinecommand.Length, mv.MachineCommands.Count);
			Assert.AreEqual(machineinitcommand.Length, mv.MachineInitCommands.Count);
		}
	}
}
