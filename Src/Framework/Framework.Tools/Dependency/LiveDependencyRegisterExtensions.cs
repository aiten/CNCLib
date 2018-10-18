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
using System.Linq;
using System.Reflection;
using Framework.Contracts.Shared;

namespace Framework.Tools.Dependency
{
    public static class LiveDependencyRegisterExtensions
    {
        public static IDependencyContainer RegisterFrameWorkTools(this IDependencyContainer container)
        {
            container.RegisterType<ICurrentDateTime, CurrentDateTime>();
            container.RegisterType<Framework.Contracts.Logging.ILoggerFactory, Framework.Tools.Logging.LoggerFactory>();
            container.RegisterType(typeof(Framework.Contracts.Logging.ILogger<>), typeof(Framework.Tools.Logging.Logger<>));

            return container;
        }
    }
}