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

#include <Control3D.h>
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <Analog8InvertIOControl.h>
#include <ReadPinIOControl.h>
#include <ReadPinIOTriggerControl.h>
#include <PushButton.h>

////////////////////////////////////////////////////////

#include "Configuration_KK1000S.h"

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

	virtual bool IsKill() override;
	virtual void Poll() override;

	virtual void GoToReference() override;
	virtual bool GoToReference(axis_t axis, steprate_t steprate, bool toMinRef) override;

	virtual bool OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo) override;
	virtual void TimerInterrupt() override;

private:

	COnOffIOControl<COOLANT_PIN, COOLANT_ON, COOLANT_OFF> _coolant;
	COnOffIOControl<SPINDEL_PIN, SPINDEL_ON, SPINDEL_OFF> _spindel;
	CReadPinIOControl<PROBE1_PIN, PROBE_ON> _probe;
//	CReadPinIOControl<MASH6050S_KILL_PIN,MASH6050S_KILL_PIN_ON> _kill;
	CReadPinIOTriggerControl<MASH6050S_KILL_PIN,MASH6050S_KILL_PIN_ON,200> _kill;
	CPushButton _holdresume;

	CAnalog8IOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
};

////////////////////////////////////////////////////////

extern CMyControl Control;
