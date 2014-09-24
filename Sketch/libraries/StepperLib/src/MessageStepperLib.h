////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define MESSAGE_OK									F("ok")
#define MESSAGE_ERROR								F("Error: ")
#define MESSAGE_INFO								F("Info: ")
#define MESSAGE_WARNING								F("Warning: ")

#ifdef REDUCED_SIZE

#define MESSAGE_STEPPER_EmptyMoveSkipped			F("I1")
#define MESSAGE_STEPPER_Backlash					F("I2")
#define MESSAGE_STEPPER_IsAnyReference				F("I3")
#define MESSAGE_STEPPER_RangeLimit					F("E1")
#define MESSAGE_STEPPER_IsReferenceIsOn				F("E2")
#define MESSAGE_STEPPER_MoveReferenceFailed			F("I4")

#define MESSAGE_STEPPER_MoveAwayFromReference		F("I5")

#else

#define MESSAGE_STEPPER_EmptyMoveSkipped			F("EmptyMove skipped")
#define MESSAGE_STEPPER_Backlash					F("Backlash")
#define MESSAGE_STEPPER_IsAnyReference				F("IsAnyReference")
#define MESSAGE_STEPPER_RangeLimit					F("Range limit")
#define MESSAGE_STEPPER_IsReferenceIsOn				F("IsReference is on")
#define MESSAGE_STEPPER_MoveReferenceFailed			F("MoveReference failed")

#define MESSAGE_STEPPER_MoveAwayFromReference		F("Move away from reference")

#endif