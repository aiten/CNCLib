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
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Repository.Contracts;
using CNCLib.Shared;
using Framework.Contracts.Repository;
using Framework.Logic;

using ConfigurationEntity = CNCLib.Repository.Contracts.Entities.Configuration;
using MachineEntity = CNCLib.Repository.Contracts.Entities.Machine;

namespace CNCLib.Logic.Manager
{
    public class MachineManager : CRUDManager<Machine, int, MachineEntity>, IMachineManager
    {
        private readonly IUnitOfWork              _unitOfWork;
        private readonly IMachineRepository       _repository;
        private readonly IConfigurationRepository _repositoryConfig;
        private readonly ICNCLibUserContext       _userContext;
        private readonly IMapper                  _mapper;

        public MachineManager(IUnitOfWork unitOfWork, IMachineRepository repository, IConfigurationRepository repositoryConfig, ICNCLibUserContext userContext, IMapper mapper) :
            base(unitOfWork, repository, mapper)
        {
            _unitOfWork       = unitOfWork ?? throw new ArgumentNullException();
            _repository       = repository ?? throw new ArgumentNullException();
            _repositoryConfig = repositoryConfig ?? throw new ArgumentNullException();
            _userContext      = userContext ?? throw new ArgumentNullException();
            _mapper           = mapper ?? throw new ArgumentNullException();
        }

        protected override int GetKey(MachineEntity entity)
        {
            return entity.MachineID;
        }

        protected override void AddEntity(MachineEntity entityInDb)
        {
            if (_userContext.UserID.HasValue)
            {
                entityInDb.UserID = _userContext.UserID;
            }
            base.AddEntity(entityInDb);
        }

        protected override void UpdateEntity(MachineEntity entityInDb, MachineEntity values)
        {
            // do not overwrite user!

            values.UserID = entityInDb.UserID;
            values.User = entityInDb.User;

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

        public async Task<int> GetDetaultMachine()
        {
            var config = await _repositoryConfig.Get("Environment", "DefaultMachineID");

            if (config == default(ConfigurationEntity))
            {
                return -1;
            }

            return int.Parse(config.Value);
        }

        public async Task SetDetaultMachine(int defaultMachineID)
        {
            await _repositoryConfig.Store(new ConfigurationEntity
            {
                Group = "Environment",
                Name  = "DefaultMachineID",
                Type  = "Int32",
                Value = defaultMachineID.ToString()
            });
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}