﻿////////////////////////////////////////////////////////
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

using System.Security.Principal;
using System.ServiceModel.Channels;
using CNCLib.Shared;
using Framework.Contracts.Shared;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace CNCLib.WebAPI
{
    public class CNCLibUserContext : ICNCLibUserContextRW
    {
        public string UserName { get; private set; }

        public int UserID { get; private set; }

        public void InitFromController(Controller controller)
        {
            UserName = "WebAPI";
        }
    }
}
