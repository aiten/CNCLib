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
using System.Xml;

namespace Framework.Tools.Pattern
{
    public sealed class ScopeInstance<T> : IScope<T>, IDisposable where T : class
    {
        private readonly T _instance;
        private bool _isDisposed;

        public ScopeInstance(T instance)
        {
            _instance = instance;
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
        }
    }

    public class FactoryInstance<T> : IFactory<T> where T : class
    {
        public FactoryInstance(T obj)
        {
            _obj = obj;
        }

        private T _obj;

        IScope<T> IFactory<T>.Create()
        {
            return new ScopeInstance<T>(_obj);
        }
    }
}
