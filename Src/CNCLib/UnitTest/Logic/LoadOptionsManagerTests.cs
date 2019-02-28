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

using System;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Logic.Client;
using CNCLib.Logic.Contract.DTO;
using CNCLib.Logic.Manager;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace CNCLib.Test.Logic
{
    public class LoadOptionsManagerTests : LogicTests
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
            var rep = Substitute.For<TInterface>();
            return rep;
        }

        [Fact]
        public async Task GetAllLoadOptions()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            rep.GetAll(typeof(LoadOptions)).Returns(
                new[]
                {
                    new DynItem { ItemId = 1, Name = "Entry1" }
                });
            rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

            var all = await ctrl.GetAll();

            all.Count().Should().Be(1);
            all.FirstOrDefault().Id.Should().Be(1);

            all.FirstOrDefault().SettingName.Should().Be("Entry1");
            all.FirstOrDefault().FileName.Should().Be("HA");
        }

        [Fact]
        public async Task GetLoadOptions()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

            var all = await ctrl.Get(1);

            all.Id.Should().Be(1);
            all.SettingName.Should().Be("Entry1");
            all.FileName.Should().Be("HA");
        }

        [Fact]
        public async Task GetLoadOptionsNull()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            rep.Create(1).Returns(new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" });

            var all = await ctrl.Get(2);

            all.Should().BeNull();
        }

        [Fact]
        public async Task AddLoadOptions()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

            await ctrl.Add(opt);

            await rep.Received().Add(Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
        }

        [Fact]
        public async Task UpdateLoadOptions()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

            await ctrl.Update(opt);

            await rep.Received().Save(Arg.Is<int>(x => x == 1), Arg.Is<string>(x => x == "Entry1"), Arg.Is<LoadOptions>(x => x.SettingName == "Entry1" && x.FileName == "HA"));
        }

        [Fact]
        public async Task DeleteLoadOptions()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new LoadOptionsManager(rep);

            var opt = new LoadOptions { SettingName = "Entry1", Id = 1, FileName = "HA" };

            await ctrl.Delete(opt).ConfigureAwait(false);

            await rep.Received().Delete(Arg.Is<int>(x => x == 1));
        }
    }
}