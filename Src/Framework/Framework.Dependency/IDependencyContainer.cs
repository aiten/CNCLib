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
using System.Collections.Generic;

namespace Framework.Dependency
{
    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public interface IDependencyContainer : IDisposable
    {
        /// <summary>
        /// Registers the given object for the interface. 
        /// </summary>
        /// <param name="typeFrom"></param>
        /// <param name="obj">Object that should be returned for Resolve&lt;TInterface&gt;() calls.</param>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterInstance(Type typeFrom, object obj);

        /// <summary>
        /// Register typefrom to typeTo 
        /// </summary>
        /// <param name="typeFrom"></param>
        /// <param name="typeTo"></param>
        /// <returns></returns>
        IDependencyContainer RegisterType(Type typeFrom, Type typeTo);

        /// <summary>
        /// Register typefrom to typeTo 
        /// </summary>
        /// <param name="typeFrom"></param>
        /// <param name="typeTo"></param>
        /// <returns></returns>
        IDependencyContainer RegisterTypeScoped(Type typeFrom, Type typeTo);


        /// <summary>
        /// This can be called in unit tests to reset the container to an empty state. 
        /// 
        /// NOTE: After calling this you should call the module's DependencyInitializer again!
        /// </summary>
        void ResetContainer();

        /// <summary>
        /// Create a ChildContainer => see unity CreateChildContainer()
        /// </summary>
        /// <returns></returns>
        IDependencyContainer CreateChildContainer();

        /// <summary>
        /// Resolve an instance of the default requested type from the container. 
        /// </summary>
        /// <param name="t">Type for which a specific instance should be resolved</param>
        /// <returns>An object that implements type t.</returns>
        /// <exception cref="ResolutionFailedException">Thrown when no type was registered for the given interface.</exception>
        object Resolve(Type t);

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        IEnumerable<Type> RegisteredTypes { get; }
    }
}