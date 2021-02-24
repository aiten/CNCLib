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

namespace CNCLib.Logic.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using AutoMapper;

    using CNCLib.Logic.Abstraction;
    using CNCLib.Logic.Abstraction.DTO;
    using CNCLib.Logic.Statistics;
    using CNCLib.Repository.Abstraction;
    using CNCLib.Repository.Abstraction.Entities;
    using CNCLib.Shared;

    using Framework.Logic;
    using Framework.Repository.Abstraction;
    using Framework.Tools.Abstraction;
    using Framework.Tools.Password;

    public class UserManager : CrudManager<User, int, UserEntity>, IUserManager
    {
        private readonly IUnitOfWork         _unitOfWork;
        private readonly IUserRepository     _repository;
        private readonly IMachineRepository  _machineRepository;
        private readonly IItemRepository     _itemRepository;
        private readonly IUserFileRepository _userFileRepository;
        private readonly IInitRepository     _initRepository;

        private readonly ICurrentDateTime   _currentDate;
        private readonly IMapper            _mapper;
        private readonly ICNCLibUserContext _userContext;

        private readonly IOneWayPasswordProvider _passwordProvider;
        private readonly CallStatisticCache      _callStatisticCache;

        public const int GlobalUserId = 1;

        public UserManager(IUnitOfWork unitOfWork,
            IUserRepository            repository,
            IMachineRepository         machineRepository,
            IItemRepository            itemRepository,
            IUserFileRepository        userFileRepository,
            IInitRepository            initRepository,
            ICNCLibUserContext         userContext,
            ICurrentDateTime           currentDate,
            IMapper                    mapper,
            IOneWayPasswordProvider    passwordProvider,
            CallStatisticCache         callStatisticCache) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork         = unitOfWork;
            _repository         = repository;
            _machineRepository  = machineRepository;
            _itemRepository     = itemRepository;
            _userFileRepository = userFileRepository;
            _initRepository     = initRepository;
            _userContext        = userContext;
            _currentDate        = currentDate;
            _mapper             = mapper;
            _passwordProvider   = passwordProvider;
            _callStatisticCache = callStatisticCache;
        }

        protected override int GetKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public async Task<User> GetByName(string username)
        {
            return await MapToDto(await _repository.GetByName(username));
        }

        public async Task<ClaimsPrincipal> Authenticate(string userName, string password)
        {
            var userEntity = await _repository.GetByName(userName);

            if (!string.IsNullOrEmpty(password) && userEntity != null && _passwordProvider.ValidatePassword(password, userEntity.Password))
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, userEntity.UserId.ToString()),
                    new Claim(ClaimTypes.Name,           userName),
                };

                if (userName == CNCLibConst.AdminUser)
                {
                    claims.Add(new Claim(CNCLibClaimTypes.IsAdmin, "true"));
                }

                var identity  = new ClaimsIdentity(claims, "BasicAuthentication");
                var principal = new ClaimsPrincipal(identity);

                _callStatisticCache.AddCall(new CallStatistic()
                {
                    CallTime = _currentDate.Now,
                    UserId   = userEntity.UserId
                });

                return principal;
            }

            return null;
        }

        public async Task<string> Register(string userName, string password)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var userEntity = await _repository.GetByName(userName);

                if (userEntity != null)
                {
                    throw new Exception("user already exists");
                }

                userEntity = new UserEntity()
                {
                    Name     = userName,
                    Password = _passwordProvider.GetPasswordHash(password),
                    Created  = _currentDate.Now
                };

                _repository.Add(userEntity);

                await trans.SaveChangesAsync();

                await _initRepository.Initialize(userEntity.UserId);

                await CommitTransaction(trans);

                return userEntity.UserId.ToString();
            }
        }

        public async Task ChangePassword(string userName, string passwordOld, string passwordNew)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var authUser = Authenticate(userName, passwordOld);

                if (authUser == null)
                {
                    throw new AuthenticationException();
                }

                var userEntity = await _repository.GetByNameTracking(userName);
                userEntity.Password = _passwordProvider.GetPasswordHash(passwordNew);

                await CommitTransaction(trans);
            }
        }

        public async Task<string> CreatePasswordHash(string password)
        {
            await Task.Delay(100);
            return await Task.FromResult(_passwordProvider.GetPasswordHash(password));
        }

        public async Task InitData()
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await _initRepository.Initialize(_userContext.UserId);

                await CommitTransaction(trans);
            }
        }

        public async Task Cleanup()
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await DeleteData(_userContext.UserId);

                await CommitTransaction(trans);
            }
        }

        private async Task DeleteData(int userId)
        {
            await _machineRepository.DeleteByUser(userId);
            await _itemRepository.DeleteByUser(userId);
            await _userFileRepository.DeleteByUser(userId);
        }

        public async Task Leave()
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await DeleteData(_userContext.UserId);

                _repository.Delete(await _repository.GetTracking(_userContext.UserId));

                await CommitTransaction(trans);
            }
        }

        public async Task InitMachines()
        {
            var defaultMachines = _initRepository.GetDefaultMachines();

            using (var trans = _unitOfWork.BeginTransaction())
            {
                var userMachines = await _machineRepository.GetTracking(await _machineRepository.GetIdByUser(_userContext.UserId));

                var sameMachines = userMachines.Join(defaultMachines,
                    m => m.Name.ToLower(),
                    m => m.Name.ToLower(),
                    (m, _) => m);

                _machineRepository.DeleteRange(sameMachines);

                await _unitOfWork.SaveChangesAsync();
                await _initRepository.AddDefaultMachines(_userContext.UserId);
                await CommitTransaction(trans);
            }
        }

        public async Task InitItems()
        {
            var defaultItems = _initRepository.GetDefaultItems();
            var defaultFiles = _initRepository.GetDefaultFiles();

            using (var trans = _unitOfWork.BeginTransaction())
            {
                var userItems = await _itemRepository.GetTracking(await _itemRepository.GetIdByUser(_userContext.UserId));

                var sameItems = userItems.Join(defaultItems,
                    item => item.Name.ToLower(),
                    item => item.Name.ToLower(),
                    (item, _) => item);

                _itemRepository.DeleteRange(sameItems);

                var userFiles = await _userFileRepository.GetTracking(await _userFileRepository.GetIdByUser(_userContext.UserId));

                var sameFiles = userFiles.Join(defaultFiles,
                    f => f.FileName.ToLower(),
                    f => f.FileName.ToLower(),
                    (r, _) => r);

                _userFileRepository.DeleteRange(sameFiles);

                await _unitOfWork.SaveChangesAsync();
                await _initRepository.AddDefaultItems(_userContext.UserId);
                await _initRepository.AddDefaultFiles(_userContext.UserId);
                await CommitTransaction(trans);
            }
        }
    }
}