////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Windows;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.EF;
using CNCLib.Repository.Context;
using AutoMapper;
using Framework.Tools.Mapper;

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
            Dependency.Container.RegisterTypesIncludingInternals(typeof(CNCLib.Repository.MachineRepository).Assembly, typeof(CNCLib.Logic.MachineController).Assembly);
            Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

			var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<CNCLib.Repository.Contracts.Entities.Machine, CNCLib.Logic.Contracts.DTO.Machine>();
					cfg.CreateMap<CNCLib.Repository.Contracts.Entities.MachineInitCommand, CNCLib.Logic.Contracts.DTO.MachineInitCommand>();
					cfg.CreateMap<CNCLib.Repository.Contracts.Entities.MachineCommand, CNCLib.Logic.Contracts.DTO.MachineCommand>();

					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.Machine, CNCLib.Repository.Contracts.Entities.Machine>();
					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineInitCommand, CNCLib.Repository.Contracts.Entities.MachineInitCommand>();
					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineCommand, CNCLib.Repository.Contracts.Entities.MachineCommand>();

					cfg.CreateMap<CNCLib.Repository.Contracts.Entities.Item, CNCLib.Logic.Contracts.DTO.Item>();

					cfg.CreateMap<CNCLib.Wpf.Models.Machine, CNCLib.Logic.Contracts.DTO.Machine>();
					cfg.CreateMap<CNCLib.Wpf.Models.MachineInitCommand, CNCLib.Logic.Contracts.DTO.MachineInitCommand>();
					cfg.CreateMap<CNCLib.Wpf.Models.MachineCommand, CNCLib.Logic.Contracts.DTO.MachineCommand>();

					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.Machine, CNCLib.Wpf.Models.Machine>();
					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineInitCommand, CNCLib.Wpf.Models.MachineInitCommand>();
					cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineCommand, CNCLib.Wpf.Models.MachineCommand>();

				});

			var mapper = config.CreateMapper();

			Dependency.Container.RegisterInstance<IMapper>(mapper);


			// Open Database here
			//
			try
			{
				using (var controller = Dependency.Resolve<IMachineController>())
				{
					var dto = controller.GetMachine(-1);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot create/connect database in c:\\tmp\n\r" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Application.Current.Shutdown();
			}
		}
	}
}
