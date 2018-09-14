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
using Microsoft.AspNetCore.Mvc;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;
using CNCLib.Shared;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class EepromConfigurationController : Controller
    {
        public EepromConfigurationController(IEepromConfigurationService eepromConfigurationService, ICNCLibUserContext usercontext)
        {
            _eepromConfigurationService = eepromConfigurationService ?? throw new ArgumentNullException();
            _usercontext                = usercontext ?? throw new ArgumentNullException();
            ((CNCLibUserContext) _usercontext).InitFromController(this);
        }

        readonly         IEepromConfigurationService _eepromConfigurationService;
        private readonly ICNCLibUserContext          _usercontext;

        [HttpGet]
        public async Task<ActionResult<EepromConfiguration>> Get(ushort teeth, double toothsizeInMm, ushort microsteps, ushort stepsPerRotation, double estimatedRotationSpeed, double timeToAcc,
                                                                 double timeToDec)
        {
            // http://localhost:2024/api/EepromConfiguration?teeth=15&toothsizeInMm=2.0&microsteps=16&stepsPerRotation=200&estimatedRotationSpeed=7.8&timeToAcc=0.2&timeToDec=0.15
            var input = new EepromConfigurationInput
            {
                Teeth                  = teeth,
                ToothsizeinMm          = toothsizeInMm,
                Microsteps             = microsteps,
                StepsPerRotation       = stepsPerRotation,
                EstimatedRotationSpeed = estimatedRotationSpeed,
                TimeToAcc              = timeToAcc,
                TimeToDec              = timeToDec
            };

            var m = await _eepromConfigurationService.CalculateConfig(input);
            if (m == null)
            {
                return NotFound();
            }

            return Ok(m);
        }
    }
}