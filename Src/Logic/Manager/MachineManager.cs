﻿/*
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

namespace CNCLib.Logic.Manager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.GCode.Machine;
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;

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

    protected override Task<IList<MachineEntity>> GetAllEntitiesAsync()
    {
        return _repository.GetByUserAsync(_userContext.UserId);
    }

    protected override void AddEntity(MachineEntity entityInDb)
    {
        entityInDb.UserId = _userContext.UserId;
        base.AddEntity(entityInDb);
    }

    protected override async Task UpdateEntityAsync(MachineEntity entityInDb, MachineEntity values)
    {
        // do not overwrite user!

        values.UserId = entityInDb.UserId;
        values.User   = entityInDb.User;

        await base.UpdateEntityAsync(entityInDb, values);
    }

    #region DefaultAsync machine

    public async Task<Machine> DefaultAsync()
    {
        var dto = new Machine
        {
            Name                = "New",
            ComPort             = "comX",
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
            MachineCommands     = Array.Empty<MachineCommand>(),
            MachineInitCommands = Array.Empty<MachineInitCommand>()
        };
        return await Task.FromResult(dto);
    }

    public async Task<int> GetDefaultAsync()
    {
        var config = await _repositoryConfig.GetAsync(_userContext.UserId, "Environment", "DefaultMachineId");

        if (config == null)
        {
            return -1;
        }

        return int.Parse(config.Value!);
    }

    public async Task SetDefaultAsync(int defaultMachineId)
    {
        await _repositoryConfig.StoreAsync(
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

    #region joystick

    public async Task<string> TranslateJoystickMessageAsync(int machineId, string joystickMessage)
    {
        var m = await GetAsync(machineId);
        return m != null ? TranslateJoystickMessage(m, joystickMessage) : String.Empty;
    }

    public string TranslateJoystickMessage(Machine machine, string joystickMessage)
    {
        // ;btn5		=> look for ;btn5
        // ;btn5:x		=> x is pressCount - always incremented, look for max x in setting => modulo 

        int idx;

        if ((idx = joystickMessage.IndexOf(':')) < 0)
        {
            var mc = machine.MachineCommands!.FirstOrDefault(m => m.JoystickMessage == joystickMessage);
            if (mc != null)
            {
                joystickMessage = mc.CommandString;
            }
        }
        else
        {
            var btn             = joystickMessage.Substring(0, idx + 1);
            var machineCommands = machine.MachineCommands!.Where(m => m.JoystickMessage?.Length > idx && m.JoystickMessage.Substring(0, idx + 1) == btn).ToList();

            uint max = 0;
            foreach (var m in machineCommands)
            {
                uint val;
                if (uint.TryParse(m.JoystickMessage!.Substring(idx + 1), out val))
                {
                    if (val > max)
                    {
                        max = val;
                    }
                }
            }

            var findCmd = $"{btn}{uint.Parse(joystickMessage.Substring(idx + 1)) % (max + 1)}";

            var mc = machine.MachineCommands!.FirstOrDefault(m => m.JoystickMessage == findCmd);
            if (mc == null)
            {
                // try to find ;btn3 (without :)  
                findCmd = joystickMessage.Substring(0, idx);
                mc      = machine.MachineCommands!.FirstOrDefault(m => m.JoystickMessage == findCmd);
            }

            if (mc != null)
            {
                joystickMessage = mc.CommandString;
            }
        }

        return joystickMessage;
    }

    #endregion

    #region Eeprom

    public Machine UpdateFromEeprom(Machine machine, uint[] eepromValues)
    {
        var eeprom = EepromExtensions.ConvertEeprom(eepromValues);

        if (eeprom != null)
        {
            machine.Coolant   = eeprom.HasCoolant;
            machine.Rotate    = eeprom.CanRotate;
            machine.Spindle   = eeprom.HasSpindle;
            machine.SDSupport = eeprom.HasSD;
            machine.Rotate    = eeprom.CanRotate;
            machine.Coolant   = eeprom.HasCoolant;
            machine.Laser     = eeprom.IsLaser;
            machine.Axis      = (int)eeprom.UseAxis;

            machine.SizeX = eeprom.GetAxis(0).Size / 1000m;
            machine.SizeY = eeprom.GetAxis(1).Size / 1000m;
            machine.SizeZ = eeprom.GetAxis(2).Size / 1000m;
            machine.SizeA = eeprom.GetAxis(3).Size / 1000m;

            machine.WorkOffsets   = (int)eeprom.WorkOffsetCount;
            machine.CommandSyntax = (CommandSyntax)eeprom.CommandSyntax;
        }

        return machine;
    }

    #endregion
}