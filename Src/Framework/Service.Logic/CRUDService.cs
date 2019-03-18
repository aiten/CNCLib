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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Framework.Logic.Abstraction;

namespace Framework.Service.Logic
{
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