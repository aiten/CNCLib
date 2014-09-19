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

class CStepperL298N : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperL298N();
	virtual void Init(void);

protected:

	static pin_t _pin[NUM_AXIS][4];
	static pin_t _pinenable[NUM_AXIS][2];

protected:

	virtual void  SetEnable(axis_t axis, unsigned char level, bool force);
	virtual unsigned char GetEnable(axis_t axis);
	virtual void  Step(const unsigned char cnt[NUM_AXIS], axisArray_t directionUp);

public:

	virtual bool IsAnyReference()									{ return IsReference(0); };
	virtual bool IsReference(unsigned char /* referenceid */)		{ return 0; }

	// Set before Init()
	void SetPin(axis_t axis, pin_t in1, pin_t in2, pin_t in3, pin_t in4) { _pin[axis][0] = in1;  _pin[axis][1] = in2; _pin[axis][2] = in3; _pin[axis][3] = in4; }
	void SetPin(axis_t axis, pin_t in1, pin_t in2)					{ _pin[axis][0] = in1;  _pin[axis][1] = in2; _pin[axis][2] = 0; _pin[axis][3] = 0; }
	void SetEnablePin(axis_t axis, pin_t en1, pin_t en2)			{ _pinenable[axis][0] = en1;  _pinenable[axis][1] = en2; }

private:

	bool IsActive(axis_t axis)										{ return _pin[axis][0] != 0; }
	bool Is4Pin(axis_t axis)										{ return _pin[axis][2] != 0; }
	bool Is2Pin(axis_t axis)										{ return _pin[axis][2] == 0; }

	bool IsUseEN1(axis_t axis)										{ return _pinenable[axis][0] != 0; }
	bool IsUseEN2(axis_t axis)										{ return _pinenable[axis][1] != 0; }

	unsigned char _stepIdx[NUM_AXIS];

	void InitMemVar();

	void   SetPhase(axis_t axis);

	////////////////////////////////////////////////////////
};








