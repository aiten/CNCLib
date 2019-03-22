/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
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

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _disposedValue = true;
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