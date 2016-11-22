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

using System.Data.Entity;

namespace CNCLib.Repository.Context
{
	public class CNCLibInitializerAzure : CreateDatabaseIfNotExists<CNCLibContext>
	{
		protected override void Seed(CNCLibContext context)
		{
			base.Seed(context);

			// add Testdata to database if no data are stored

			if (context.Machines.CountAsync().Result == 0 && context.Configurations.CountAsync().Result == 0 && context.Items.CountAsync().Result == 0)
			{
				CNCLibDefaultData.CNCSeed(context);
				context.SaveChanges();
			}
		}
	}
}
