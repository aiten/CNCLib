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
    public static class UserMapping
    {
        public static void Map(this EntityTypeBuilder<User> entity)
        {
            entity.ToTable("User");

            entity.HasKey(m => m.UserId);

            entity.HasIndex(e => e.UserName).IsUnique();

            entity.Property(m => m.UserName).IsRequired().IsUnicode().HasMaxLength(128);

            entity.Property(m => m.UserPassword).IsUnicode().HasMaxLength(255);
        }
    }
}