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
#include "UtilitiesCNCLib.h"

////////////////////////////////////////////////////////

class CMotionControl : public CMotionControlBase
{
private:

	typedef CMotionControlBase super;

public:

	CMotionControl();

	void SetRotate(float rad, const mm1000_t vect[NUM_AXISXYZ], const mm1000_t ofs[NUM_AXISXYZ]);
	void ClearRotate()												{ _rotateType = NoRotate; }
	bool IsRotate()													{ return _rotateType != NoRotate; }
	mm1000_t GetOffset(axis_t axis)									{ return _rotateOffset[axis]; }
	mm1000_t GetVector(axis_t axis)									{ return _vect[axis]; }
	float GetAngle()												{ return _angle; }

	void SetRotate2D(float alpha, float beta, float gamma, const mm1000_t ofs[NUM_AXISXYZ]);
	void SetRotate2D(axis_t axis, float rad);
	void SetOffset2D(const mm1000_t ofs[NUM_AXISXYZ]);
	mm1000_t GetOffset2D(axis_t axis)								{ return _rotateOffset2D[axis]; }
	float GetAngle2D(axis_t axis)									{ return _rotate2D[axis].GetAngle(); }
	bool IsEnabled2D(axis_t axis)									{ return IsBitSet(_rotateEnabled2D,axis); }
	void ClearRotate2D()											{ _rotateEnabled2D=0; }

	static CMotionControl* GetInstance()							{ return (CMotionControl*) CMotionControlBase::GetInstance(); } 

protected:

	virtual void TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;
	virtual bool TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;

private:

	float _angle;
	mm1000_t _vect[NUM_AXISXYZ];

	struct SRotate3D			// Performance Array for matrix
	{
		float _vect[NUM_AXISXYZ][NUM_AXISXYZ];
		void Set(float rad, const mm1000_t vect[NUM_AXISXYZ]);

		void Rotate(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXISXYZ], mm1000_t dest[NUM_AXIS]);
	};

	SRotate3D  _rotate3D;
	mm1000_t _rotateOffset[NUM_AXISXYZ];

	enum ERotateType
	{
		NoRotate=0,
		Rotate,
		RotateInvert
	};

	EnumAsByte(ERotateType) _rotateType=NoRotate;

private:

	struct SRotate
	{
		float _sin;
		float _cos;
		float _angle;

		void Set(float rad)
		{
			_angle = rad;
			_sin = sin(rad);
			_cos = cos(rad);
		}

		float GetAngle()	{ return _angle; }

		void Rotate(mm1000_t& ax1, mm1000_t& ax2, mm1000_t ofs1, mm1000_t ofs2) const ALWAYSINLINE
		{
			// rotate with positive angle
			float fx = (float) (ax1 - ofs1);
			float fy = (float) (ax2 - ofs2);
			ax1 = CMm1000::Convert(fx*_cos - fy*_sin) + ofs1;
			ax2 = CMm1000::Convert(fy*_cos + fx*_sin) + ofs2;
		}

		void RotateInvert(mm1000_t& ax1, mm1000_t& ax2, mm1000_t ofs1, mm1000_t ofs2) const ALWAYSINLINE
		{
			// rotate with negative angle (e.g. from 30 to -30)
			float fx = (float)(ax1 - ofs1);
			float fy = (float)(ax2 - ofs2);
			ax1 = CMm1000::Convert(fx*_cos + fy*_sin) + ofs1;
			ax2 = CMm1000::Convert(fy*_cos - fx*_sin) + ofs2;
		}
	};

	SRotate  _rotate2D[NUM_AXISXYZ];
	mm1000_t _rotateOffset2D[NUM_AXISXYZ];

	axisArray_t _rotateEnabled2D=0;

#ifdef _MSC_VER

public:

	virtual void UnitTest() override;
	bool Test3D(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXISXYZ],mm1000_t dest[NUM_AXIS], mm1000_t vect[NUM_AXISXYZ], float angle, bool pintOK);
	bool Test2D(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXISXYZ],mm1000_t dest[NUM_AXIS], float angle[NUM_AXISXYZ], bool pintOK);

	bool Test(const mm1000_t src[NUM_AXIS],const mm1000_t ofs[NUM_AXISXYZ],mm1000_t dest[NUM_AXIS], bool printOK, std::function<void()> print);

#endif
};

////////////////////////////////////////////////////////
