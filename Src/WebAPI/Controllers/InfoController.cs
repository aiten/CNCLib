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

namespace CNCLib.WebAPI.Controllers;

using System.Reflection;

using Microsoft.AspNetCore.Mvc;

public class Info
{
    public string Version   { get; set; }
    public string Name      { get; set; }
    public string FullName  { get; set; }
    public string Copyright { get; set; }
}

[Route("api/[controller]")]
public class InfoController : Controller
{
    #region Query/Info

    [HttpGet]
    public Info Get()
    {
        return Info();
    }

    [HttpGet("version")]
    public string GetVersion()
    {
        return Info().Version.ToString();
    }

    [HttpGet("name")]
    public string GetName()
    {
        return Info().Name;
    }

    [HttpGet("fullname")]
    public string GetFullName()
    {
        return Info().FullName;
    }

    [HttpGet("copyright")]
    public string GetCopyright()
    {
        return Info().Copyright;
    }

    private static Info Info()
    {
        var ass     = Assembly.GetExecutingAssembly();
        var assName = ass.GetName();

        return new Info()
        {
            Version   = ass.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            Copyright = ass.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright,
            Name      = assName.Name,
            FullName  = assName.FullName,
        };
    }

    #endregion
}