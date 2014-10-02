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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#include <StepperLib.h>
#include <CNCLib.h>

#include "MyControl.h"
#include "PlotterControl.h"
#include "MyLcd.h"
#include "HPGLParser.h"

#include <Wire.h>  // Comes with Arduino IDE

#include <LiquidCrystal_I2C.h>

////////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

CStepperRamps14 Stepper;

#elif defined(__SAM3X8E__)

CStepperRampsFD Stepper;

#elif defined(__AVR_ATmega328P__)

CStepperSMC800 Stepper;

#elif defined(_MSC_VER)

#error NOT in MVC

#endif

CMyControl Control;
CPlotter Plotter;

#ifdef __USE_LCD__
CMyLcd Lcd;
#endif

void setup()
{  
  StepperSerial.begin(115200);
}

void loop()
{
  Control.Run();
}

