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

class CStepperTB6560 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperTB6560();
	virtual void Init() override;

protected:

	virtual void  SetEnable(axis_t axis, unsigned char level, bool force) override;
	virtual unsigned char GetEnable(axis_t axis) override;
	virtual void  Step(const unsigned char cnt[NUM_AXIS], unsigned char directionUp) override;

public:

	virtual bool IsReference(unsigned char referenceid) override;
	virtual bool IsAnyReference() override;

    protected:

////////////////////////////////////////////////////////

private:

};
