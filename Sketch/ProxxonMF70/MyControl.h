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

#include <Control3D.h>
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <Analog8InvertIOControl.h>
#include <ReadPinIOControl.h>

////////////////////////////////////////////////////////

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl3D
{
private:

	typedef CControl3D super;

public:

	CMyControl()				 { }

	virtual void Kill();

	virtual void IOControl(unsigned char tool, unsigned short level);
	virtual unsigned short IOControl(unsigned char tool);

protected:

	virtual void Init();
	virtual void Initialized();

	virtual bool IsKill();

	virtual void GoToReference(axis_t axis, steprate_t steprate);

	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo);

private:

	COnOffIOControl<COOLANT_PIN, COOLANT_ON, COOLANT_OFF> _coolant;
	COnOffIOControl<SPINDEL_PIN, SPINDEL_ON, SPINDEL_OFF> _spindel;
	CReadPinIOControl<PROBE1_PIN, PROBE_ON> _probe;
	CReadPinIOControl<CAT(BOARDNAME,_LCD_KILL_PIN),CAT(BOARDNAME,_LCD_KILL_PIN_ON)> _kill;

#if defined(USE_RAMPSFD)

	CAnalog8InvertIOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;

#else
	CAnalog8IOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
#endif

};

////////////////////////////////////////////////////////

extern CMyControl Control;
