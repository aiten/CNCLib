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

using CNCLib.Logic.Contract;
using CNCLib.Logic.Contract.DTO;
using CNCLib.Logic.Converter;
using CNCLib.Repository.Contract;
using CNCLib.Shared;

using Framework.Contract.Repository;
using Framework.Logic;

using ItemEntity = CNCLib.Repository.Contract.Entities.Item;

namespace CNCLib.Logic.Manager
{
    public class ItemManager : CRUDManager<Item, int, ItemEntity>, IItemManager
    {
        private readonly IUnitOfWork        _unitOfWork;
        private readonly IItemRepository    _repository;
        private readonly ICNCLibUserContext _userContext;
        private readonly IMapper            _mapper;

        public ItemManager(IUnitOfWork unitOfWork, IItemRepository repository, ICNCLibUserContext userContext, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork  = unitOfWork ?? throw new ArgumentNullException();
            _repository  = repository ?? throw new ArgumentNullException();
            _userContext = userContext ?? throw new ArgumentNullException();
            _mapper      = mapper ?? throw new ArgumentNullException();
        }

        protected override int GetKey(ItemEntity entity)
        {
            return entity.ItemId;
        }

        protected override void AddEntity(ItemEntity entityInDb)
        {
            if (_userContext.UserId.HasValue)
            {
                entityInDb.UserId = _userContext.UserId;
            }

            base.AddEntity(entityInDb);
        }

        protected override void UpdateEntity(ItemEntity entityInDb, ItemEntity values)
        {
            // do not overwrite user!

            values.UserId = entityInDb.UserId;
            values.User   = entityInDb.User;

            base.UpdateEntity(entityInDb, values);
        }

        public async Task<IEnumerable<Item>> GetByClassName(string classname)
        {
            return (await _repository.Get(classname)).ToDto(_mapper);
        }
    }
}