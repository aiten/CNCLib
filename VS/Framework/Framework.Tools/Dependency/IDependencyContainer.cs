////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Reflection;

namespace Framework.Tools.Dependency
{
    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Registers public and internal types of the given assemblies with the unity container. This is necessary
        /// to workaround the internalsvisibleto hacks in the code base.
        /// </summary>
        /// <param name="assemblies">List of assemblies in which all types should be registered with their interfaces. 
        /// This includes internal types. </param>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterTypesIncludingInternals(params Assembly[] assemblies);
        
        /// <summary>
        /// Registers the given object for the interface. 
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <param name="obj">Object that should be returned for Resolve&lt;TInterface&gt;() calls.</param>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterInstance<TInterface>(TInterface obj);
        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterType<TInterface, TType>() where TType : TInterface;
        
        /// <summary>
        /// Registers a type for the given interface as a singleton. This means only one instance of this
        /// type will ever be craeted.
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() always the same instance is returned.</typeparam>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterTypeAsSingleton<TInterface, TType>() where TType : TInterface;

        /// <summary>
        /// Registers a type for the given interface using the parameter-less constructor.
        /// 
        /// Usually unity uses the public constructor with the most parameters (and tries to resolve these parameters). 
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterTypeWithDefaultConstructor<TInterface, TType>() where TType : TInterface;

        /// <summary>
        /// This can be called in unit tests to reset the container to an empty state. 
        /// 
        /// NOTE: After calling this you should call the module's DependencyInitializer again!
        /// </summary>
        void ResetContainer();

        /// <summary>
        /// Resolve an instance of the default requested type from the container. 
        /// </summary>
        /// <param name="t">Type for which a specific instance should be resolved</param>
        /// <returns>An object that implements type t.</returns>
        /// <exception cref="ResolutionFailedException">Thrown when no type was registered for the given interface.</exception>
        object Resolve(Type t);

        /// <summary>
        /// Resolves the interface to a specific type that was registered earlier. 
        /// </summary>
        /// <typeparam name="TInterface">Interface for which the registered type is looked up.</typeparam>
        /// <returns>An instance of the interface that was registered with the container earlier.</returns>
        /// <exception cref="ResolutionFailedException">Thrown when no type was registered for the given interface.</exception>
        TInterface Resolve<TInterface>();

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        IEnumerable<Type> RegisteredTypes { get; }
    }
}