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

namespace CNCLib.Logic.Abstraction.DTO;

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
    public double DistancePerStepInMm     { get; set; }

    public double EstimatedMaxStepRate                      { get; set; }
    public double EstimatedMaxSpeedInMmSec                  { get; set; }
    public double EstimatedMaxSpeedInMmMin                  { get; set; }
    public double EstimatedAccelerationInMmSec2             { get; set; }
    public double EstimatedAccelerationDistToMaxSpeedInMm   { get; set; }
    public double EstimatedDecelerationInMmSec2             { get; set; }
    public double EstimatedDecelerationDistFromMaxSpeedInMm { get; set; }
    public double EstimatedAcc                              { get; set; }
    public double EstimatedDec                              { get; set; }
    public double EstimatedJerkSpeed                        { get; set; }
}