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
////////////////////////////////////////////////////////

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "HAL.h"

////////////////////////////////////////////////////////

#if defined(__SAMD21G18A__)

static void IgnoreIrq() {}

void TC4_Handler()
{
	TcCount16* TC = GetTimer1Struct();

	if (TC->INTFLAG.bit.OVF == 1)                     // A overflow caused the interrupt
	{
		TC->INTFLAG.bit.OVF = 1;                     // writing a one clears the flag ovf flag
		WaitForSyncTC(TC);
	}
	
	StepperSerial.println("TC");
	CHAL::_TimerEvent1();
}
/*
void TC6_Handler()
{
	//DODO:SAMD21
	CHAL::_TimerEvent3();
}
*/
void CAN0_Handler()
{
	CHAL::_BackgroundEvent();
}

CHAL::HALEvent CHAL::_BackgroundEvent = IgnoreIrq;

////////////////////////////////////////////////////////

#endif 

