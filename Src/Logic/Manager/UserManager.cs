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
using System.Buffers.Text;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Repository.Abstraction;

using Framework.Logic;
using Framework.Repository.Abstraction;
using Framework.Tools;

using UserEntity = CNCLib.Repository.Abstraction.Entities.User;

namespace CNCLib.Logic.Manager
{
    public class UserManager : CRUDManager<User, int, UserEntity>, IUserManager
    {
        private readonly IUnitOfWork     _unitOfWork;
        private readonly IUserRepository _repository;
        private readonly IMapper         _mapper;

        public UserManager(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _mapper     = mapper;
        }

        protected override int GetKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public static string DecodePassword(string encodedPassword)
        {
            return Base64Helper.StringFromBase64(encodedPassword);
        }

        public static string EncodePassword(string password)
        {
            return Base64Helper.StringToBase64(password);
        }

        protected override void AddEntity(UserEntity entity)
        {
            base.AddEntity(entity);
            if (string.IsNullOrEmpty(entity.Password))
            {
                entity.Password = EncodePassword(entity.Name);
            }
        }

        public async Task<User> GetByName(string username)
        {
            return MapToDto(await _repository.GetByName(username));
        }

        public async Task<int?> Authenticate(string userName, string password)
        {
            var userEntity = await _repository.GetByName(userName);

            if (userEntity != null && ComparePassword(password, DecodePassword(userEntity.Password)))
            {
                return userEntity.UserId;
            }

            return null;
        }

        private bool ComparePassword(string pwd1, string pwd2)
        {
            return pwd1 == pwd2 || (string.IsNullOrEmpty(pwd1) && string.IsNullOrEmpty(pwd2));
        }
    }
}