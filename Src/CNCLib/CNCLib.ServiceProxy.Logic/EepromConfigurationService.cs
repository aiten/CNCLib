////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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


using System.Collections.Generic;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;
using System.Threading.Tasks;
using System;
using Framework.Tools;

namespace CNCLib.ServiceProxy.Logic
{
    public class EepromConfigurationService : DisposeWrapper, IEepromConfigurationService
    {
        public EepromConfigurationService(IEepromConfigurationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException();
        }

        readonly IEepromConfigurationController _controller;

        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            return await _controller.CalculateConfig(param);
        }

        protected override void DisposeManaged()
        {
        }
    }
}
