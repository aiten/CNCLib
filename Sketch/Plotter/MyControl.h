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

#include <Control.h>
#include <Analog8IOControl.h>
#include <ReadPinIOControl.h>

#include "Configuration_Plotter.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	void MyInit() { return Init(); }

	virtual void IOControl(unsigned char tool, unsigned short level) override;
	virtual unsigned short IOControl(unsigned char tool) override;

protected:

	virtual void Init() override;
	virtual bool Parse(CStreamReader* reader, Stream* output) override;
	virtual void Idle(unsigned int idletime) override;
	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo) override;

	virtual bool IsKill() override;

	virtual void GoToReference() override;
	virtual bool GoToReference(axis_t axis, steprate_t steprate, bool toMinRef) override;

private:

	CAnalog8IOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
	CReadPinIOControl<RAMPS14_KILL_PIN,RAMPS14_LCD_KILL_PIN_ON> _kill;

};

////////////////////////////////////////////////////////

extern CMyControl Control;
