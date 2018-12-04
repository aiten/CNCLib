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

using CNCLib.Service.Contract;
using CNCLib.Wpf.Services;

using Framework.Dependency;
using Framework.Dependency.Abstraction;
using Framework.Pattern;

namespace CNCLib.Wpf
{
    public static class LiveDependencyRegisterExtensions
    {
        public static IDependencyContainer RegisterCNCLibWpf(this IDependencyContainer container)
        {
            Dependency.Container.RegisterInstance(Global.Instance);

            Dependency.Container.RegisterType<IJoystickService, JoystickService>();

            Dependency.Container.RegisterType<IFactory<IMachineService>, FactoryResolve<IMachineService>>();
            Dependency.Container.RegisterType<IFactory<ILoadOptionsService>, FactoryResolve<ILoadOptionsService>>();
            Dependency.Container.RegisterType<IFactory<IJoystickService>, FactoryResolve<IJoystickService>>();

            Dependency.Container.RegisterTypesByName(n => n.EndsWith("ViewModel"), typeof(ViewModels.MachineViewModel).Assembly, typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            return container;
        }
    }
}