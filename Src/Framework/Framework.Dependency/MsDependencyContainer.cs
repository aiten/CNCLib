////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
    using System.Linq;

    using Abstraction;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Dependency container for use in Live. Throws an exception when a Type cannot be resolved.
    /// </summary>
    public class MsDependencyContainer : IDependencyContainer
    {
        private readonly IServiceCollection _container;

        public MsDependencyContainer(IServiceCollection services)
        {
            _container = services;
        }

        public MsDependencyContainer()
        {
            _container = new ServiceCollection();
        }

        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        public IEnumerable<Type> RegisteredTypes => _container.Select(r => r.ServiceType);

        /// <summary>
        /// This can be called in unit tests to reset the container to an empty state. 
        /// NOTE: After calling this you should call the module's DependencyInitializer again.
        /// </summary>
        public void ResetContainer()
        {
            _container.Clear();
        }

        /// <inheritdoc />
        public IDependencyContainer RegisterType(Type typeFrom, Type typeTo)
        {
            _container.AddTransient(typeFrom, typeTo);
            return this;
        }

        /// <inheritdoc />
        public IDependencyContainer RegisterTypeScoped(Type typeFrom, Type typeTo)
        {
            _container.AddScoped(typeFrom, typeTo);
            return this;
        }

        /// <inheritdoc />
        public IDependencyContainer RegisterInstance(Type typeFrom, object obj)
        {
            _container.AddSingleton(typeFrom, obj);
            return this;
        }

        /// <inheritdoc />
        public IDependencyResolver GetResolver()
        {
            return new MsDependencyResolver(_container.BuildServiceProvider());
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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