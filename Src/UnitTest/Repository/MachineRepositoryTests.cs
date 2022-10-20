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

namespace CNCLib.UnitTest.Repository;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

using FluentAssertions;

using Framework.Repository;
using Framework.Tools;
using Framework.UnitTest.Repository;

using Xunit;

public class MachineRepositoryTests : RepositoryTests
{
    #region crt and overrides

    public MachineRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
    {
    }

    protected CrudRepositoryTests<CNCLibContext, MachineEntity, int, IMachineRepository> CreateTestContext()
    {
        return new CrudRepositoryTests<CNCLibContext, MachineEntity, int, IMachineRepository>()
        {
            CreateTestDbContext = () =>
            {
                var context = TestFixture.CreateDbContext();
                var uow     = new UnitOfWork<CNCLibContext>(context);
                var rep     = new MachineRepository(context);
                return new CrudTestDbContext<CNCLibContext, MachineEntity, int, IMachineRepository>(context, uow, rep);
            },
            GetEntityKey = (entity) => entity.MachineId,
            SetEntityKey = (entity, key) =>
            {
                entity.MachineId = key;
                foreach (var mc in entity.MachineCommands)
                {
                    mc.MachineId = key;
                }

                foreach (var mic in entity.MachineInitCommands)
                {
                    mic.MachineId = key;
                }
            },
            CompareEntity = (entity1, entity2) => entity1.ArePropertiesEqual(entity2, new[] { @"MachineId", @"MachineCommandId", @"MachineInitCommandId", @"User" })
        };
    }

    #endregion

    #region CRUD Test

    [Fact]
    public async Task GetAllTest()
    {
        var entities = await CreateTestContext().GetAll();
        entities.Should().HaveCountGreaterThan(1);
        entities.Count(i => i.Name == "DC-K40-Laser").Should().Be(1);
        entities.Count(i => i.Name == "Laser").Should().Be(1);
    }

    [Fact]
    public async Task GetOKTest()
    {
        var entity = await CreateTestContext().GetOK(1);
        entity.MachineId.Should().Be(1);
    }

    [Fact]
    public async Task GetTrackingOKTest()
    {
        var entity = await CreateTestContext().GetTrackingOK(2);
        entity.MachineId.Should().Be(2);
    }

    [Fact]
    public async Task GetNotExistTest()
    {
        await CreateTestContext().GetNotExist(2342341);
    }

    [Fact]
    public async Task AddUpdateDeleteTest()
    {
        await CreateTestContext().AddUpdateDelete(() => CreateMachine(@"AddUpdateDeleteTest"), (entity) => entity.Name = "DummyNameUpdate");
    }

    [Fact]
    public async Task AddUpdateDeleteWithCommandAndInitCommandsTest()
    {
        await CreateTestContext().AddUpdateDelete(
            () => AddMachineInitCommands((AddMachineCommands(CreateMachine(@"AddUpdateDeleteWithPropertiesTest")))),
            (entity) =>
            {
                entity.Name = "DummyNameUpdate";
                entity.MachineInitCommands.Remove(entity.MachineInitCommands.First());
                entity.MachineInitCommands.Add(
                    new MachineInitCommandEntity()
                    {
                        CommandString = @"CommandStr",
                        SeqNo         = 2
                    });

                entity.MachineCommands.Remove(entity.MachineCommands.Last());
                entity.MachineCommands.Add(
                    new MachineCommandEntity()
                    {
                        CommandString   = @"CommandStr",
                        CommandName     = "NewName",
                        JoystickMessage = "Maxi",
                        PosX            = 2,
                        PosY            = 3
                    });
            });
    }

    [Fact]
    public async Task AddUpdateDeleteWithCommandAndInitCommandsToEmptyTest()
    {
        await CreateTestContext().AddUpdateDelete(
            () => AddMachineInitCommands((AddMachineCommands(CreateMachine(@"AddUpdateDeleteWithPropertiesTest")))),
            (entity) =>
            {
                entity.Name = "DummyNameUpdate";
                entity.MachineInitCommands.Clear();
                entity.MachineCommands.Clear();
            });
    }

    [Fact]
    public async Task AddUpdateDeleteBulkTest()
    {
        await CreateTestContext().AddUpdateDeleteBulk(
            () => new[]
            {
                CreateMachine(@"AddUpdateDeleteBulkTest1"), CreateMachine(@"AddUpdateDeleteBulkTest2"), CreateMachine(@"AddUpdateDeleteBulkTest3")
            },
            (entities) =>
            {
                int i = 0;
                foreach (var entity in entities)
                {
                    entity.Name = $"DummyNameUpdate{i++}";
                }
            });
    }

    [Fact]
    public async Task AddRollbackTest()
    {
        await CreateTestContext().AddRollBack(() => CreateMachine(@"AddRollbackTest"));
    }

    #endregion

    private static MachineEntity CreateMachine(string name)
    {
        var machine = new MachineEntity
        {
            UserId              = 1,
            ComPort             = "com47",
            Axis                = 2,
            BaudRate            = 6500,
            DtrIsReset          = true,
            Name                = name,
            SizeX               = 1m,
            SizeY               = 1m,
            SizeZ               = 1m,
            SizeA               = 1m,
            SizeB               = 1m,
            SizeC               = 1m,
            BufferSize          = 63,
            CommandToUpper      = true,
            ProbeSizeX          = 1m,
            ProbeSizeY          = 1m,
            ProbeSizeZ          = 1m,
            ProbeDistUp         = 1m,
            ProbeDist           = 1m,
            ProbeFeed           = 1m,
            MachineInitCommands = new HashSet<MachineInitCommandEntity>(),
            MachineCommands     = new HashSet<MachineCommandEntity>()
        };
        return machine;
    }

    private static MachineEntity AddMachineCommands(MachineEntity machine)
    {
        machine.MachineCommands = new List<MachineCommandEntity>
        {
            new MachineCommandEntity { CommandName = "Name1", CommandString = "Test1", Machine = machine },
            new MachineCommandEntity { CommandName = "Name1", CommandString = "Test2", Machine = machine }
        };
        return machine;
    }

    private static MachineEntity AddMachineInitCommands(MachineEntity machine)
    {
        machine.MachineInitCommands = new List<MachineInitCommandEntity>
        {
            new MachineInitCommandEntity { SeqNo = 0, CommandString = "Test1", Machine = machine },
            new MachineInitCommandEntity { SeqNo = 1, CommandString = "Test2", Machine = machine }
        };
        return machine;
    }
}