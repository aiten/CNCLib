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

using Microsoft.Extensions.DependencyInjection;

namespace Framework.Test.Dependency
{
    using System;

    using Framework.Dependency;

    using NSubstitute;

    public sealed class MockingDependencyContainer : MsDependencyContainer
    {
        public MockingDependencyContainer(IServiceCollection services) : base(services)
        {
        }

        public override object Resolve(Type t)
        {
/*
            if (!t.IsInterface)
            {
                throw new ResolutionFailedException($"Tried to resolve type {t.FullName}. This is not an interface which indicates a bug.");
            }
*/
            try
            {
                return base.Resolve(t);
            }
            catch (Unity.Exceptions.ResolutionFailedException)
            {
                return Substitute.For(new[] { t }, new object[0]);
            }
        }
    }
}