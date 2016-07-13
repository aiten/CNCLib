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

#pragma once

////////////////////////////////////////////////////////

#include <Control.h>
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <Analog8InvertIOControl.h>
#include <ReadPinIOControl.h>
#include <PushButtonLow.h>

#include "Configuration_DCK40Laser.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	CMyControl() { }

	virtual void Kill() override;

	virtual void IOControl(uint8_t tool, unsigned short level) override;
	virtual unsigned short IOControl(uint8_t tool) override;

protected:

	virtual void Init() override;
	virtual void Initialized() override;

	virtual void TimerInterrupt() override;

	bool IsKill() override;
	virtual void Poll() override;
	virtual bool Parse(CStreamReader* reader, Stream* output) override;
	virtual void GoToReference() override;
	virtual bool GoToReference(axis_t axis, steprate_t steprate, bool toMinRef) override;

	virtual bool OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo) override;

private:

	//    CAnalog8InvertIOControl<LASER_PWM_PIN> _laserPWM;
	CAnalog8IOControl<LASER_PWM_PIN> _laserPWM;
	COnOffIOControl<LASER_ENABLE_PIN, LASER_ENABLE_ON, LASER_ENABLE_OFF> _laserOnOff;

	CReadPinIOControl<KILL_PIN, KILL_PIN_ON> _kill;

	CReadPinIOControl<LASERWATCHDOG_PIN, LASERWATCHDOG_ON> _laserWatchDog;

	CPushButtonLow _hold;
	CPushButtonLow _resume;

	COnOffIOControl<LASERWATER_PIN, LASERWATER_ON, LASERWATER_OFF> _laserWater;
	COnOffIOControl<LASERVACUUM_PIN, LASERVACUUM_ON, LASERVACUUM_OFF> _laserVacuum;
};

////////////////////////////////////////////////////////

extern CMyControl Control;

