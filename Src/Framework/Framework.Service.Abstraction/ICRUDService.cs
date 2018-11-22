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

namespace Framework.Service.Abstraction
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICRUDService<T, TKey> : IGetService<T, TKey> where T : class
    {
        Task<TKey> Add(T value);

        Task Update(T value);

        Task Delete(T    value);
        Task Delete(TKey key);

        Task<IEnumerable<TKey>> Add(IEnumerable<T> values);

        Task Update(IEnumerable<T>    values);
        Task Delete(IEnumerable<T>    values);
        Task Delete(IEnumerable<TKey> keys);
    }
}