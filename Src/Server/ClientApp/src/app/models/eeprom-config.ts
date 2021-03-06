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

export class EepromConfig {
  maxStepRate: number;
  acc: number;
  dec: number;
  jerkSpeed: number;
  stepsPerMm1000: number;

  stepsPerRotation: number;
  distancePerRotationInMm: number;
  distancePerStepInMm: number;
  stepsPerMm: number;

  estimatedMaxStepRate: number;
  estimatedMaxSpeedInMmSec: number;
  estimatedMaxSpeedInMmMin: number;
  estimatedAccelerationInMmSec2: number;
  estimatedDecelerationInMmSec2: number;
  estimatedAccelerationDistToMaxSpeedInMm: number;
  estimatedDecelerationDistFromMaxSpeedInMm: number;
  estimatedAcc: number;
  estimatedDec: number;
  estimatedJerkSpeed: number;
}
