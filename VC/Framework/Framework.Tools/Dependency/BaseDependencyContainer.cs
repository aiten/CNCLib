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
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Framework.Tools.Dependency
{
    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public abstract class BaseDependencyContainer : IDependencyContainer
    {
        protected UnityContainer _container;

        protected BaseDependencyContainer()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// This can be called in unit tests to reset the container to an empty state. 
        /// 
        /// NOTE: After calling this you should call the module's DependencyInitializer again!
        /// </summary>
        public void ResetContainer()
        {
            _container.Dispose();
            _container = new UnityContainer();
        }

        /// <summary>
        /// Registers all types in the given assemblies transiently with the interface that has the corresponding name.
        /// E.g. a type FooDA would be registered to the interface IFooDA if it exists.
        /// </summary>
        /// <param name="assemblies">List of assemblies in which all non-abstract public types should be registered with their interfaces.</param>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterTypes(params Assembly[] assemblies)
        {
            _container.RegisterTypes(AllClasses.FromAssemblies(assemblies), WithMappings.FromMatchingInterface,
                WithName.Default, WithLifetime.Transient);
            return this;
        }

        /// <summary>
        /// Registers public and internal types of the given assemblies with the unity container. This is necessary
        /// to workaround the internalsvisibleto hacks in the code base.
        /// </summary>
        /// <param name="assemblies">List of assemblies in which all types should be registered with their interfaces. 
        /// This includes internal types. </param>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterTypesIncludingInternals(params Assembly[] assemblies)
        {
            _container.RegisterTypes(GetAllTypesFromAssemblies(assemblies), WithMappings.FromMatchingInterface, 
                WithName.Default, WithLifetime.Transient);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblies">List of assemblies that should be searched for types.</param>
        /// <returns>A list of all non-abstract classes in the given assemblies.</returns>
        private static IEnumerable<Type> GetAllTypesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract));
        }

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterType<TInterface, TType>() where TType : TInterface
        {
            _container.RegisterType<TInterface, TType>();
            return this;
        }

        /// <summary>
        /// Registers a type for the given interface using the parameter-less constructor.
        /// 
        /// Usually unity uses the public constructor with the most parameters (and tries to resolve these parameters). 
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterTypeWithDefaultConstructor<TInterface, TType>() where TType : TInterface
        {
            _container.RegisterType<TInterface, TType>(new InjectionConstructor());
            return this;
        }

        /// <summary>
        /// Registers a type for the given interface as a singleton. This means only one instance of this
        /// type will ever be craeted.
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() always the same instance is returned.</typeparam>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterTypeAsSingleton<TInterface, TType>() where TType : TInterface
        {
            _container.RegisterType<TInterface, TType>(new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers the given object for the interface. 
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <param name="obj">Object that should be returned for Resolve&lt;TInterface&gt;() calls.</param>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterInstance<TInterface>(TInterface obj)
        {
            _container.RegisterInstance(obj);
            return this;
        }

        /// <summary>
        /// Resolves the interface to a specific type that was registered earlier. 
        /// </summary>
        /// <typeparam name="TInterface">Interface for which the registered type is looked up.</typeparam>
        /// <returns>An instance of the interface that was registered with the container earlier.</returns>
        /// <exception cref="ResolutionFailedException">Thrown when no type was registered for the given interface.</exception>
        public TInterface Resolve<TInterface>()
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        public abstract object Resolve(Type t);

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        public IEnumerable<Type> RegisteredTypes
        {
            get { return _container.Registrations.Select(r => r.RegisteredType); }
        }
    }
}
