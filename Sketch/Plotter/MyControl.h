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

#include <Servo.h>

#include <Control3D.h>
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <Analog8IOControlSmooth.h>
#include <ReadPinIOControl.h>
#include <PushButtonLow.h>
#include <DummyIOControl.h>
#include <ConfigEeprom.h>

#include "Configuration.h"
#include "MyLCD.h"

// must be after "Configuration.h" because of defines
#include <ControlImplementation.h>

////////////////////////////////////////////////////////

class CMyControl : public CControl3D
{
private:

	typedef CControl3D super;

public:

	CMyControl() { }

	virtual void Kill() override;

	virtual void IOControl(uint8_t tool, unsigned short level) override;
#ifndef REDUCED_SIZE
	virtual unsigned short IOControl(uint8_t tool) override;
#endif

protected:

	virtual void Init() override;
	virtual void Initialized() override;

	virtual void TimerInterrupt() override;
	virtual bool Parse(CStreamReader* reader, Stream* output) override;
	virtual void Idle(unsigned int idletime) override;

	virtual bool IsKill() override;
	virtual void Poll() override;

	virtual bool OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo) override;

public:

	struct SMyCNCEeprom
	{
		CConfigEeprom::SCNCEeprom _std;
		feedrate_t	pendownFeedrate;
		feedrate_t	penupFeedrate;

		feedrate_t	movependownFeedrate;
		feedrate_t	movepenupFeedrate;
		feedrate_t	movepenchangeFeedrate;

		mm1000_t	pendownpos;
		mm1000_t	penuppos;

		mm1000_t	penchangepos_x;
		mm1000_t	penchangepos_y;
		mm1000_t	penchangepos_z;

		mm1000_t	penchangepos_x_ofs;
		mm1000_t	penchangepos_y_ofs;

		unsigned short penchangeServoClampOpenPos;
		unsigned short penchangeServoClampClosePos;

		unsigned short penchangeServoClampOpenDelay;
		unsigned short penchangeServoClampCloseDelay;
	};

private:

	static const SMyCNCEeprom _eepromFlash;

	ControlData _data;

	Servo _servo1;
	Servo _servo2;
};

////////////////////////////////////////////////////////

extern CMyControl Control;











