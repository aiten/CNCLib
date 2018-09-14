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
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Contracts.Repository;

namespace Framework.Logic
{
    public abstract class GetManager<T, TKey, TEntity> : ManagerBase where T : class where TEntity : class
    {
        private IMapper                       _mapper;
        private IGetRepository<TEntity, TKey> _repository;
        private IUnitOfWork                   _unitOfWork;

        protected GetManager(IUnitOfWork unitOfWork, IGetRepository<TEntity, TKey> repository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
            ;
        }

        protected abstract TKey GetKey(TEntity entity);

        public async Task<IEnumerable<T>> GetAll()
        {
            return _mapper.Map<IEnumerable<TEntity>, IEnumerable<T>>(await _repository.GetAll());
        }

        public async Task<T> Get(TKey key)
        {
            return _mapper.Map<TEntity, T>(await _repository.Get(key));
        }

        public async Task<IEnumerable<T>> Get(IEnumerable<TKey> keys)
        {
            return _mapper.Map<IEnumerable<TEntity>, IEnumerable<T>>(await _repository.Get(keys));
        }
    }
}