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
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Manager;

using FluentAssertions;

using NSubstitute;

using Xunit;

public class EepromConfigManagerTests : LogicTests
{
    private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
    {
        var rep = Substitute.For<TInterface>();
        return rep;
    }

    [Fact]
    public async Task CalculateMaxStepRate()
    {
        // no repository is needed
        //var rep = CreateMock<IItemXXX>();
        //    
        //var itemEntity = new ItemEntity[0];
        //rep.GetAsync().Returns(itemEntity);

        var ctrl = new EepromConfigurationManager();

        var input = new EepromConfigurationInput
        {
            Teeth                  = 15,
            ToothSizeInMm          = 2.0,
            MicroSteps             = 16,
            StepsPerRotation       = 200,
            EstimatedRotationSpeed = 7.8,
            TimeToAcc              = 0.2,
            TimeToDec              = 0.15
        };
        var result = await ctrl.CalculateConfigAsync(input);
        result.Should().NotBeNull();
        result.StepsPerRotation.Should().Be(3200);
        result.StepsPerMm.Should().Be(3200.0 / (15.0 * 2.0));
        result.Acc.Should().Be(375);
        result.Dec.Should().Be(433);
        result.MaxStepRate.Should().Be(24960);
    }
}