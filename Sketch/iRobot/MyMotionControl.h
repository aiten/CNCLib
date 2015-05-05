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

protected:

	virtual void TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;
	virtual void TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS]) override;

private:

	static void ToAngle(mm1000_t x, mm1000_t y, mm1000_t z, float& angle1, float& angle2, float& angle3);
	static void FromAngle(float angle1, float angle2, float angle3, mm1000_t& x, mm1000_t& y, mm1000_t& z);

};

////////////////////////////////////////////////////////
