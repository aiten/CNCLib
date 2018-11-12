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

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using NSubstitute;

using CNCLib.Wpf.ViewModels;
using CNCLib.Logic.Contracts.DTO;

using System.Threading.Tasks;

using CNCLib.Service.Contracts;

using FluentAssertions;

using Framework.Dependency;
using Framework.Pattern;

namespace CNCLib.Tests.Wpf
{
    [TestClass]
    public class MachineViewModelTests : CNCUnitTest
    {
        /*
                [ClassInitialize]
                public static void ClassInit(CRUDTestDbContext testContext)
                {
                }

                [TestInitialize]
                public void Init()
                {
                }
        */
/*
		private FactoryType2Obj CreateMock()
		{
			var mockFactory = new FactoryType2Obj();
			BaseViewModel.LogicFactory = mockFactory;
			return mockFactory;
        }
*/
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
//			var mockFactory = CreateMock();
            var rep = Substitute.For<TInterface>();
//			mockFactory.Register(typeof(TInterface), rep);

            Dependency.Container.RegisterInstance(rep);

            return rep;
        }

        [TestMethod]
        public async Task GetMachine()
        {
            var rep = CreateMock<IMachineService>();

            Machine machine = CreateMachine(1);
            rep.Get(1).Returns(machine);

            var mv = new MachineViewModel(new FactoryInstance<IMachineService>(rep));
            await mv.LoadMachine(1);

            mv.AddNewMachine.Should().Be(false);
            mv.Machine.Name.Should().Be(machine.Name);

            mv.MachineCommands.Count.Should().Be(machine.MachineCommands.Count());
            mv.MachineInitCommands.Count.Should().Be(machine.MachineInitCommands.Count());

            mv.Machine.Name.Should().Be(machine.Name);
            mv.Machine.ComPort.Should().Be(machine.ComPort);
            mv.Machine.Axis.Should().Be(machine.Axis);
            mv.Machine.BaudRate.Should().Be(machine.BaudRate);
            mv.Machine.CommandToUpper.Should().Be(machine.CommandToUpper);
            mv.Machine.SizeX.Should().Be(machine.SizeX);
            mv.Machine.SizeY.Should().Be(machine.SizeY);
            mv.Machine.SizeZ.Should().Be(machine.SizeZ);
            mv.Machine.SizeA.Should().Be(machine.SizeA);
            mv.Machine.SizeB.Should().Be(machine.SizeB);
            mv.Machine.SizeC.Should().Be(machine.SizeC);
            mv.Machine.BufferSize.Should().Be(machine.BufferSize);
            mv.Machine.ProbeSizeX.Should().Be(machine.ProbeSizeX);
            mv.Machine.ProbeSizeY.Should().Be(machine.ProbeSizeY);
            mv.Machine.ProbeSizeZ.Should().Be(machine.ProbeSizeZ);
            mv.Machine.ProbeDist.Should().Be(machine.ProbeDist);
            mv.Machine.ProbeDistUp.Should().Be(machine.ProbeDistUp);
            mv.Machine.ProbeFeed.Should().Be(machine.ProbeFeed);
            mv.Machine.SDSupport.Should().Be(machine.SDSupport);
            mv.Machine.Spindle.Should().Be(machine.Spindle);
            mv.Machine.Coolant.Should().Be(machine.Coolant);

            mv.Machine.Rotate.Should().Be(machine.Rotate);
        }

        [TestMethod]
        public void GetMachineAddNew()
        {
            var rep = CreateMock<IMachineService>();

            Machine machine1 = CreateMachine(1);
            rep.Get(1).Returns(machine1);

            Machine machineDef = CreateMachine(0);
            rep.DefaultMachine().Returns(machineDef);

            var mv = new MachineViewModel(new FactoryInstance<IMachineService>(rep));
            mv.LoadMachine(-1);

            mv.AddNewMachine.Should().BeTrue();
            mv.Machine.Name.Should().Be(machineDef.Name);

            mv.MachineCommands.Count.Should().Be(machineDef.MachineCommands.Count());
            mv.MachineInitCommands.Count.Should().Be(machineDef.MachineInitCommands.Count());
        }

        private Machine CreateMachine(int machineId)
        {
            var machineCommands = new[]
            {
                new MachineCommand
                {
                    MachineId        = machineId,
                    CommandName      = "Test1",
                    CommandString    = "G20",
                    MachineCommandId = machineId * 10 + 0
                },
                new MachineCommand
                {
                    MachineId        = machineId,
                    CommandName      = "Test2",
                    CommandString    = "G21",
                    MachineCommandId = machineId * 10 + 1
                }
            };
            var machineInitCommands = new[]
            {
                new MachineInitCommand
                {
                    MachineId            = machineId,
                    SeqNo                = 1,
                    CommandString        = "G20",
                    MachineInitCommandId = machineId * 20
                },
                new MachineInitCommand
                {
                    MachineId            = 1,
                    SeqNo                = 2,
                    CommandString        = "G21",
                    MachineInitCommandId = machineId * 20 + 1
                }
            };
            var machine = new Machine
            {
                MachineId           = machineId,
                Name                = "Maxi" + machineId.ToString(),
                ComPort             = "Com7",
                Axis                = 3,
                BaudRate            = 115200,
                DtrIsReset          = true,
                CommandToUpper      = true,
                SizeX               = 1234,
                SizeY               = 5678,
                SizeZ               = 987,
                SizeA               = 1,
                SizeB               = 2,
                SizeC               = 3,
                BufferSize          = 63,
                ProbeSizeX          = 0,
                ProbeSizeY          = 0,
                ProbeSizeZ          = 25,
                ProbeDist           = 3,
                ProbeDistUp         = 10,
                ProbeFeed           = 300,
                SDSupport           = true,
                Spindle             = true,
                Coolant             = true,
                Rotate              = true,
                MachineCommands     = machineCommands,
                MachineInitCommands = machineInitCommands
            };

            return machine;
        }
    }
}