/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;
using CNCLib.WpfClient.ViewModels;

using FluentAssertions;

using Framework.Pattern;

using NSubstitute;

using Xunit;

namespace CNCLib.WpfClient.Test.ViewModels
{
    public class MachineViewModelTests : WpfUnitTestBase
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
//			var mockFactory = CreateMock();
            var rep = Substitute.For<TInterface>();

//			mockFactory.Register(typeof(TInterface), rep);

            return rep;
        }

        [Fact]
        public async Task GetMachine()
        {
            var rep = CreateMock<IMachineService>();

            Machine machine = CreateMachine(1);
            rep.Get(1).Returns(machine);

            var mv = new MachineViewModel(new FactoryInstance<IMachineService>(rep), Mapper, new Global());
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

        [Fact]
        public void GetMachineAddNew()
        {
            var rep = CreateMock<IMachineService>();

            Machine machine1 = CreateMachine(1);
            rep.Get(1).Returns(machine1);

            Machine machineDef = CreateMachine(0);
            rep.DefaultMachine().Returns(machineDef);

            var mv = new MachineViewModel(new FactoryInstance<IMachineService>(rep), Mapper, new Global());
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