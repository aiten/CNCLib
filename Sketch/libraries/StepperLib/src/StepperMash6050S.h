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

#include "Stepper.h"

////////////////////////////////////////////////////////

#define MASH6050S_ENDSTOPCOUNT 4

////////////////////////////////////////////////////////

class CStepperMash6050S : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperMash6050S();
	virtual void Init();

protected:

	virtual void  SetEnable(axis_t axis, unsigned char level, bool force);
	virtual unsigned char GetEnable(axis_t axis);
	virtual void  Step(const unsigned char cnt[NUM_AXIS], axisArray_t directionUp);

public:

	virtual bool IsReference(unsigned char referenceid);
	virtual bool IsAnyReference();

protected:

	////////////////////////////////////////////////////////

private:
};
