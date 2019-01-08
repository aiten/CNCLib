////////////////////////////////////////////////////////
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