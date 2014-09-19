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

#define SMC800_NUM_AXIS	3

////////////////////////////////////////////////////////

class CStepperSMC800 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperSMC800();
	virtual void Init();
	virtual void Remove();

protected:

#define LevelToProcent(a) (a*100/255)
#define ProcentToLevel(a) (a*255/100)

	enum ELevel
	{
		Level0  = LevelOff,
		Level20 = ProcentToLevel(20),
		Level60 = ProcentToLevel(60),
		Level100 = LevelMax
	};

public:

	virtual bool IsAnyReference()							{ return IsReference(0); };
	virtual bool IsReference(unsigned char referenceid);

protected:

	virtual void  Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp);
	virtual void  SetEnable(axis_t axis, unsigned char level);
	virtual unsigned char GetEnable(axis_t axis);

	virtual void MoveAwayFromReference(axis_t axis, sdist_t diff, steprate_t vMax);

	////////////////////////////////////////////////////////

private:

	void InitMemVar();

	unsigned char _stepIdx[SMC800_NUM_AXIS];
	unsigned char _level[SMC800_NUM_AXIS];

	void   SetPhase(axis_t axis);
	static void OutSMC800Cmd(const unsigned char val);
};
