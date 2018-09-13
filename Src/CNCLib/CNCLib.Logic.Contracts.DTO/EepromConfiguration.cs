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


namespace CNCLib.Logic.Contracts.DTO
{
    public class EepromConfiguration
    {
        public uint   MaxStepRate    { get; set; }
        public ushort Acc            { get; set; }
        public ushort Dec            { get; set; }
        public uint   JerkSpeed      { get; set; }
        public float  StepsPerMm1000 { get; set; }

        public uint   StepsPerRotation        { get; set; }
        public double DistancePerRotationInMm { get; set; }
        public double StepsPerMm              { get; set; }

        public double EstimatedMaxStepRate          { get; set; }
        public double EstimatedMaxSpeedInMmSec      { get; set; }
        public double EstimatedAccelerationInMmSec2 { get; set; }
        public double EstimatedDecelerationInMmSec2 { get; set; }
        public double EstimatedAcc                  { get; set; }
        public double EstimatedDec                  { get; set; }
        public double EstimatedJerkSpeed            { get; set; }
    }
}