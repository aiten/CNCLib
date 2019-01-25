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

namespace Framework.Logic.Abstraction
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICRUDManager<T, TId> : IGetManager<T, TId> where T : class
    {
        Task<TId> Add(T value);

        Task Update(T   value);
        Task Delete(T   value);
        Task Delete(TId key);

        Task<IEnumerable<TId>> Add(IEnumerable<T> values);

        Task Update(IEnumerable<T>   values);
        Task Delete(IEnumerable<T>   values);
        Task Delete(IEnumerable<TId> keys);
    }
}