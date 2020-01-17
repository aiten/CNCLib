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
using CNCLib.Repository.Abstraction;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;

using ConfigurationEntity = CNCLib.Repository.Abstraction.Entities.Configuration;
using MachineEntity = CNCLib.Repository.Abstraction.Entities.Machine;

namespace CNCLib.Logic.Manager
{
    public class MachineManager : CrudManager<Machine, int, MachineEntity>, IMachineManager
    {
        private readonly IUnitOfWork              _unitOfWork;
        private readonly IMachineRepository       _repository;
        private readonly IConfigurationRepository _repositoryConfig;
        private readonly ICNCLibUserContext       _userContext;
        private readonly IMapper                  _mapper;

        public MachineManager(IUnitOfWork unitOfWork, IMachineRepository repository, IConfigurationRepository repositoryConfig, ICNCLibUserContext userContext, IMapper mapper) :
            base(unitOfWork, repository, mapper)
        {
            _unitOfWork       = unitOfWork;
            _repository       = repository;
            _repositoryConfig = repositoryConfig;
            _userContext      = userContext;
            _mapper           = mapper;
        }

        protected override int GetKey(MachineEntity entity)
        {
            return entity.MachineId;
        }

        protected override Task<IList<MachineEntity>> GetAllEntities()
        {
            return _repository.GetByUser(_userContext.UserId);
        }

        protected override void AddEntity(MachineEntity entityInDb)
        {
            entityInDb.UserId = _userContext.UserId;
            base.AddEntity(entityInDb);
        }

        protected override void UpdateEntity(MachineEntity entityInDb, MachineEntity values)
        {
            // do not overwrite user!

            values.UserId = entityInDb.UserId;
            values.User   = entityInDb.User;

            base.UpdateEntity(entityInDb, values);
        }

        #region Default machine

        public async Task<Machine> DefaultMachine()
        {
            var dto = new Machine
            {
                Name                = "New",
                ComPort             = "comX",
                SerialServerPort    = 5000,
                SerialServerProtocol = "http",
                Axis                = 3,
                SizeX               = 130m,
                SizeY               = 45m,
                SizeZ               = 81m,
                SizeA               = 360m,
                SizeB               = 360m,
                SizeC               = 360m,
                BaudRate            = 115200,
                DtrIsReset          = true,
                BufferSize          = 63,
                CommandToUpper      = false,
                ProbeSizeZ          = 25,
                ProbeDist           = 10m,
                ProbeDistUp         = 3m,
                ProbeFeed           = 100m,
                SDSupport           = true,
                Spindle             = true,
                Coolant             = true,
                Rotate              = true,
                Laser               = false,
                MachineCommands     = new MachineCommand[0],
                MachineInitCommands = new MachineInitCommand[0]
            };
            return await Task.FromResult(dto);
        }

        public async Task<int> GetDefaultMachine()
        {
            var config = await _repositoryConfig.Get(_userContext.UserId, "Environment", "DefaultMachineId");

            if (config == default(ConfigurationEntity))
            {
                return -1;
            }

            return int.Parse(config.Value);
        }

        public async Task SetDefaultMachine(int defaultMachineId)
        {
            await _repositoryConfig.Store(
                new ConfigurationEntity
                {
                    UserId = _userContext.UserId,
                    Group  = "Environment",
                    Name   = "DefaultMachineId",
                    Type   = "Int32",
                    Value  = defaultMachineId.ToString()
                });
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}