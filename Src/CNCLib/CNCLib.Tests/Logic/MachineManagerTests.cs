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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Contracts.Entities;
using CNCLib.Repository.Contracts;
using System.Linq;
using NSubstitute;
using System.Threading.Tasks;
using AutoMapper;
using CNCLib.Logic.Manager;
using FluentAssertions;
using Framework.Contracts.Repository;
using Framework.Dependency;
using MachineDto = CNCLib.Logic.Contracts.DTO.Machine;
using MachineInitCommandDto = CNCLib.Logic.Contracts.DTO.MachineInitCommand;
using MachineCommandDto = CNCLib.Logic.Contracts.DTO.MachineCommand;

namespace CNCLib.Tests.Logic
{
    [TestClass]
    public class MachineManagerTests : LogicTests
    {
        [TestMethod]
        public async Task AddMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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

        [TestMethod]
        public async Task UpdateMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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

        [TestMethod]
        public async Task DeleteMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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

        [TestMethod]
        public async Task GetMachinesNone()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var machineEntity = new Machine[0];
            rep.GetAll().Returns(machineEntity);

            var machines = (await ctrl.GetAll()).ToArray();
            machines.Length.Should().Be(0);
        }

        [TestMethod]
        public async Task GetMachinesOne()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var machineEntity = new[]
            {
                new Machine
                {
                    MachineId           = 1,
                    Name                = "Maxi",
                    BufferSize          = 115200,
                    MachineCommands     = new List<MachineCommand>(),
                    MachineInitCommands = new MachineInitCommand[0]
                }
            };
            rep.GetAll().Returns(machineEntity);

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

        [TestMethod]
        public async Task GetMachinesMany()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var machineEntity = new[]
            {
                new Machine
                {
                    MachineId           = 1,
                    Name                = "Maxi",
                    BufferSize          = 115200,
                    MachineCommands     = new List<MachineCommand>(),
                    MachineInitCommands = new List<MachineInitCommand>()
                },
                new Machine
                {
                    MachineId  = 2,
                    Name       = "Maxi",
                    BufferSize = 115200,
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

            rep.GetAll().Returns(machineEntity);

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

        [TestMethod]
        public async Task QueryOneMachinesFound()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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

        [TestMethod]
        public async Task QueryOneMachinesNotFound()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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

        [TestMethod]
        public async Task DefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var machine = await ctrl.DefaultMachine();
            machine.Should().NotBeNull();
            machine.Name.Should().Be("New");
        }

        [TestMethod]
        public async Task GetDefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            repC.Get("Environment", "DefaultMachineId").Returns(new Configuration { Value = "14" });
            int dm = await ctrl.GetDetaultMachine();

            dm.Should().Be(14);
        }

        [TestMethod]
        public async Task GetDefaultMachineNotSet()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            repC.Get("Environment", "DefaultMachineId").Returns((Configuration) null);

            (await ctrl.GetDetaultMachine()).Should().Be(-1);
        }

        [TestMethod]
        public async Task SetDefaultMachine()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IMachineRepository>();
            var repC       = Substitute.For<IConfigurationRepository>();

            var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            await ctrl.SetDetaultMachine(15);

            repC.Get("Environment", "DefaultMachineId").Returns(new Configuration { Value = "14" });

            await repC.Received().Store(Arg.Is<Configuration>(x => x.Group == "Environment" && x.Name == "DefaultMachineId" && x.Value == "15"));
        }
    }
}