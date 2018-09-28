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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Framework.Web.Filter
{
    public abstract class ActionContextLoggerProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        protected ActionContextLoggerProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected ILogger CreateLogger(ActionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor cas)
            {
                return _loggerFactory.CreateLogger(cas.ControllerTypeInfo.AsType());
            }

            // Should not happen, but if it does we use string identifier.
            return _loggerFactory.CreateLogger("WebApiFilter");
        }
    }
}