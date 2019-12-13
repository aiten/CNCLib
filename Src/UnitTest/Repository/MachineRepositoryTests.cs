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

namespace CNCLib.UnitTest.Repository
{
    public class MachineRepositoryTests : RepositoryTests
    {
        #region crt and overrides

        public MachineRepositoryTests(RepositoryTestFixture testFixture) : base(testFixture)
        {
        }

        protected CRUDRepositoryTests<CNCLibContext, Machine, int, IMachineRepository> CreateTestContext()
        {
            return new CRUDRepositoryTests<CNCLibContext, Machine, int, IMachineRepository>()
            {
                CreateTestDbContext = () =>
                {
                    var context = TestFixture.CreateDbContext();
                    var uow     = new UnitOfWork<CNCLibContext>(context);
                    var rep     = new MachineRepository(context);
                    return new CRUDTestDbContext<CNCLibContext, Machine, int, IMachineRepository>(context, uow, rep);
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
                CompareEntity = (entity1, entity2) => CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[] { @"MachineId", @"MachineCommandId", @"MachineInitCommandId" })
            };
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = await CreateTestContext().GetAll();
            entities.Count().Should().BeGreaterThan(1);
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
                        new MachineInitCommand()
                        {
                            CommandString = @"CommandStr",
                            SeqNo         = 2
                        });

                    entity.MachineCommands.Remove(entity.MachineCommands.Last());
                    entity.MachineCommands.Add(
                        new MachineCommand()
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
                    CreateMachine(@"AddUpdateDeleteBulkTest1"), CreateMachine(@"AddUpdateDeleteBulkTest2"), CreateMachine(@"AddUpdateDeleteBulkTest2")
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

        private static Machine CreateMachine(string name)
        {
            var machine = new Machine
            {
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
                MachineInitCommands = new HashSet<MachineInitCommand>(),
                MachineCommands     = new HashSet<MachineCommand>()
            };
            return machine;
        }

        private static Machine AddMachineCommands(Machine machine)
        {
            machine.MachineCommands = new List<MachineCommand>
            {
                new MachineCommand { CommandName = "Name1", CommandString = "Test1", Machine = machine },
                new MachineCommand { CommandName = "Name1", CommandString = "Test2", Machine = machine }
            };
            return machine;
        }

        private static Machine AddMachineInitCommands(Machine machine)
        {
            machine.MachineInitCommands = new List<MachineInitCommand>
            {
                new MachineInitCommand { SeqNo = 0, CommandString = "Test1", Machine = machine },
                new MachineInitCommand { SeqNo = 1, CommandString = "Test2", Machine = machine }
            };
            return machine;
        }
    }
}