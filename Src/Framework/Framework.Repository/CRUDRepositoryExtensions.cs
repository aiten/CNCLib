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

namespace Framework.Repository
{
    using System.Data;
    using System.Threading.Tasks;

    using Abstraction;

    public static class CRUDRepositoryExtensions
    {
        public static async Task Store<TEntity, TKey>(this ICRUDRepository<TEntity, TKey> repository, TEntity entity, TKey key)
            where TEntity : class
        {
            TEntity entityInDb = await repository.GetTracking(key);
            if (entityInDb == default(TEntity))
            {
                repository.Add(entity);
            }
            else
            {
                // syn with existing
                repository.SetValue(entityInDb, entity);
            }
        }

        public static async Task Update<TEntity, TKey>(this ICRUDRepository<TEntity, TKey> repository, TKey key, TEntity values)
            where TEntity : class
        {
            TEntity entityInDb = await repository.GetTracking(key);
            if (entityInDb == default(TEntity))
            {
                throw new DBConcurrencyException();
            }

            repository.SetValueGraph(entityInDb, values);
        }
    }
}