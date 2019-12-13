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

using CNCLib.Repository.Abstraction.Entities;

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

            entity.HasAlternateKey(c => new { c.UserId, c.Name });

            entity.Property(m => m.Name).IsRequired().HasMaxLength(64);

            entity.Property(m => m.ComPort).IsRequired().HasMaxLength(32);

            entity.Property(m => m.Axis).IsRequired();

            entity.Property(m => m.SizeX).IsRequired();
            entity.Property(m => m.SizeY).IsRequired();
            entity.Property(m => m.SizeZ).IsRequired();

            entity.Property(m => m.CommandSyntax).IsRequired();

            entity.HasOne(p => p.User);
            entity.Property(m => m.UserId);
        }
    }
}