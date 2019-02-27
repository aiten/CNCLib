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

namespace Framework.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Logic.Abstraction;

    public abstract class CRUDService<T, TKey> : GetService<T, TKey> where T : class
    {
        private readonly ICRUDManager<T, TKey> _manager;

        protected CRUDService(ICRUDManager<T, TKey> manager) : base(manager)
        {
            _manager = manager ?? throw new ArgumentNullException();
        }

        public async Task<TKey> Add(T value)
        {
            return await _manager.Add(value);
        }

        public async Task<IEnumerable<TKey>> Add(IEnumerable<T> values)
        {
            return await _manager.Add(values);
        }

        public async Task Delete(T value)
        {
            await _manager.Delete(value);
        }

        public async Task Delete(IEnumerable<T> values)
        {
            await _manager.Delete(values);
        }

        public async Task Delete(TKey key)
        {
            await _manager.Delete(key);
        }

        public async Task Delete(IEnumerable<TKey> keys)
        {
            await _manager.Delete(keys);
        }

        public async Task Update(T value)
        {
            await _manager.Update(value);
        }

        public async Task Update(IEnumerable<T> values)
        {
            await _manager.Update(values);
        }
    }
}