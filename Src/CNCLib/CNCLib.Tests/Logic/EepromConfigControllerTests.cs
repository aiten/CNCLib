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
using System.Threading.Tasks;
using CNCLib.Logic;
using CNCLib.Logic.Contracts.DTO;
using FluentAssertions;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CNCLib.Tests.Logic
{
    [TestClass]
    public class EepromConfigControllerTests : CNCUnitTest
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
            var rep = Substitute.For<TInterface>();
            Dependency.Container.RegisterInstance(rep);

            Dependency.Container.RegisterType<Framework.Contracts.Repository.IUnitOfWork, Framework.EF.UnitOfWork<CNCLib.Repository.Context.CNCLibContext>>();
            return rep;
        }

        [TestMethod]
        public async Task CalculateMaxStepRate()
        {
            // no repository is needed
            //var rep = CreateMock<IItemXXX>();
            //    
            //var itemEntity = new Item[0];
            //rep.Get().Returns(itemEntity);

            var ctrl = new EepromConfigurationManager();

            var input = new EepromConfigurationInput
            {
                Teeth = 15,
                ToothsizeinMm = 2.0,
                Microsteps = 16,
                StepsPerRotation = 200,
                EstimatedRotationSpeed = 7.8,
                TimeToAcc = 0.2,
                TimeToDec = 0.15
            };
            var result = await ctrl.CalculateConfig(input);
            result.Should().NotBeNull();
            result.StepsPerRotation.Should().Be(3200);
            result.StepsPerMm.Should().Be(3200.0 / (15.0 * 2.0));
            result.Acc.Should().Be(375);
            result.Dec.Should().Be(433);
            result.MaxStepRate.Should().Be(24960);
        }
    }
}
