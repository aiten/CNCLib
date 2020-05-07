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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Logic.Manager;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;

using FluentAssertions;

using Framework.Repository.Abstraction;

using NSubstitute;

using Xunit;

using MachineDto = CNCLib.Logic.Abstraction.DTO.Machine;
using MachineInitCommandDto = CNCLib.Logic.Abstraction.DTO.MachineInitCommand;
using MachineCommandDto = CNCLib.Logic.Abstraction.DTO.MachineCommand;

namespace CNCLib.UnitTest.Logic
{
    public class MachineManagerTests : LogicTests
    {
        [Fact]
        public async Task AddMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity1 = new MachineDto
            {
                MachineId = 1,
                Name      = "Maxi",
                MachineCommands = new[]
                {
                    new MachineCommandDto
                    {
                        MachineId        = 1,
                        MachineCommandId = 1,
                        CommandName      = @"1",
                        CommandString    = @"1",
                        PosX             = 0,
                        PosY             = 1
                    }
                },
                MachineInitCommands = new[]
                {
                    new MachineInitCommandDto
                    {
                        MachineId            = 1,
                        MachineInitCommandId = 1,
                        CommandString        = "2",
                        SeqNo                = 1
                    }
                }
            };

            var machineId = await ctrl.Add(machineEntity1);

            rep.ReceivedWithAnyArgs().AddRange(new Machine[1]);
            machineId.Should().Be(1);
        }

        [Fact]
        public async Task UpdateMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity1 = new Machine
            {
                MachineId           = 11,
                Name                = "Maxi",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            rep.Get(11).Returns(machineEntity1);
            rep.GetTracking(Arg.Any<IEnumerable<int>>()).Returns(new[] { machineEntity1 });

            var machine = await ctrl.Get(11);
            machine.Name = "SuperMaxi";

            await ctrl.Update(machine);

            //await rep.Received().Update(11, Arg.Is<Machine>(x => x.Name == "SuperMaxi"));
        }

        [Fact]
        public async Task DeleteMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity1 = new Machine
            {
                MachineId           = 11,
                Name                = "Maxi",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            rep.Get(1).Returns(machineEntity1);

            var machine = await ctrl.Get(1);
            machine.Name = "SuperMaxi";

            await ctrl.Delete(machine);

            rep.Received().DeleteRange(Arg.Is<IEnumerable<Machine>>(x => x.First().Name == "SuperMaxi"));
            rep.Received().DeleteRange(Arg.Is<IEnumerable<Machine>>(x => x.First().MachineId == 11));
        }

        [Fact]
        public async Task GetMachinesNone()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity = new Machine[0];
            rep.GetAll().Returns(machineEntity);

