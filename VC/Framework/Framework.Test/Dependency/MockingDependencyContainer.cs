////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using Microsoft.Practices.Unity;
using NSubstitute;

namespace Framework.Test.Dependency
{
    public sealed class MockingDependencyContainer : Framework.Tools.Dependency.BaseDependencyContainer
    {
        public override object Resolve(Type t)
        {
            if (!t.IsInterface)
            {
                throw new Framework.Tools.Dependency.ResolutionFailedException(string.Format("Tried to resolve type {0}. This is not an interface which indicates a bug.", t.FullName));
            }
            try
            {
                return _container.Resolve(t);
            }
            catch (Microsoft.Practices.Unity.ResolutionFailedException)
            {
                return Substitute.For(new Type[] { t }, new object[0]);
            }
        }
    }
}
