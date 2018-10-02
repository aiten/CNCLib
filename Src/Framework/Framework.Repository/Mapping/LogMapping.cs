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

using Framework.Contracts.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Repository.Mapping
{
    public static class LogMapping
    {
        public static void Map(this EntityTypeBuilder<Log> entity)
        {
            entity.ToTable("Log");

            entity.HasKey(m => m.Id);

            entity.Property(l => l.Application).IsRequired().IsUnicode().HasMaxLength(50);
            entity.Property(l => l.Level).IsRequired().IsUnicode().HasMaxLength(50);
            entity.Property(l => l.Message).IsRequired().IsUnicode();
            entity.Property(l => l.UserName).IsUnicode().HasMaxLength(250);
            entity.Property(l => l.ServerName).IsUnicode().HasMaxLength(64);
            entity.Property(l => l.Port).IsUnicode().HasMaxLength(256);
            entity.Property(l => l.Url).IsUnicode().HasMaxLength(500);
            entity.Property(l => l.ServerAddress).IsUnicode().HasMaxLength(100);
            entity.Property(l => l.RemoteAddress).IsUnicode().HasMaxLength(100);
            entity.Property(l => l.Logger).IsUnicode().HasMaxLength(250);
            entity.Property(l => l.Callsite).IsUnicode();
            entity.Property(l => l.Exception).IsUnicode();
        }
    }
}