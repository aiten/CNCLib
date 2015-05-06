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

// Cartesian coordinate system, same as CMotionControlBase
// CMotionControl supports rotation

////////////////////////////////////////////////////////

#include "MotionControlBase.h"

////////////////////////////////////////////////////////

class CMotionControl : public CMotionControlBase
{
private:

	typedef CMotionControlBase super;

public:

	CMotionControl();

	void SetRotate(axis_t axis, double rad);
	void SetOffset(axis_t axis, mm1000_t ofs)	{ _rotateOffset[axis] = ofs; }
	void ClearOffset()							{ for (register unsigned char i=0;i<3;i++) _rotateOffset[i] = 0; }

protected:

	virtual void TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;
	virtual bool TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;

private:

	struct SRotate
	{
		float _sin;
		float _cos;
	};

	SRotate  _rotate[3];
	mm1000_t _rotateOffset[3];

	bool _rotateEnabled[3];

	static void Rotate(const SRotate&rotate, mm1000_t& ax1, mm1000_t& ax2, mm1000_t ofs1, mm1000_t ofs2) ALWAYSINLINE;
	static void RotateInvert(const SRotate&rotate, mm1000_t& ax1, mm1000_t& ax2, mm1000_t ofs1, mm1000_t ofs2) ALWAYSINLINE;
};

////////////////////////////////////////////////////////
