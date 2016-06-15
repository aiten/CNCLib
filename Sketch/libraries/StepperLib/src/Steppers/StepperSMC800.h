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

#include "Stepper.h"

////////////////////////////////////////////////////////

#define SMC800_NUM_AXIS	3

////////////////////////////////////////////////////////

class CStepperSMC800 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperSMC800();
	virtual void Init() override;
	void Remove();

public:

	virtual bool IsAnyReference() override							{ return IsReference(0); };
	virtual bool IsReference(uint8_t referenceid) override;

	void SetFullStepMode(axis_t axis, bool fullstepMode)			{ _fullStepMode[axis] = fullstepMode; };

protected:

	virtual void  Step(const uint8_t steps[NUM_AXIS], axisArray_t directionUp) override;
	virtual void  SetEnable(axis_t axis, uint8_t level, bool force) override;
	virtual uint8_t GetEnable(axis_t axis) override;

	virtual void MoveAwayFromReference(axis_t axis, sdist_t diff, steprate_t vMax) override;

	////////////////////////////////////////////////////////

private:

	void InitMemVar();

	uint8_t _stepIdx[SMC800_NUM_AXIS];
	uint8_t _level[SMC800_NUM_AXIS];
	bool		  _fullStepMode[NUM_AXIS];

	void   SetPhase(axis_t axis);
	static void OutSMC800Cmd(const uint8_t val);
};
