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

namespace Framework.Dependency
{
    using System;
    using Unity;

    /// <summary>
    /// Dependency Contaienr for use in Live. Throws an exception when a Type cannot be resolved.
    /// </summary>
    public sealed class LiveDependencyContainer : UnityDependencyContainer
    {
        public override object Resolve(Type t)
        {
            try
            {
                return MyUnityContainer.Resolve(t);
            }
            catch (ResolutionFailedException ex)
            {
                throw new ResolutionFailedException($"Resolution for {t.FullName} failed", ex);
            }
        }
    }
}