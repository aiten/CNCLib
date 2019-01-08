////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using System.Reflection;

using CNCLib.Serial.Shared;

using Microsoft.AspNetCore.Mvc;

namespace CNCLib.Serial.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class InfoController : Controller
    {
        protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        #region Query/Info

        [HttpGet]
        public Info Get()
        {
            return new Info()
            {
                Version   = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Name      = Assembly.GetExecutingAssembly().GetName().Name,
                FullName  = Assembly.GetExecutingAssembly().GetName().FullName,
                Copyright = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright
            };
        }

        [HttpGet("version")]
        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [HttpGet("name")]
        public string GetName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }

        [HttpGet("fullname")]
        public string GetFullName()
        {
            return Assembly.GetExecutingAssembly().GetName().FullName;
        }

        [HttpGet("copyright")]
        public string GetCopyright()
        {
            return ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;
        }

        #endregion
    }
}