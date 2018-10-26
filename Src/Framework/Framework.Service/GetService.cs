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

namespace Framework.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Framework.Contracts.Logic;

    public abstract class GetService<T, TKey> : ServiceBase where T : class
    {
        private readonly ICRUDManager<T, TKey> _manager;

        protected GetService(ICRUDManager<T, TKey> manager)
        {
            _manager = manager ?? throw new ArgumentNullException();
        }

        public async Task<T> Get(TKey id)
        {
            return await _manager.Get(id);
        }

        public async Task<IEnumerable<T>> Get(IEnumerable<TKey> ids)
        {
            return await _manager.Get(ids);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _manager.GetAll();
        }
    }
}