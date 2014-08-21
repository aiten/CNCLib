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

#pragma once

////////////////////////////////////////////////////////

//#define USE_RAMPS14
#define USE_RAMPSFD

////////////////////////////////////////////////////////

#if defined(USE_RAMPS14)

#include <StepperRamps14_pins.h>
#define BOARDNAME RAMPS14
#else if  defined(USE_RAMPSFD)

#include <StepperRampsFD_pins.h>
#define BOARDNAME RAMPSFD

#endif

////////////////////////////////////////////////////////

#define CAT(x, y) CAT_(x, y)
#define CAT_(x, y) x ## y

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	CAT(BOARDNAME,_FAN_PIN)

////////////////////////////////////////////////////////

#define COOLANT_PIN	42

#define COOLANT_ON  LOW
#define COOLANT_OFF HIGH

////////////////////////////////////////////////////////

#define SPINDEL_PIN	40

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	44	// on AUX2
#define PROBE2_PIN	64	// on AUX2

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON		CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

#if defined(__SAM3X8E__) && defined (USE_RAMPS14)

#define SD_ENABLE				52
#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)

#else

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define SD_ENABLE			CAT(BOARDNAME,_SDSS)

#endif

////////////////////////////////////////////////////////

#include <MessageCNCExLib.h>

#if defined(__SAM3X8E__)
#if defined(USE_RAMPS14)
#define MESSAGE_MYCONTROL_Proxxon_Starting					F("Proxxon MF 70(HA) Ramps 1.4 due is starting ... ("__DATE__", "__TIME__")")
#else if defined(USE_RAMPSFD)
#define MESSAGE_MYCONTROL_Proxxon_Starting					F("Proxxon MF 70(HA) Ramps FD due is starting ... ("__DATE__", "__TIME__")")
#endif
#elif defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)
#define MESSAGE_MYCONTROL_Proxxon_Starting					F("Proxxon MF 70(HA) Ramps 1.4 mega is starting ... ("__DATE__", "__TIME__")")
#else
#define MESSAGE_MYCONTROL_Proxxon_Starting					F("Proxxon MF 70(HA) Ramps 1.4 is starting ... ("__DATE__", "__TIME__")")
#endif
#define MESSAGE_MYCONTROL_InitializingSDCard				F("Initializing SD card...")
#define MESSAGE_MYCONTROL_initializationFailed				MESSAGE_PARSER3D_INITIALIZATION_FAILED
#define MESSAGE_MYCONTROL_initializationDone				MESSAGE_PARSER3D_INITIALIZATION_DONE
#define MESSAGE_MYCONTROL_ExecutingStartupNc				F("Executing startup.nc")
#define MESSAGE_MYCONTROL_NoStartupNcFoundOnSD				F("no startup.nc found on SD")
#define MESSAGE_MYCONTROL_ExecutingStartupNcDone			F("Executing startup.nc done")

