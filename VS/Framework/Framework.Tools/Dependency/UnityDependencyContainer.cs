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
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Framework.Tools.Dependency
{
    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public abstract class UnityDependencyContainer : IDependencyContainer
    {
        protected UnityContainer _container;

        protected UnityDependencyContainer()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// Get the Unity container
        /// use the container e.g. in WebApi to use this container instead
        /// </summary>
        public UnityContainer MyUnityContainer { get { return _container;  } }

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
        /// Registers a type for the given interface.
        /// </summary>
        /// <typeparam name="typeFrom">Interface that can be later resolved.</typeparam>
        /// <typeparam name="typeTo">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>

        public IDependencyContainer RegisterType(Type typeFrom, Type typeTo)
        {
            _container.RegisterType(typeFrom,typeTo);
            return this;
        }

        /// <summary>
        /// Registers instance for a specified interface.
        /// </summary>
        /// <typeparam name="typeFrom">Interface that can be later resolved.</typeparam>
        /// <typeparam name="obj">intance of object</typeparam>
        /// <returns>This instance.</returns>

        public IDependencyContainer RegisterInstance(Type typeFrom, object obj)
        {
            _container.RegisterInstance(typeFrom, obj);
            return this;
        }

        /*
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
        */
        /*
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
        */
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
