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

namespace CNCLib.Repository.Context;

using System;
using System.Collections.Generic;

using CNCLib.Repository.Abstraction.Entities;

using Framework.Repository.Tools;

public class CNCLibDbImporter : DbImporter
{
    protected Dictionary<int, UserEntity>                        _userMap;
    protected Dictionary<int, UserFileEntity>                    _userFileMap;
    protected Dictionary<int, MachineEntity>                     _machineMap;
    protected Dictionary<int, MachineCommandEntity>              _machineCommandMap;
    protected Dictionary<int, MachineInitCommandEntity>          _machineInitMap;
    protected Dictionary<int, ItemEntity>                        _itemMap;
    protected Dictionary<Tuple<int, string>, ItemPropertyEntity> _itemPropertyMap;
    protected Dictionary<int, ConfigurationEntity>               _configurationMap;

    public CNCLibDbImporter(CNCLibContext context) : base(context)
    {
    }
}