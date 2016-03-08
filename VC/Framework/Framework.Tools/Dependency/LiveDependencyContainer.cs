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

namespace Framework.Tools.Dependency
{
    /// <summary>
    /// Dependency Contaienr for use in Live. Throws an exception when a Type cannot be resolved.
    /// </summary>
    public sealed class LiveDependencyContainer : BaseDependencyContainer
    {
        public override object Resolve(Type t)
        {
            if (!t.IsInterface)
            {
                throw new ResolutionFailedException(string.Format("Tried to resolve type {0}. This is not an interface which indicates a bug.", t.FullName));
            }
            try
            {
                return _container.Resolve(t);
            }
            catch (Microsoft.Practices.Unity.ResolutionFailedException ex)
            {
                throw new ResolutionFailedException(string.Format("Resolution for {0} failed", t.FullName), ex);
            }
        }
    }
}