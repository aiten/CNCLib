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

namespace Framework.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abstraction;

    using AutoMapper;

    using Repository.Abstraction;

    public abstract class GetManager<T, TKey, TEntity> : ManagerBase, IGetManager<T, TKey> where T : class where TEntity : class
    {
        private readonly IMapper                       _mapper;
        private readonly IGetRepository<TEntity, TKey> _repository;

        protected GetManager(IUnitOfWork unitOfWork, IGetRepository<TEntity, TKey> repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        protected abstract TKey GetKey(TEntity entity);

        public async Task<IEnumerable<T>> GetAll()
        {
            return MapToDto(await _repository.GetAll());
        }

        public async Task<T> Get(TKey key)
        {
            return MapToDto(await _repository.Get(key));
        }

        public async Task<IEnumerable<T>> Get(IEnumerable<TKey> keys)
        {
            return MapToDto(await _repository.Get(keys));
        }

        protected virtual T SetDto(T dto)
        {
            return dto;
        }

        protected virtual IEnumerable<T> SetDto(IEnumerable<T> dtos)
        {
            foreach (var dto in dtos)
            {
                SetDto(dto);
            }

            return dtos;
        }

        protected IEnumerable<T> MapToDto(IEnumerable<TEntity> entities)
        {
            var dtos = _mapper.Map<IEnumerable<TEntity>, IEnumerable<T>>(entities);
            return SetDto(dtos);
        }

        protected T MapToDto(TEntity entity)
        {
            var dto = _mapper.Map<TEntity, T>(entity);
            return SetDto(dto);
        }
    }
}