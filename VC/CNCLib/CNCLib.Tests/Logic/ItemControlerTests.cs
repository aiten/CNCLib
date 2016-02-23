////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNCLib.Repository.Context;
using CNCLib.Repository;
using CNCLib.Repository.Contracts.Entities;
using CNCLib.Repository.Contracts;
using CNCLib.Logic;
using Framework.EF;
using Framework.Logic;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Framework.Tools.Pattern;
using Framework.Test;
using Framework.Tools.Dependency;

namespace CNCLib.Tests.Logic
{
	[TestClass]
	public class ItemControlerTests : UnitTestBase
    {
		/*
				[ClassInitialize]
				public static void ClassInit(TestContext testContext)
				{
				}

				[TestInitialize]
				public void Init()
				{
				}
		*/
/*
		private FactoryType2Obj CreateMock()
		{
			var mockfactory = new FactoryType2Obj();
			ControlerBase.RepositoryFactory = mockfactory;
			return mockfactory;
        }
*/
		private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
//			var mockfactory = CreateMock();
			TInterface rep = Substitute.For<TInterface>();
//            mockfactory.Register(typeof(TInterface), rep);

            Dependency.Container.RegisterInstance(rep);
            return rep;
		}


		[TestMethod]
		public void GetMachinesNone()
		{
			var rep = CreateMock<IItemRepository>();

			var itemEntity = new Item[0];
			rep.Get().Returns(itemEntity);

			ItemControler ctrl = new ItemControler();

			var all = ctrl.GetAll().ToArray();
			Assert.AreEqual(true, all.Length == 0);
		}

        [TestMethod]
        public void CreateObject()
        {
            var rep = CreateMock<IItemRepository>();

            Item itemEntity = CreateItem();

            rep.Get(1).Returns(itemEntity);

            ItemControler ctrl = new ItemControler();

            var item = ctrl.Create(1);
            Assert.IsNotNull(item);

            ItemControlerTestClass item2 = (ItemControlerTestClass)item;

            Assert.AreEqual("Hallo", item2.StringProperty);
            Assert.AreEqual(1, item2.IntProperty);
            Assert.AreEqual(1, item2.IntProperty);
            Assert.AreEqual(1.234, item2.DoubleProperty);
            Assert.AreEqual(1.234, item2.DoubleNullProperty);
            Assert.AreEqual(9.876m, item2.DecimalProperty);
            Assert.AreEqual(9.876m, item2.DecimalNullProperty);
        }

        private static Item CreateItem()
        {
            return new Item
            {
                ItemID = 1,
                Name = "Hallo",
                ClassName = typeof(ItemControlerTestClass).AssemblyQualifiedName,
                ItemProperties = new[]
                            {
                                new ItemProperty{ ItemID = 1, Name = "StringProperty", Value = "Hallo" },
                                new ItemProperty{ ItemID = 1, Name = "IntProperty", Value = "1" },
                                new ItemProperty{ ItemID = 1, Name = "DoubleProperty",  Value = "1.234" },
                                new ItemProperty{ ItemID = 1, Name = "DecimalProperty", Value = "9.876" },
                                new ItemProperty{ ItemID = 1, Name = "IntNullProperty" },
                                new ItemProperty{ ItemID = 1, Name = "DoubleNullProperty",  Value = "1.234" },
                                new ItemProperty{ ItemID = 1, Name = "DecimalNullProperty", Value = "9.876" }
                            }
            };
        }

        [TestMethod]
        public void AddObject()
        {
            var rep = CreateMock<IItemRepository>();

            Item itemEntity = CreateItem();

            ItemControlerTestClass obj = new ItemControlerTestClass()
            {
                StringProperty = "Hallo",
                IntProperty = 1,
                DoubleProperty = 1.234,
                DoubleNullProperty = 1.234,
                DecimalProperty = 9.876m,
                DecimalNullProperty = 9.876m
            };

            ItemControler ctrl = new ItemControler();

            var id = ctrl.Add("Hallo", obj);

            rep.Received().Store(Arg.Is<Item>(x => x.Name == "Hallo"));
            rep.Received().Store(Arg.Is<Item>(x => x.ItemID == 0));
            rep.Received().Store(Arg.Is<Item>(x => x.ItemProperties.Count == 7));
            rep.Received().Store(Arg.Is<Item>(x => x.ItemProperties.Where(y => y.Name == "StringProperty").FirstOrDefault().Value == "Hallo" ));
            rep.Received().Store(Arg.Is<Item>(x => x.ItemProperties.Where(y => y.Name == "DoubleProperty").FirstOrDefault().Value == "1.234"));
            rep.Received().Store(Arg.Is<Item>(x => x.ItemProperties.Where(y => y.Name == "DecimalNullProperty").FirstOrDefault().Value == "9.876"));
        }

        [TestMethod]
        public void DeleteItem()
        {
            // arrange

            var rep = CreateMock<IItemRepository>();

            Item itemEntity = CreateItem();
            rep.Get(1).Returns(itemEntity);

            ItemControler ctrl = new ItemControler();

            //act

            ctrl.Delete(1);

            //assert
            rep.Received().Get(1);
            rep.Received().Delete(itemEntity);
        }

        [TestMethod]
        public void DeleteItemNone()
        {
            // arrange

            var rep = CreateMock<IItemRepository>();

            ItemControler ctrl = new ItemControler();

            //act

            ctrl.Delete(1);

            //assert
            rep.Received().Get(1);
            rep.DidNotReceiveWithAnyArgs().Delete(null);
        }
    }
}
