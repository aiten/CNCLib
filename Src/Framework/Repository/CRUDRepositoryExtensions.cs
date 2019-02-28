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