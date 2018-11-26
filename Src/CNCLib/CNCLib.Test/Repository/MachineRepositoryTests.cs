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
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using FluentAssertions;

using Framework.Dependency;
using Framework.Repository;
using Framework.Test.Repository;
using Framework.Tools;

using Xunit;

namespace CNCLib.Test.Repository
{
    public class MachineRepositoryTests : RepositoryTests<CNCLibContext, Machine, int, IMachineRepository>
    {
        #region crt and overrides

        protected override GetTestDbContext<CNCLibContext, Machine, int, IMachineRepository> CreateTestDbContext()
        {
            var context = new CNCLibContext();
            var uow     = new UnitOfWork<CNCLibContext>(context);
            var rep     = new MachineRepository(context, UserContext);
            return new GetTestDbContext<CNCLibContext, Machine, int, IMachineRepository>(context, uow, rep);
        }

        protected override int GetEntityKey(Machine entity)
        {
            return entity.MachineId;
        }

        protected override Machine SetEntityKey(Machine entity, int key)
        {
            entity.MachineId = key;
            return entity;
        }

        protected override bool CompareEntity(Machine entity1, Machine entity2)
        {
            //entity1.Should().BeEquivalentTo(entity2, opts => 
            //    opts.Excluding(x => x.UserId)
            //);
            return CompareProperties.AreObjectsPropertiesEqual(entity1, entity2, new[]
            {
                @"MachineId", @"MachineCommandId", @"MachineInitCommandId"
            });
        }

        #endregion

        #region CRUD Test

        [Fact]
        public async Task GetAllTest()
        {
            var entities = await GetAll();
            entities.Count().Should().BeGreaterThan(1);
            entities.Count(i => i.Name == "DC-K40-Laser").Should().Be(1);
            entities.Count(i => i.Name == "Laser").Should().Be(1);
        }

        [Fact]
        public async Task GetOKTest()
        {
            var entity = await GetOK(1);
            entity.MachineId.Should().Be(1);
        }

        [Fact]
        public async Task GetTrackingOKTest()
        {
            var entity = await GetTrackingOK(2);
            entity.MachineId.Should().Be(2);
        }

        [Fact]
        public async Task GetNotExistTest()
        {
            await GetNotExist(2342341);
        }

        [Fact]
        public async Task AddUpdateDeleteTest()
        {
            await AddUpdateDelete(() => CreateMachine(@"AddUpdateDeleteTest"), (entity) => entity.Name = "DummyNameUpdate");
        }

        [Fact]
        public async Task AddUpdateDeleteWithCommandAndInitCommandsTest()
        {
            await AddUpdateDelete(() => AddMachineInitCommands((AddMachineCommands(CreateMachine(@"AddUpdateDeleteWithPropertiesTest")))), (entity) =>
            {
                entity.Name = "DummyNameUpdate";
                entity.MachineInitCommands.Remove(entity.MachineInitCommands.First());
                entity.MachineInitCommands.Add(new MachineInitCommand()
                {
                    CommandString = @"CommandStr",
                    SeqNo         = 2
                });

                entity.MachineCommands.Remove(entity.MachineCommands.Last());
                entity.MachineCommands.Add(new MachineCommand()
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
            await AddUpdateDelete(() => AddMachineInitCommands((AddMachineCommands(CreateMachine(@"AddUpdateDeleteWithPropertiesTest")))), (entity) =>
            {
                entity.Name = "DummyNameUpdate";
                entity.MachineInitCommands.Clear();
                entity.MachineCommands.Clear();
            });
        }

        [Fact]
        public async Task AddUpdateDeleteBulkTest()
        {
            await AddUpdateDeleteBulk(() => new[]
            {
                CreateMachine(@"AddUpdateDeleteBulkTest1"), CreateMachine(@"AddUpdateDeleteBulkTest2"), CreateMachine(@"AddUpdateDeleteBulkTest2")
            }, (entities) =>
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
            await AddRollBack(() => CreateMachine(@"AddRollbackTest"));
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