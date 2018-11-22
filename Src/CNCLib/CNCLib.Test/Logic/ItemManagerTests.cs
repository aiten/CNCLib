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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Manager;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entity;

using FluentAssertions;

using Framework.Dependency;
using Framework.Repository;
using Framework.Repository.Abstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using ItemDto = CNCLib.Logic.Contract.DTO.Item;

namespace CNCLib.Test.Logic
{
    [TestClass]
    public class ItemManagerTests : LogicTests
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
            var rep = Substitute.For<TInterface>();
            Dependency.Container.RegisterInstance(rep);

            //			TInterface uow = Substitute.For<Framework.EF.UnitOfWork>();
            //			Dependency.Container.RegisterInstance(uow);

            Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLib.Repository.Context.CNCLibContext>>();

            return rep;
        }

        [TestMethod]
        public async Task GetItemNone()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var itemEntity = new Item[0];
            rep.GetAll().Returns(itemEntity);

            var all = (await ctrl.GetAll()).ToArray();
            all.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetItemAll()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

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
                }.Should().
                BeEquivalentTo(all.FirstOrDefault(), options => options.ExcludingMissingMembers());
        }

        [TestMethod]
        public async Task GetItem()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            rep.Get(1).Returns(new Item { ItemId = 1, Name = "Test1" });

            var all = await ctrl.Get(1);

            new
                {
                    ItemId = 1,
                    Name   = "Test1"
                }.Should().
                BeEquivalentTo(all, options => options.ExcludingMissingMembers());
        }

        [TestMethod]
        public async Task GetItemNull()
        {
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var all = await ctrl.Get(10);

            all.Should().BeNull();
        }

        [TestMethod]
        public async Task DeleteItemNone()
        {
            // arrange

            var unitOfWork = Substitute.For<IUnitOfWork>();
            var rep        = Substitute.For<IItemRepository>();

            var ctrl = new ItemManager(unitOfWork, rep, new CNCLibUserContext(), Dependency.Resolve<IMapper>());

            var item = new ItemDto { ItemId = 3000, Name = "Hallo" };

            //act

            await ctrl.Delete(item);

            //assert
            rep.Received().DeleteRange(Arg.Is<IEnumerable<Item>>(x => x.First().ItemId == item.ItemId));
        }
    }
}