////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

//#define STEPPERTYPE 1		// CStepperL298N
//#define STEPPERTYPE 2		// CStepperSMC800
#define STEPPERTYPE 3		// CStepperTB6560
//#define STEPPERTYPE 4		// CStepperCNCShield

////////////////////////////////////////////////////////

#if STEPPERTYPE==1

#include "Configuration_MiniCNC_L298N.h"

#elif STEPPERTYPE==2

#include "Configuration_MiniCNC_SMC800.h"

#elif STEPPERTYPE==3

#include "Configuration_MiniCNC_TB6560.h"

#elif STEPPERTYPE==4

#include "Configuration_MiniCNC_CNCShield.h"

#endif

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_Proxxon_Starting					F("MiniCNC:" __DATE__ )

