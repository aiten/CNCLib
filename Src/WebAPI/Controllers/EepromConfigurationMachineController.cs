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
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;

using Framework.WebAPI.Controller;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class EepromConfigurationController : Controller
    {
        private readonly IEepromConfigurationManager _eepromConfigurationManager;
        private readonly ICNCLibUserContext          _userContext;

        public EepromConfigurationController(IEepromConfigurationManager eepromConfigurationManager, ICNCLibUserContext userContext)
        {
            _eepromConfigurationManager = eepromConfigurationManager ?? throw new ArgumentNullException();
            _userContext                = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext)_userContext).InitFromController(this);
        }

        [HttpGet]
        public async Task<ActionResult<EepromConfiguration>> Get(
            ushort teeth,
            double toothSizeInMm,
            ushort microSteps,
            ushort stepsPerRotation,
            double estimatedRotationSpeed,
            double timeToAcc,
            double timeToDec)
        {
            // http://localhost:2024/api/EepromConfiguration?teeth=15&toothSizeInMm=2.0&microSteps=16&stepsPerRotation=200&estimatedRotationSpeed=7.8&timeToAcc=0.2&timeToDec=0.15
            var input = new EepromConfigurationInput
            {
                Teeth                  = teeth,
                ToothSizeInMm          = toothSizeInMm,
                MicroSteps             = microSteps,
                StepsPerRotation       = stepsPerRotation,
                EstimatedRotationSpeed = estimatedRotationSpeed,
                TimeToAcc              = timeToAcc,
                TimeToDec              = timeToDec
            };

            var m = await _eepromConfigurationManager.CalculateConfig(input);
            return await this.NotFoundOrOk(m);
        }
    }
}