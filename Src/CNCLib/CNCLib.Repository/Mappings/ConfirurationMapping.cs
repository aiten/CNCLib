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

using CNCLib.Repository.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNCLib.Repository.Mappings
{
	public static class ConfigurationMapping
    {
        public static void Map(this EntityTypeBuilder<Configuration> entity)
        {
            entity.ToTable("Configuration");

            entity.HasKey(m => new { m.Group, m.Name });

            entity.Property(m => m.Group).
                IsRequired().
                HasMaxLength(256);

            entity.Property(m => m.Name).
                IsRequired().
                HasMaxLength(256);

            entity.Property(m => m.Type).
                IsRequired().
                HasMaxLength(256);

            entity.Property(m => m.Value).
                HasMaxLength(4000);

//            entity.Property(m => m.UserID).
//                IsOptional();
        }
    }
}
