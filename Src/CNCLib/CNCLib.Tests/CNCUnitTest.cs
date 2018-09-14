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

using Framework.Test;
using AutoMapper;
using CNCLib.Logic;
using Framework.Tools.Dependency;
using CNCLib.Wpf;
using CNCLib.GCode.GUI;

namespace CNCLib.Tests
{
    public abstract class CNCUnitTest : UnitTestBase
    {
        protected override void InitializeCoreDependencies()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogicAutoMapperProfile>();
                cfg.AddProfile<WpfAutoMapperProfile>();
                cfg.AddProfile<GCodeGUIAutoMapperProfile>();
            });
            config.AssertConfigurationIsValid();

            IMapper mapper = config.CreateMapper();
            Dependency.Container.RegisterInstance(mapper);
        }
    }
}