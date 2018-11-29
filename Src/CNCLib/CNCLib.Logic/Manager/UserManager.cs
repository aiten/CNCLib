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
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Contract;
using CNCLib.Logic.Contract.DTO;
using CNCLib.Repository.Contract;

using Framework.Logic;
using Framework.Repository.Abstraction;

using UserEntity = CNCLib.Repository.Contract.Entities.User;

namespace CNCLib.Logic.Manager
{
    public class UserManager : CRUDManager<User, int, UserEntity>, IUserManager
    {
        private readonly IUnitOfWork     _unitOfWork;
        private readonly IUserRepository _repository;
        private readonly IMapper         _mapper;

        public UserManager(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        protected override int GetKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public async Task<User> GetByName(string username)
        {
            return MapToDto(await _repository.GetByName(username));
        }
    }
}