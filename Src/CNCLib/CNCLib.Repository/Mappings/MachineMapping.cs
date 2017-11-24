////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

using CNCLib.Repository.Contracts.Entities;
using System.Data.Entity.ModelConfiguration;

namespace CNCLib.Repository.Mappings
{
	public class MachineMapping : EntityTypeConfiguration<Machine>
    {
		public MachineMapping()
        {
            HasKey(m => m.MachineID);

            Property(m => m.Name).
                IsRequired().
                HasMaxLength(64);

            Property(m => m.ComPort).
                IsRequired().
                HasMaxLength(32);

            Property(m => m.Axis).IsRequired();

            Property(m => m.SizeX).IsRequired();
            Property(m => m.SizeY).IsRequired();
            Property(m => m.SizeZ).IsRequired();

            Property(m => m.CommandSyntax).
                IsRequired();

            Property(m => m.UserID).
                IsOptional();

        }
    }
}
