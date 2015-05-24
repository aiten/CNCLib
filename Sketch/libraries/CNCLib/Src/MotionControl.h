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

	void SetRotate(float rad, const mm1000_t vect[NUM_AXIS], const mm1000_t ofs[NUM_AXIS]);
	void ClearRotate()												{ _rotateType = NoRotate; }
	bool IsRotate()													{ return _rotateType != NoRotate; }

protected:

	virtual void TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;
	virtual bool TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;

private:

	float _angle;
	mm1000_t _vect[3];

	struct SRotate3D			// Performance Array for matrix
	{
		float _vect[3][3];
		void Set(float rad, const mm1000_t vect[NUM_AXIS]);

		void Rotate(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXIS], mm1000_t dest[NUM_AXIS]);
	};

	SRotate3D  _rotate3D;
	mm1000_t _rotateOffset[3];

	enum ERotateType
	{
		NoRotate=0,
		Rotate,
		RotateInvert
	};

	EnumAsByte(ERotateType) _rotateType=NoRotate;

#ifdef _MSC_VER

public:

	virtual void UnitTest() override;
	bool Test(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXIS],mm1000_t dest[NUM_AXIS], mm1000_t vect[NUM_AXIS], float angle, bool pintOK);

#endif
};

////////////////////////////////////////////////////////
