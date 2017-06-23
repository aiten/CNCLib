////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
#include <ReadPinIOTriggerControl.h>
#include <PushButtonLow.h>
#include <PushButton.h>
#include <DummyIOControl.h>
#include <ConfigEeprom.h>

#include "Configuration.h"

// must be after "Configuration.h" because of defines
#include <ControlImplementation.h>

////////////////////////////////////////////////////////

class CMyControl : public CControl3D
{
private:

	typedef CControl3D super;

public:

	CMyControl()				 { }

	virtual void Kill() override;

	virtual void IOControl(uint8_t tool, unsigned short level) override;
	virtual unsigned short IOControl(uint8_t tool) override;

protected:

	virtual void Init() override;
	virtual void Initialized() override;

	virtual void TimerInterrupt() override;

	virtual bool IsKill() override;
	virtual void Poll() override;
	virtual bool GoToReference(axis_t axis, steprate_t steprate, bool toMinRef) override;

	virtual bool OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo) override;

private:

	static const CConfigEeprom::SCNCEeprom _eepromFlash;

	ControlData _data;
};

////////////////////////////////////////////////////////

extern CMyControl Control;

