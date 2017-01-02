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
	static pin_t _pinRef[NUM_AXIS*2];
	static uint8_t _referenceOn;

protected:

	virtual void  SetEnable(axis_t axis, uint8_t level, bool force) override;
	virtual uint8_t GetEnable(axis_t axis) override;
	virtual void  Step(const uint8_t cnt[NUM_AXIS], axisArray_t directionUp) override;

public:

	virtual bool IsAnyReference() override;
	virtual bool IsReference(uint8_t referenceid) override;

	// Set before Init()
	void SetPin(axis_t axis, pin_t in1, pin_t in2, pin_t in3, pin_t in4) { _pin[axis][0] = in1;  _pin[axis][1] = in2; _pin[axis][2] = in3; _pin[axis][3] = in4; }
	void SetPin(axis_t axis, pin_t in1, pin_t in2)					{ _pin[axis][0] = in1;  _pin[axis][1] = in2; _pin[axis][2] = 0; _pin[axis][3] = 0; }
	void SetEnablePin(axis_t axis, pin_t en1, pin_t en2)			{ _pinenable[axis][0] = en1;  _pinenable[axis][1] = en2; }
	void SetRefPin(axis_t axis, pin_t refmin, pin_t refmax)			{ _pinRef[ToReferenceId(axis, true)] = refmin;  _pinRef[ToReferenceId(axis, false)] = refmax; }

	void SetFullStepMode(axis_t axis, bool fullstepMode)			{ _fullStepMode[axis] = fullstepMode; };

private:

	bool IsActive(axis_t axis)										{ return _pin[axis][0] != 0; }
	bool Is4Pin(axis_t axis)										{ return _pin[axis][2] != 0; }
	bool Is2Pin(axis_t axis)										{ return _pin[axis][2] == 0; }

	bool IsUseEN1(axis_t axis)										{ return _pinenable[axis][0] != 0; }
	bool IsUseEN2(axis_t axis)										{ return _pinenable[axis][1] != 0; }

	uint8_t _stepIdx[NUM_AXIS];
	bool _fullStepMode[NUM_AXIS];

	void InitMemVar();

	void   SetPhase(axis_t axis);

	////////////////////////////////////////////////////////
};








