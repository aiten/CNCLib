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
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <ReadPinIOControl.h>

#include "Configuration_MiniCNC.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	CMyControl()				 { }

	virtual void Kill() override;

	virtual void IOControl(unsigned char tool, unsigned short level) override;
	virtual unsigned short IOControl(unsigned char tool) override;

protected:

	virtual void Init() override;

	virtual bool IsKill() override;

	virtual bool Parse(CStreamReader* reader, Stream* output) override;

	virtual void GoToReference() override;

	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo) override;

private:

	COnOffIOControl<SPINDEL_PIN, SPINDEL_ON, SPINDEL_OFF> _spindel;
	COnOffIOControl<CONTROLLERFAN_FAN_PIN, CONTROLLERFAN_ON, CONTROLLERFAN_OFF> _controllerfan;
	CReadPinIOControl<PROBE1_PIN, PROBE_ON> _probe;
	CReadPinIOControl<KILL_PIN, KILL_ON> _kill;
};

////////////////////////////////////////////////////////

extern CMyControl Control;
