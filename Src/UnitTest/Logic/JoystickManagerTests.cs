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

namespace CNCLib.UnitTest.Logic
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CNCLib.Logic.Abstraction.DTO;
    using CNCLib.Logic.Client;
    using CNCLib.Logic.Manager;

    using FluentAssertions;

    using NSubstitute;

    using Xunit;

    public class JoystickManagerTests : LogicTests
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
            var rep = Substitute.For<TInterface>();
            return rep;
        }

        [Fact]
        public async Task GetAllJoysticks()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            rep.GetAll(typeof(Joystick)).Returns(
                new[]
                {
                    new DynItem { ItemId = 1, Name = "Entry1" }
                });
            rep.Create(1).Returns(new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" });

            var all = (await ctrl.GetAll()).ToList();

            all.Should().HaveCount(1);

            var first = all.First();

            first.Id.Should().Be(1);
            first.SerialServer.Should().Be("Entry1");
            first.SerialServerUser.Should().Be("HA");
        }

        [Fact]
        public async Task GetJoystick()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            rep.Create(1).Returns(new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" });

            var all = await ctrl.Get(1);

            all.Id.Should().Be(1);
            all.SerialServer.Should().Be("Entry1");
            all.SerialServerUser.Should().Be("HA");
        }

        [Fact]
        public async Task GetJoystickNull()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            rep.Create(1).Returns(new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" });

            var all = await ctrl.Get(2);

            all.Should().BeNull();
        }

        [Fact]
        public async Task AddJoystick()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            var joystick = new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" };

            await ctrl.Add(joystick);

            await rep.Received().Add(Arg.Is<string>(x => x == "Joystick1"), Arg.Is<Joystick>(x => x.SerialServer == "Entry1" && x.SerialServerUser == "HA"));
        }

        [Fact]
        public async Task UpdateJoystick()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            var joystick = new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" };

            await ctrl.Update(joystick);

            await rep.Received().Save(Arg.Is<int>(x => x == 1), Arg.Is<string>(x => x == "Joystick1"), Arg.Is<Joystick>(x => x.SerialServer == "Entry1" && x.SerialServerUser == "HA"));
        }

        [Fact]
        public async Task DeleteJoystick()
        {
            var rep  = CreateMock<IDynItemController>();
            var ctrl = new JoystickManager(rep);

            var joystick = new Joystick { SerialServer = "Entry1", Id = 1, SerialServerUser = "HA" };

            await ctrl.Delete(joystick).ConfigureAwait(false);

            await rep.Received().Delete(Arg.Is<int>(x => x == 1));
        }
    }
}