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

#include <MotionControl.h>

////////////////////////////////////////////////////////

class CMyMotionControl : public CMotionControl
{
private:

	typedef CMotionControl super;

public:

	CMyMotionControl();

	void PrintInfo();

	void MoveAngle(const mm1000_t dest[NUM_AXIS]);
	void MoveAngleLog(const mm1000_t dest[NUM_AXIS]);
	virtual void MoveAbs(const mm1000_t to[NUM_AXIS], feedrate_t feedrate) override;

protected:

	virtual void TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;
	virtual bool TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;

private:

//	static bool ToAngle(mm1000_t x, mm1000_t y, mm1000_t z, float& angle1, float& angle2, float& angle3);
//	static bool FromAngle(float angle1, float angle2, float angle3, mm1000_t& x, mm1000_t& y, mm1000_t& z);

	static bool ToAngle(const mm1000_t pos[NUM_AXIS], float angle[NUM_AXIS]);
	static bool FromAngle(const float angle[NUM_AXIS], mm1000_t dest[NUM_AXIS]);

	static void AdjustToAngle(float angle[NUM_AXIS]);
	static void AdjustFromAngle(float angle[NUM_AXIS]);

	void Test();
	bool Test(mm1000_t dest1, mm1000_t dest2, mm1000_t dest3,bool printOK);
};

////////////////////////////////////////////////////////
