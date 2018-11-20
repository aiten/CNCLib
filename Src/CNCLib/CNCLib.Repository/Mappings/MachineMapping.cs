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

using CNCLib.Repository.Contract.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNCLib.Repository.Mappings
{
    public static class MachineMapping
    {
        public static void Map(this EntityTypeBuilder<Machine> entity)
        {
            entity.ToTable("Machine");

            entity.HasKey(m => m.MachineId);

            entity.Property(m => m.Name).IsRequired().HasMaxLength(64);

            entity.Property(m => m.ComPort).IsRequired().HasMaxLength(32);

            entity.Property(m => m.Axis).IsRequired();

            entity.Property(m => m.SizeX).IsRequired();
            entity.Property(m => m.SizeY).IsRequired();
            entity.Property(m => m.SizeZ).IsRequired();

            entity.Property(m => m.CommandSyntax).IsRequired();

            entity.HasOne(p => p.User);
            entity.Property(m => m.UserId);
//                IsOptional();
        }
    }
}