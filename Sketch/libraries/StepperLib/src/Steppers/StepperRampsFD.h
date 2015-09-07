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

#include "Stepper.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

// only available on Arduino Mega or due

////////////////////////////////////////////////////////

#define E0_AXIS A_AXIS
#define E1_AXIS B_AXIS
#define E2_AXIS C_AXIS

#define RAMPSFD_ENDSTOPCOUNT 6

class CStepperRampsFD : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperRampsFD();
	virtual void Init() override;

protected:

	virtual void  SetEnable(axis_t axis, unsigned char level, bool force) override;
	virtual unsigned char GetEnable(axis_t axis) override;
	virtual void  Step(const unsigned char cnt[NUM_AXIS], axisArray_t directionUp) override;

public:

	virtual bool IsReference(unsigned char referenceid) override;
	virtual bool IsAnyReference() override;

protected:

	////////////////////////////////////////////////////////

private:
};

#endif