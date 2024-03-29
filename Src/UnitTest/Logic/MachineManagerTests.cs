﻿/*
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

namespace CNCLib.UnitTest.Logic;

using System;
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

public class MachineManagerTests : LogicTests
{
    [Fact]
    public async Task AddMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity1 = new MachineDto
        {
            MachineId = 1,
            Name      = "Maxi",
            ComPort   = "x",
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

        var machineId = await ctrl.AddAsync(machineEntity1);

        await rep.ReceivedWithAnyArgs().AddRangeAsync(new MachineEntity[1]);
        machineId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity1 = new MachineEntity
        {
            MachineId           = 11,
            Name                = "Maxi",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        rep.GetAsync(11).Returns(machineEntity1);
        rep.GetTrackingAsync(Arg.Any<IEnumerable<int>>()).Returns(new[] { machineEntity1 });

        var machine = (await ctrl.GetAsync(11))!;
        machine.Name = "SuperMaxi";

        await ctrl.UpdateAsync(machine);

        //await rep.Received().UpdateAsync(11, Arg.Is<MachineEntity>(x => x.Name == "SuperMaxi"));
    }

    [Fact]
    public async Task DeleteMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity1 = new MachineEntity
        {
            MachineId           = 11,
            Name                = "Maxi",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        rep.GetAsync(1).Returns(machineEntity1);

        var machine = (await ctrl.GetAsync(1))!;
        machine.Name = "SuperMaxi";

        await ctrl.DeleteAsync(machine);

        await rep.Received().DeleteRangeAsync(Arg.Is<IEnumerable<MachineEntity>>(x => x.First().Name == "SuperMaxi"));
        await rep.Received().DeleteRangeAsync(Arg.Is<IEnumerable<MachineEntity>>(x => x.First().MachineId == 11));
    }

    [Fact]
    public async Task GetMachinesNone()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity = Array.Empty<MachineEntity>();
        rep.GetAllAsync().Returns(machineEntity);

        var machines = (await ctrl.GetAllAsync()).ToArray();
        machines.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetMachinesOne()
    {
        var unitOfWork  = Substitute.For<IUnitOfWork>();
        var rep         = Substitute.For<IMachineRepository>();
        var repC        = Substitute.For<IConfigurationRepository>();
        var userContext = new CNCLibUserContext();

        var ctrl = new MachineManager(unitOfWork, rep, repC, userContext, Mapper!);

        var machineEntity = new[]
        {
            new MachineEntity
            {
                MachineId           = 1,
                Name                = "Maxi",
                ComPort             = "x",
                BufferSize          = 115200,
                UserId              = userContext.UserId,
                MachineCommands     = new List<MachineCommandEntity>(),
                MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
            }
        };
        rep.GetByUserAsync(userContext.UserId).Returns(machineEntity);

        var machines = (await ctrl.GetAllAsync()).ToArray();
        machines.Should().HaveCount(1);
        machines[0].MachineId.Should().Be(1);
        machines[0].Name.Should().Be("Maxi");
        machines[0].BufferSize.Should().Be(115200);
        machines[0].MachineCommands.Should().NotBeNull();
        machines[0].MachineInitCommands.Should().NotBeNull();
        machines[0].MachineCommands.Should().HaveCount(0);
        machines[0].MachineInitCommands.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetMachinesMany()
    {
        var unitOfWork  = Substitute.For<IUnitOfWork>();
        var rep         = Substitute.For<IMachineRepository>();
        var repC        = Substitute.For<IConfigurationRepository>();
        var userContext = new CNCLibUserContext();

        var ctrl = new MachineManager(unitOfWork, rep, repC, userContext, Mapper!);

        var machineEntity = new[]
        {
            new MachineEntity
            {
                MachineId           = 1,
                Name                = "Maxi",
                ComPort             = "x",
                BufferSize          = 115200,
                MachineCommands     = new List<MachineCommandEntity>(),
                MachineInitCommands = new List<MachineInitCommandEntity>(),
                UserId              = userContext.UserId
            },
            new MachineEntity
            {
                MachineId  = 2,
                Name       = "Maxi",
                ComPort    = "x",
                BufferSize = 115200,
                UserId     = userContext.UserId,
                MachineCommands = new List<MachineCommandEntity>()
                {
                    new MachineCommandEntity
                    {
                        MachineId        = 2,
                        MachineCommandId = 1,
                        CommandName      = "Test",
                        CommandString    = "f"
                    }
                },
                MachineInitCommands = new List<MachineInitCommandEntity>()
                {
                    new MachineInitCommandEntity
                    {
                        MachineId            = 2,
                        MachineInitCommandId = 1,
                        SeqNo                = 0,
                        CommandString        = "f"
                    }
                }
            }
        };

        rep.GetByUserAsync(userContext.UserId).Returns(machineEntity);

        var machines = (await ctrl.GetAllAsync()).ToArray();
        machines.Should().HaveCount(2);
        machines[0].MachineId.Should().Be(1);
        machines[0].Name.Should().Be("Maxi");
        machines[0].BufferSize.Should().Be(115200);
        machines[1].MachineCommands.Should().HaveCount(1);
        machines[1].MachineInitCommands.Should().HaveCount(1);
        machines[0].MachineCommands.Should().HaveCount(0);
        machines[0].MachineInitCommands.Should().HaveCount(0);
        machines[1].MachineCommands!.First().CommandName.Should().Be("Test");
        machines[1].MachineCommands!.First().CommandString.Should().Be("f");
        machines[1].MachineInitCommands!.First().SeqNo.Should().Be(0);
        machines[1].MachineInitCommands!.First().CommandString.Should().Be("f");
    }

    [Fact]
    public async Task QueryOneMachinesFound()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity1 = new MachineEntity
        {
            MachineId           = 1,
            Name                = "Maxi",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        var machineEntity2 = new MachineEntity
        {
            MachineId           = 2,
            Name                = "Mini",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        rep.GetAsync(1).Returns(machineEntity1);
        rep.GetAsync(2).Returns(machineEntity2);

        var machine = (await ctrl.GetAsync(1))!;
        machineEntity1.Name.Should().Be(machine.Name);
        machineEntity1.MachineId.Should().Be(machine.MachineId);
        machine.MachineCommands.Should().NotBeNull();
        machine.MachineInitCommands.Should().NotBeNull();
        machine.MachineCommands.Should().HaveCount(0);
        machine.MachineInitCommands.Should().HaveCount(0);
    }

    [Fact]
    public async Task QueryOneMachinesNotFound()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machineEntity1 = new MachineEntity
        {
            MachineId           = 1,
            Name                = "Maxi",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        var machineEntity2 = new MachineEntity
        {
            MachineId           = 2,
            Name                = "Mini",
            ComPort             = "x",
            MachineCommands     = new List<MachineCommandEntity>(),
            MachineInitCommands = Array.Empty<MachineInitCommandEntity>()
        };
        rep.GetAsync(1).Returns(machineEntity1);
        rep.GetAsync(2).Returns(machineEntity2);

        var machine = await ctrl.GetAsync(3);
        machine.Should().BeNull();
    }

    [Fact]
    public async Task DefaultMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        var machine = await ctrl.DefaultAsync();
        machine.Should().NotBeNull();
        machine.Name.Should().Be("New");
    }

    [Fact]
    public async Task GetDefaultMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        repC.GetAsync(1, "Environment", "DefaultMachineId").Returns(new ConfigurationEntity() { Group = "x", Name = "x", Type = "x", Value = "14" });
        int dm = await ctrl.GetDefaultAsync();

        dm.Should().Be(14);
    }

    [Fact]
    public async Task GetDefaultMachineNotSet()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        repC.GetAsync(1, "Environment", "DefaultMachineId").Returns(((ConfigurationEntity)null!));

        (await ctrl.GetDefaultAsync()).Should().Be(-1);
    }

    [Fact]
    public async Task SetDefaultMachine()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IMachineRepository>();
        var repC       = Substitute.For<IConfigurationRepository>();

        var ctrl = new MachineManager(unitOfWork, rep, repC, new CNCLibUserContext(), Mapper!);

        await ctrl.SetDefaultAsync(15);

        repC.GetAsync(1, "Environment", "DefaultMachineId").Returns(new ConfigurationEntity { Value = "14", Group = "x", Name = "x", Type = "x" });

        await repC.Received().StoreAsync(Arg.Is<ConfigurationEntity>(x => x.Group == "Environment" && x.Name == "DefaultMachineId" && x.Value == "15"));
    }
}