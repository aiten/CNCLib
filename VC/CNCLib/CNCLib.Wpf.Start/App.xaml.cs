////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using Framework.Logic;
using CNCLib.Repository;
using CNCLib.Repository.Contracts;
using CNCLib.Logic;
using CNCLib.Logic.Contracts;
using Framework.Wpf.ViewModels;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.EF;
using CNCLib.Repository.Context;

namespace CNCLib.Wpf.Start
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void AppStartup(object sender, StartupEventArgs e)
		{
            Dependency.Initialize(new LiveDependencyProvider());
            Dependency.Container.RegisterTypesIncludingInternals(typeof(CNCLib.Repository.MachineRepository).Assembly, typeof(CNCLib.Logic.MachineControler).Assembly);
            Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();
        }
    }
}