            var machines = (await ctrl.GetAll()).ToArray();
            machines.Length.Should().Be(0);
        }

        [Fact]
        public async Task GetMachinesOne()
        {
            var unitOfWork  = Substitute.For<IUnitOfWork>();
            var rep         = Substitute.For<IMachineRepository>();
            var repC        = Substitute.For<IConfigurationRepository>();
            var userContext = new CNCLibUserContext();

            var ctrl = new MachineManager(unitOfWork, rep, repC, userContext, Mapper);

            var machineEntity = new[]
            {
                new Machine
                {
                    MachineId           = 1,
                    Name                = "Maxi",
                    BufferSize          = 115200,
                    UserId              = userContext.UserId,
                    MachineCommands     = new List<MachineCommand>(),
                    MachineInitCommands = new MachineInitCommand[0]
                }
            };
            rep.GetByUser(userContext.UserId).Returns(machineEntity);

            var machines = (await ctrl.GetAll()).ToArray();
            machines.Length.Should().Be(1);
            machines[0].MachineId.Should().Be(1);
            machines[0].Name.Should().Be("Maxi");
            machines[0].BufferSize.Should().Be(115200);
            machines[0].MachineCommands.Should().NotBeNull();
            machines[0].MachineInitCommands.Should().NotBeNull();
            machines[0].MachineCommands.Count().Should().Be(0);
            machines[0].MachineInitCommands.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetMachinesMany()
        {
            var unitOfWork  = Substitute.For<IUnitOfWork>();
            var rep         = Substitute.For<IMachineRepository>();
            var repC        = Substitute.For<IConfigurationRepository>();
            var userContext = new CNCLibUserContext();

            var ctrl = new MachineManager(unitOfWork, rep, repC, userContext, Mapper);

            var machineEntity = new[]
            {
                new Machine
                {
                    MachineId           = 1,
                    Name                = "Maxi",
                    BufferSize          = 115200,
                    MachineCommands     = new List<MachineCommand>(),
                    MachineInitCommands = new List<MachineInitCommand>(),
                    UserId              = userContext.UserId
                },
                new Machine
                {
                    MachineId  = 2,
                    Name       = "Maxi",
                    BufferSize = 115200,
                    UserId     = userContext.UserId,
                    MachineCommands = new List<MachineCommand>()
                    {
                        new MachineCommand
                        {
                            MachineId        = 2,
                            MachineCommandId = 1,
                            CommandName      = "Test",
                            CommandString    = "f"
                        }
                    },
                    MachineInitCommands = new List<MachineInitCommand>()
                    {
                        new MachineInitCommand
                        {
                            MachineId            = 2,
                            MachineInitCommandId = 1,
                            SeqNo                = 0,
                            CommandString        = "f"
                        }
                    }
                }
            };

            rep.GetByUser(userContext.UserId).Returns(machineEntity);

            var machines = (await ctrl.GetAll()).ToArray();
            machines.Length.Should().Be(2);
            machines[0].MachineId.Should().Be(1);
            machines[0].Name.Should().Be("Maxi");
            machines[0].BufferSize.Should().Be(115200);
            machines[1].MachineCommands.Count().Should().Be(1);
            machines[1].MachineInitCommands.Count().Should().Be(1);
            machines[0].MachineCommands.Count().Should().Be(0);
            machines[0].MachineInitCommands.Count().Should().Be(0);
            machines[1].MachineCommands.First().CommandName.Should().Be("Test");
            machines[1].MachineCommands.First().CommandString.Should().Be("f");
            machines[1].MachineInitCommands.First().SeqNo.Should().Be(0);
            machines[1].MachineInitCommands.First().CommandString.Should().Be("f");
        }

        [Fact]
        public async Task QueryOneMachinesFound()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity1 = new Machine
            {
                MachineId           = 1,
                Name                = "Maxi",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            var machineEntity2 = new Machine
            {
                MachineId           = 2,
                Name                = "Mini",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            rep.Get(1).Returns(machineEntity1);
            rep.Get(2).Returns(machineEntity2);

            var machine = await ctrl.Get(1);
            machineEntity1.Name.Should().Be(machine.Name);
            machineEntity1.MachineId.Should().Be(machine.MachineId);
            machine.MachineCommands.Should().NotBeNull();
            machine.MachineInitCommands.Should().NotBeNull();
            machine.MachineCommands.Count().Should().Be(0);
            machine.MachineInitCommands.Count().Should().Be(0);
        }

        [Fact]
        public async Task QueryOneMachinesNotFound()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machineEntity1 = new Machine
            {
                MachineId           = 1,
                Name                = "Maxi",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            var machineEntity2 = new Machine
            {
                MachineId           = 2,
                Name                = "Mini",
                MachineCommands     = new List<MachineCommand>(),
                MachineInitCommands = new MachineInitCommand[0]
            };
            rep.Get(1).Returns(machineEntity1);
            rep.Get(2).Returns(machineEntity2);

            var machine = await ctrl.Get(3);
            machine.Should().BeNull();
        }

        [Fact]
        public async Task DefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            var machine = await ctrl.Default();
            machine.Should().NotBeNull();
            machine.Name.Should().Be("New");
        }

        [Fact]
        public async Task GetDefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            repC.Get(1, "Environment", "DefaultMachineId").Returns(new Configuration { Value = "14" });
            int dm = await ctrl.GetDefault();

            dm.Should().Be(14);
        }

        [Fact]
        public async Task GetDefaultMachineNotSet()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            repC.Get(1, "Environment", "DefaultMachineId").Returns((Configuration)null);

            (await ctrl.GetDefault()).Should().Be(-1);
        }

        [Fact]
        public async Task SetDefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper);

            await ctrl.SetDefault(15);

            repC.Get(1, "Environment", "DefaultMachineId").Returns(new Configuration { Value = "14" });

            await repC.Received().Store(Arg.Is<Configuration>(x => x.Group == "Environment" && x.Name == "DefaultMachineId" && x.Value == "15"));
        }
    }
}