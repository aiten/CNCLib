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

using System;
using System.Threading.Tasks;

using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;

using Framework.Logic;

namespace CNCLib.Logic.Manager
{
    public class EepromConfigurationManager : ManagerBase, IEepromConfigurationManager
    {
        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            var result = new EepromConfiguration();

            double acc_corr   = 1.0615;
            double jerkfactor = 25;

            result.StepsPerRotation        = param.Microsteps * (uint) param.StepsPerRotation;
            result.DistancePerRotationInMm = param.Teeth * param.ToothsizeinMm;
            if (Math.Abs(result.DistancePerRotationInMm) > double.Epsilon)
            {
                result.StepsPerMm = result.StepsPerRotation / result.DistancePerRotationInMm;
            }

            result.EstimatedMaxStepRate     = result.StepsPerRotation * param.EstimatedRotationSpeed;
            result.EstimatedMaxSpeedInMmSec = result.DistancePerRotationInMm * param.EstimatedRotationSpeed;
            if (Math.Abs(param.TimeToAcc) > double.Epsilon)
            {
                result.EstimatedAccelerationInMmSec2 = result.EstimatedMaxSpeedInMmSec / param.TimeToAcc;
                result.EstimatedAcc                  = Math.Sqrt(result.EstimatedMaxStepRate / param.TimeToAcc) * acc_corr;
            }

            if (Math.Abs(param.TimeToDec) > double.Epsilon)
            {
                result.EstimatedDecelerationInMmSec2 = result.EstimatedMaxSpeedInMmSec / param.TimeToDec;
                result.EstimatedDec                  = Math.Sqrt(result.EstimatedMaxStepRate / param.TimeToDec) * acc_corr;
            }

            result.EstimatedJerkSpeed = result.EstimatedMaxStepRate / jerkfactor;

            result.MaxStepRate    = (uint) Math.Round(result.EstimatedMaxStepRate, 0);
            result.Acc            = (ushort) Math.Round(result.EstimatedAcc,       0);
            result.Dec            = (ushort) Math.Round(result.EstimatedDec,       0);
            result.JerkSpeed      = (uint) Math.Round(result.EstimatedJerkSpeed,   0);
            result.StepsPerMm1000 = (float) (result.StepsPerMm / 1000.0);

            return await Task.FromResult(result);
        }
    }
}