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
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Converter;
using CNCLib.Repository.Contracts;
using Framework.Contracts.Repository;
using Framework.Logic;

namespace CNCLib.Logic.Manager
{
    public class ItemManager : CRUDManager<Item, int, Repository.Contracts.Entities.Item>, IItemManager
    {
        private          IUnitOfWork     _unitOfWork;
        private readonly IItemRepository _repository;
        private readonly IMapper         _mapper;

        public ItemManager(IUnitOfWork unitOfWork, IItemRepository repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        protected override int GetKey(Repository.Contracts.Entities.Item entity)
        {
            return entity.ItemID;
        }

        public async Task<IEnumerable<Item>> GetByClassName(string classname)
        {
            return (await _repository.Get(classname)).ToDto(_mapper);
        }
    }
}