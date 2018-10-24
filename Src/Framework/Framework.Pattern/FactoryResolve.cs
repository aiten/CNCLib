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

using Framework.Dependency;

namespace Framework.Pattern
{
    using System;

    public sealed class ScopeResolve<T> : IScope<T>, IDisposable where T : class
    {
        private readonly IDependencyContainer _container;
        private readonly T                    _instance;

        private bool _isDisposed;

        public ScopeResolve(IDependencyContainer container, T instance)
        {
            _container = container;
            _instance  = instance;
        }

        public T Instance
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("this", "Bad person.");
                }

                return _instance;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _container.Dispose();
        }
    }

    public class FactoryResolve<T> : IFactory<T> where T : class
    {
        private readonly IDependencyContainer _container;

        public FactoryResolve()
        {
            _container = Dependency.Dependency.Container;
        }

        public IScope<T> Create()
        {
            var childContainer = _container.CreateChildContainer();
            return new ScopeResolve<T>(childContainer, childContainer.Resolve<T>());
        }
    }
}