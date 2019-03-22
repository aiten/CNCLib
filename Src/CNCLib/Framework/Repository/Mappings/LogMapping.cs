/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Framework.Repository.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Abstraction.Entities;

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
            entity.Property(l => l.MachineName).IsUnicode().HasMaxLength(64);
            entity.Property(l => l.ServerName).IsUnicode().HasMaxLength(64);
            entity.Property(l => l.Port).IsUnicode().HasMaxLength(256);
            entity.Property(l => l.Url).IsUnicode().HasMaxLength(500);
            entity.Property(l => l.ServerAddress).IsUnicode().HasMaxLength(100);
            entity.Property(l => l.RemoteAddress).IsUnicode().HasMaxLength(100);
            entity.Property(l => l.Logger).IsUnicode().HasMaxLength(250);
            entity.Property(l => l.StackTrace).IsUnicode();
            entity.Property(l => l.Exception).IsUnicode();
        }
    }
}