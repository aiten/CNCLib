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
    using System.Collections.Generic;

    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolve an instance of the default requested type from the container. 
        /// </summary>
        /// <param name="t">Type for which a specific instance should be resolved</param>
        /// <returns>An object that implements type t.</returns>
        object Resolve(Type t);

        IDependencyScope CreateScope();
    }
}