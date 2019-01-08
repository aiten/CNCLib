////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Logic.Manager;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using FluentAssertions;

using Framework.Repository.Abstraction;

using NSubstitute;

using Xunit;

using ItemDto = CNCLib.Logic.Contract.DTO.Item;

namespace CNCLib.Test.Logic
{
    public class ItemManagerTests : LogicTests
    {
        [Fact]
        public async Task GetItemNone()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper);

            var itemEntity = new Item[0];
            rep.GetAll().Returns(itemEntity);

            var all = (await ctrl.GetAll()).ToArray();
            all.Should().BeEmpty();
        }

        [Fact]
        public async Task GetItemAll()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper);

            var itemEntity = new[]
            {
                new Item { ItemId = 1, Name = "Test1" }, new Item { ItemId = 2, Name = "Test2" }
            };
            rep.GetAll().Returns(itemEntity);

            var all = (await ctrl.GetAll()).ToArray();

            all.Should().HaveCount(2);
            new
            {
                ItemId = 1,
                Name   = "Test1"
            }.Should().BeEquivalentTo(all.FirstOrDefault(), options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetItem()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper);

            rep.Get(1).Returns(new Item { ItemId = 1, Name = "Test1" });

            var all = await ctrl.Get(1);

            new
            {
                ItemId = 1,
                Name   = "Test1"
            }.Should().BeEquivalentTo(all, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetItemNull()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper);

            var all = await ctrl.Get(10);

            all.Should().BeNull();
        }

        [Fact]
        public async Task DeleteItemNone()
        {
            // arrange

            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper);

            var item = new ItemDto { ItemId = 3000, Name = "Hallo" };

            //act

            await ctrl.Delete(item);

            //assert
            rep.Received().DeleteRange(Arg.Is<IEnumerable<Item>>(x => x.First().ItemId == item.ItemId));
        }
    }
}