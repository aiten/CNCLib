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

using CNCLib.Repository.Contract.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNCLib.Repository.Mappings
{
    public static class ItemMapping
    {
        public static void Map(this EntityTypeBuilder<Item> entity)
        {
            entity.ToTable("Item");

            entity.HasKey(i => i.ItemId);

            entity.HasIndex(i => i.Name).IsUnique();

            entity.Property(i => i.Name).IsRequired().HasMaxLength(64);

            entity.Property(i => i.ClassName).IsRequired().HasMaxLength(255);

            entity.HasOne(i => i.User);
            entity.Property(i => i.UserId);
        }
    }
}