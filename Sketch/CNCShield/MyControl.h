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
#include <PushButtonLow.h>

#include "Configuration_CNCShield.h"

#include <Steppers/StepperCNCShield.h>

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
  virtual void TimerInterrupt() override;

  bool IsButton(EnumAsByte(EIOButtons) button) override;

	virtual bool Parse(CStreamReader* reader, Stream* output) override;

  virtual void GoToReference(axis_t axis, steprate_t steprate) override;

	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo) override;

private:

#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
	COnOffIOControl<CNCSHIELD_SPINDEL_ENABLE_PIN, CNCSHIELD_SPINDEL_ON,       CNCSHIELD_SPINDEL_OFF> _spindel;
  COnOffIOControl<CNCSHIELD_SPINDEL_DIR_PIN,    CNCSHIELD_SPINDEL_DIR_CLW,  CNCSHIELD_SPINDEL_DIR_CCLW> _spindelDir;
#endif  
  COnOffIOControl<CNCSHIELD_COOLANT_PIN, CNCSHIELD_COOLANT_ON, CNCSHIELD_COOLANT_OFF> _coolant;
#ifdef CNCSHIELD_PROBE_PIN
	CReadPinIOControl<CNCSHIELD_PROBE_PIN, CNCSHIELD_PROBE_ON> _probe;
#endif
	CReadPinIOControl<CNCSHIELD_ABORT_PIN, CNCSHIELD_ABORT_ON> _kill;
  CPushButtonLow _hold;
  CPushButtonLow _resume;
};

////////////////////////////////////////////////////////

extern CMyControl Control;
