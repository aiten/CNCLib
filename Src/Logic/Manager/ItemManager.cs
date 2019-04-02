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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Converter;
using CNCLib.Repository.Abstraction;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;

using ItemEntity = CNCLib.Repository.Abstraction.Entities.Item;

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