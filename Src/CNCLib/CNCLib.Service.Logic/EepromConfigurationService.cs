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
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;
using Framework.Pattern;

namespace CNCLib.Service.Logic
{
    public class EepromConfigurationService : DisposeWrapper, IEepromConfigurationService
    {
        readonly IEepromConfigurationManager _manager;

        public EepromConfigurationService(IEepromConfigurationManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException();
        }

        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            return await _manager.CalculateConfig(param);
        }

        protected override void DisposeManaged()
        {
        }
    }
}