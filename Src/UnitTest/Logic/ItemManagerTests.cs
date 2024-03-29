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

using ItemDto = CNCLib.Logic.Abstraction.DTO.Item;

public class ItemManagerTests : LogicTests
{
    [Fact]
    public async Task GetItemNone()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IItemRepository>();

        var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper!);

        var itemEntity = Array.Empty<ItemEntity>();
        rep.GetAllAsync().Returns(itemEntity);

        var all = (await ctrl.GetAllAsync()).ToArray();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task GetItemAll()
    {
        var unitOfWork  = Substitute.For<IUnitOfWork>();
        var rep         = Substitute.For<IItemRepository>();
        var userContext = new CNCLibUserContext();

        var ctrl = new ItemManager(unitOfWork, rep, userContext, Mapper!);

        var itemEntity = new[]
        {
            new ItemEntity { ItemId = 1, Name = "Test1", UserId = userContext.UserId, ClassName = "x" },
            new ItemEntity { ItemId = 2, Name = "Test2", UserId = userContext.UserId, ClassName = "x" }
        };
        rep.GetByUserAsync(userContext.UserId).Returns(itemEntity);

        var all = (await ctrl.GetAllAsync()).ToArray();

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

        var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper!);

        rep.GetAsync(1).Returns(new ItemEntity { ItemId = 1, Name = "Test1", ClassName = "x" });

        var all = await ctrl.GetAsync(1);

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

        var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper!);

        var all = await ctrl.GetAsync(10);

        all.Should().BeNull();
    }

    [Fact]
    public async Task DeleteItemNone()
    {
        // arrange

        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rep        = Substitute.For<IItemRepository>();

        var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Mapper!);

        var item = new ItemDto { ItemId = 3000, Name = "Hallo", ClassName = "x" };

        //act

        await ctrl.DeleteAsync(item);

        //assert
        await rep.Received().DeleteRangeAsync(Arg.Is<IEnumerable<ItemEntity>>(x => x.First().ItemId == item.ItemId));
    }
}