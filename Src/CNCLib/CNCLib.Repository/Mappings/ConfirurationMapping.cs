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
    public static class ConfigurationMapping
    {
        public static void Map(this EntityTypeBuilder<Configuration> entity)
        {
            entity.ToTable("Configuration");

            entity.HasKey(c => new { c.Group, c.Name });

            entity.Property(c => c.Group).IsRequired().HasMaxLength(256);

            entity.Property(c => c.Name).IsRequired().HasMaxLength(256);

            entity.Property(c => c.Type).IsRequired().HasMaxLength(256);

            entity.Property(c => c.Value).HasMaxLength(4000);

            entity.HasOne(c => c.User);
            entity.Property(c => c.UserId);

//                IsOptional();
        }
    }
}