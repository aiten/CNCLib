export class EepromConfig
{
  maxStepRate: number;
  acc: number;
  dec: number;
  jerkSpeed: number;
  stepsPerMm1000: number;

  stepsPerRotation: number;
  distancePerRotationInMm: number;
  stepsPerMm: number;

  estimatedMaxStepRate: number;
  estimatedMaxSpeedInMmSec: number;
  estimatedAccelerationInMmSec2: number;
  estimatedDecelerationInMmSec2: number;
  estimatedAcc: number;
  estimatedDec: number;
  estimatedJerkSpeed: number;
}
