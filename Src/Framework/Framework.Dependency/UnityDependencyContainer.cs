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
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace Framework.Dependency
{
    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public abstract class UnityDependencyContainer : IDependencyContainer
    {
        private IUnityContainer _container;

        protected UnityDependencyContainer()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// Get the Unity container
        /// use the container e.g. in WebApi to use this container instead
        /// </summary>
        public IUnityContainer MyUnityContainer => _container;

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
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterType(Type typeFrom, Type typeTo)
        {
            _container.RegisterType(typeFrom, typeTo);
            return this;
        }

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterTypeScoped(Type typeFrom, Type typeTo)
        {
            _container.RegisterType(typeFrom, typeTo, new PerResolveLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers instance for a specified interface.
        /// </summary>
        /// <returns>This instance.</returns>
        public IDependencyContainer RegisterInstance(Type typeFrom, object obj)
        {
            _container.RegisterInstance(typeFrom, obj);
            return this;
        }

        public abstract object Resolve(Type t);

        /// <summary>
        /// Create unity child container
        /// </summary>
        /// <returns></returns>
        public IDependencyContainer CreateChildContainer()
        {
            var newcontainer = (UnityDependencyContainer) MemberwiseClone();
            newcontainer._container = _container.CreateChildContainer();

            return newcontainer;
        }

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        public IEnumerable<Type> RegisteredTypes { get { return _container.Registrations.Select(r => r.RegisteredType); } }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _container.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}