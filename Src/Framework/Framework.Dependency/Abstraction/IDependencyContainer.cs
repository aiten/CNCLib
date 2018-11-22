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
        /// Register typeFrom to typeTo 
        /// </summary>
        /// <param name="typeFrom"></param>
        /// <param name="typeTo"></param>
        /// <returns></returns>
        IDependencyContainer RegisterType(Type typeFrom, Type typeTo);

        /// <summary>
        /// Register typeFrom to typeTo 
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
        /// Get an instance of an object which can resolve the "service" (object). 
        /// </summary>
        IDependencyResolver GetResolver();

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        IEnumerable<Type> RegisteredTypes { get; }
    }
}