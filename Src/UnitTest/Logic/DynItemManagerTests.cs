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

namespace CNCLib.UnitTest.Logic;

using System;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Client;
using CNCLib.Service.Abstraction;

using FluentAssertions;

using NSubstitute;

using Xunit;

public class DynItemManagerTests : LogicTests
{
    private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
    {
        var srv = Substitute.For<TInterface>();
        return srv;
    }

    [Fact]
    public async Task GetItemNone()
    {
        var srv = CreateMock<IItemService>();

        var itemEntity = new Item[0];
        srv.GetAllAsync().Returns(itemEntity);

        var ctrl = new DynItemController(srv);

        var all = (await ctrl.GetAllAsync()).ToArray();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task GetItemAll()
    {
        var srv = CreateMock<IItemService>();

        var itemEntity = new[]
        {
            new Item { ItemId = 1, Name = "Test1" }, new Item { ItemId = 2, Name = "Test2" }
        };
        srv.GetAllAsync().Returns(itemEntity);

        var ctrl = new DynItemController(srv);
        var all  = (await ctrl.GetAllAsync()).ToArray();

        all.Should().HaveCount(2);
        all.FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                ItemId = 1,
                Name   = "Test1"
            },
            options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAllType()
    {
        var srv = CreateMock<IItemService>();

        var itemEntity = new[]
        {
            new Item { ItemId = 1, Name = "Test1" }, new Item { ItemId = 2, Name = "Test2" }
        };
        srv.GetByClassNameAsync(DynItemController.GetClassName(typeof(string))).Returns(itemEntity);

        var ctrl = new DynItemController(srv);
        var all  = await ctrl.GetAllAsync(typeof(string));

        all.Should().HaveCount(2);
        all.FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                ItemId = 1,
                Name   = "Test1"
            },
            options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetItem()
    {
        var srv = CreateMock<IItemService>();
        srv.GetAsync(1).Returns(new Item { ItemId = 1, Name = "Test1" });

        var ctrl = new DynItemController(srv);
        var all  = await ctrl.GetAsync(1);

        all.Should().BeEquivalentTo(
            new
            {
                ItemId = 1,
                Name   = "Test1"
            },
            options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetItemNull()
    {
        var srv = CreateMock<IItemService>();

        var ctrl = new DynItemController(srv);
        var all  = await ctrl.GetAsync(10);

        all.Should().BeNull();
    }

    [Fact]
    public async Task CreateObject()
    {
        var srv = CreateMock<IItemService>();

        Item itemEntity = CreateItem();

        srv.GetAsync(1).Returns(itemEntity);

        var ctrl = new DynItemController(srv);

        var item = await ctrl.CreateAsync(1);
        item.Should().NotBeNull();
        item.Should().BeOfType(typeof(DynItemManagerTestClass));

        var item2 = (DynItemManagerTestClass)item;

        item2.StringProperty.Should().Be("Hallo", item2.StringProperty);
        item2.IntProperty.Should().Be(1);
        item2.IntProperty.Should().Be(1);
        item2.DoubleProperty.Should().Be(1.234);
        item2.DoubleNullProperty.Should().Be(1.234);
        item2.DecimalProperty.Should().Be(9.876m);
        item2.DecimalNullProperty.Should().Be(9.876m);
    }

    private static Item CreateItem()
    {
        return new Item
        {
            ItemId    = 1,
            Name      = "Hallo",
            ClassName = typeof(DynItemManagerTestClass).AssemblyQualifiedName,
            ItemProperties = new[]
            {
                new ItemProperty { ItemId = 1, Name = "StringProperty", Value                        = "Hallo" },
                new ItemProperty { ItemId = 1, Name = "IntProperty", Value                           = "1" },
                new ItemProperty { ItemId = 1, Name = "DoubleProperty", Value                        = "1.234" },
                new ItemProperty { ItemId = 1, Name = "DecimalProperty", Value                       = "9.876" },
                new ItemProperty { ItemId = 1, Name = "IntNullProperty" }, new ItemProperty { ItemId = 1, Name = "DoubleNullProperty", Value = "1.234" },
                new ItemProperty { ItemId = 1, Name = "DecimalNullProperty", Value                   = "9.876" }
            }
        };
    }

    [Fact]
    public async Task AddObject()
    {
        var srv = CreateMock<IItemService>();

        Item itemEntity = CreateItem();

        var obj = new DynItemManagerTestClass
        {
            StringProperty      = "Hallo",
            IntProperty         = 1,
            DoubleProperty      = 1.234,
            DoubleNullProperty  = 1.234,
            DecimalProperty     = 9.876m,
            DecimalNullProperty = 9.876m
        };

        var ctrl = new DynItemController(srv);

        int id = await ctrl.AddAsync("Hallo", obj);

        await srv.Received().AddAsync(Arg.Is<Item>(x => x.Name == "Hallo"));
        await srv.Received().AddAsync(Arg.Is<Item>(x => x.ItemId == 0));
        await srv.Received().AddAsync(Arg.Is<Item>(x => x.ItemProperties.Count == 7));
        await srv.Received().AddAsync(Arg.Is<Item>(x => x.ItemProperties.FirstOrDefault(y => y.Name == "StringProperty").Value == "Hallo"));
        await srv.Received().AddAsync(Arg.Is<Item>(x => x.ItemProperties.FirstOrDefault(y => y.Name == "DoubleProperty").Value == "1.234"));
        await srv.Received().AddAsync(Arg.Is<Item>(x => x.ItemProperties.FirstOrDefault(y => y.Name == "DecimalNullProperty").Value == "9.876"));
    }

    [Fact]
    public async Task DeleteItem()
    {
        // arrange

        var srv = CreateMock<IItemService>();

        Item itemEntity = CreateItem();
        srv.GetAsync(1).Returns(itemEntity);

        var ctrl = new DynItemController(srv);

        //act

        await ctrl.DeleteAsync(1);

        //assert
        await srv.Received().GetAsync(1);
        await srv.Received().DeleteAsync(itemEntity);
    }

    [Fact]
    public async Task DeleteItemNone()
    {
        // arrange

        var srv = CreateMock<IItemService>();

        var ctrl = new DynItemController(srv);

        //act

        await ctrl.DeleteAsync(1);

        //assert
        await srv.Received().GetAsync(1);
        await srv.DidNotReceiveWithAnyArgs().DeleteAsync((Item)null);
    }

    [Fact]
    public async Task SaveItem()
    {
        // arrange

        var srv  = CreateMock<IItemService>();
        var ctrl = new DynItemController(srv);

        //act

        await ctrl.SaveAsync(1, "Test", new DynItemManagerTestClass { IntProperty = 1 });

        //assert
        await srv.Received().UpdateAsync(Arg.Is<Item>(x => x.ItemId == 1));
        await srv.Received().UpdateAsync(Arg.Is<Item>(x => x.ItemProperties.FirstOrDefault(y => y.Name == "IntProperty").Value == "1"));
        await srv.DidNotReceiveWithAnyArgs().DeleteAsync((Item)null);
    }
}