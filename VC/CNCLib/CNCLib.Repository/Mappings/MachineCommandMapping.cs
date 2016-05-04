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

using CNCLib.Repository.Contracts.Entities;
using System.Data.Entity.ModelConfiguration;

namespace CNCLib.Repository.Mappings
{
	public class MachineCommandMapping : EntityTypeConfiguration<MachineCommand>
    {
		public MachineCommandMapping()
        {
            HasKey(mc => mc.MachineCommandID);

            Property((m) => m.CommandString).
                IsRequired().
                HasMaxLength(64);
            Property((m) => m.CommandName).
                IsRequired().
                HasMaxLength(64);

            /*
            ==> Index should ignore null

                        Property(m => m.MachineID)
                            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IDX_MachinePosXPosY", 0) { IsUnique = true }));

                        Property(m => m.PosX)
                            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IDX_MachinePosXPosY", 1) { IsUnique = true }));

                        Property(m => m.PosY)
                            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IDX_MachinePosXPosY", 2) { IsUnique = true }));
            */
        }
    }
}
