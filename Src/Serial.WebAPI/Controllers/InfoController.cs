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

namespace CNCLib.Serial.WebAPI.Controllers
{
    using System.Reflection;

    using CNCLib.Serial.Shared;

    using Microsoft.AspNetCore.Mvc;

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