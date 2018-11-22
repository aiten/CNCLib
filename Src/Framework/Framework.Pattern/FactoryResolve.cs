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

using Framework.Dependency.Abstraction;

namespace Framework.Pattern
{
    using System;

    using Framework.Dependency;

    // Factory/Scope using Resolve of dependencyInjection

    public sealed class ScopeResolve<T> : IScope<T>, IDisposable where T : class
    {
        private readonly IDependencyScope     _scope;
        private readonly T                    _instance;

        private bool _isDisposed;

        public ScopeResolve(IDependencyScope scope, T instance)
        {
            _scope = scope;
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
            _scope.Dispose();
        }
    }

    public class FactoryResolve<T> : IFactory<T> where T : class
    {
        private readonly IDependencyContainer _container;

        public FactoryResolve()
        {
            _container = Dependency.Container;
        }

        public IScope<T> Create()
        {
            var childContainer = _container.GetResolver().CreateScope();
            return new ScopeResolve<T>(childContainer, childContainer.Resolver.Resolve<T>());
        }
    }
}