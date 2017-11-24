////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using FluentAssertions;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CNCLib.Tests.Logic
{
    [TestClass]
	public class ItemControllerTests : CNCUnitTest
	{
		private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
			var rep = Substitute.For<TInterface>();
            Dependency.Container.RegisterInstance(rep);

//			TInterface uow = Substitute.For<Framework.EF.UnitOfWork>();
//			Dependency.Container.RegisterInstance(uow);

			Dependency.Container.RegisterType<Framework.Tools.Pattern.IUnitOfWork, Framework.EF.UnitOfWork<CNCLib.Repository.Context.CNCLibContext>>();

			return rep;
		}


		[TestMethod]
		public async Task GetItemNone()
		{
			var rep = CreateMock<IItemRepository>();

			var itemEntity = new Item[0];
			rep.Get().Returns(itemEntity);

			var ctrl = new ItemController();

			var all = (await ctrl.GetAll()).ToArray();
			all.Should().BeEmpty();
		}

		[TestMethod]
		public async Task GetItemAll()
		{
			var rep = CreateMock<IItemRepository>();

			var itemEntity = new []
			{
				new Item { ItemID=1,Name="Test1" },
				new Item { ItemID=2,Name="Test2" }
			};
			rep.Get().Returns(itemEntity);

			var ctrl = new ItemController();

			var all = (await ctrl.GetAll()).ToArray();

            all.Should().HaveCount(2);
            new
            {
                ItemID = 1,
                Name = "Test1"
            }
            .ShouldBeEquivalentTo(all.FirstOrDefault());
		}

		[TestMethod]
		public async Task GetItem()
		{
			var rep = CreateMock<IItemRepository>();
			rep.Get(1).Returns(new Item { ItemID = 1, Name = "Test1" });

			var ctrl = new ItemController();

			var all = await ctrl.Get(1);

            new
            {
                ItemID = 1,
                Name = "Test1"
            }
            .ShouldBeEquivalentTo(all);
		}

		[TestMethod]
		public async Task GetItemNull()
		{
			var rep = CreateMock<IItemRepository>();

			var ctrl = new ItemController();

			var all = await ctrl.Get(10);

            all.Should().BeNull();
		}

        [TestMethod]
        public async Task DeleteItemNone()
        {
            // arrange

            var rep = CreateMock<IItemRepository>();

            var ctrl = new ItemController();

			var item = new CNCLib.Logic.Contracts.DTO.Item { ItemID = 3000, Name = "Hallo" };

            //act

            await ctrl.Delete(item);

			//assert
			await rep.Received().Delete(Arg.Is<Item>(x => x.ItemID == item.ItemID));
		}
	}
}
