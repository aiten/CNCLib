﻿/*
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

namespace CNCLib.UnitTest;

using System.Security.Claims;

using CNCLib.Logic.Abstraction;
using CNCLib.Shared;

public class CNCLibUserContext : ICNCLibUserContext
{
    public int    UserId   { get; private set; }
    public string UserName { get; private set; }
    public bool   IsAdmin  { get; private set; }

    public ClaimsPrincipal User { get; private set; }

    public CNCLibUserContext()
    {
        UserName = "Maxi";
        UserId   = 1;
        IsAdmin  = true;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim(ClaimTypes.Name,           UserName),
            new Claim(CNCLibClaimTypes.IsAdmin,  "true"),
        };
        var identity = new ClaimsIdentity(claims, "BasicAuthentication");

        User = new ClaimsPrincipal(identity);
    }
}