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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "CNCLib.h"
#include "MyMotionControl.h"

/////////////////////////////////////////////////////////

#define A 152000.0	//  140.0;	// second segment
#define B 140000.0	//  152.0;	// first segment
#define H 105000.0	//  105.0   // height start first segement
#define E 30000.0		//  30.0	// 3. segment

#define ANGLE1ADD (M_PI/2.0)
#define ANGLE2ADD (0.0)
#define ANGLE3ADD (M_PI/2.0)

/////////////////////////////////////////////////////////

CMyMotionControl::CMyMotionControl()
{
#ifdef _MSC_VER

	Test(200000, 0, H);
	Test(200000, 100000, H);
	Test(100000, 100000, H);
	Test(A+B+E, 0, H);

	Test(200000, 0, 50000);
	Test(200000, 0, 150000);
	Test(200000, 50000, 150000);
	Test(300000, 150000, 200000);
#endif
}

/////////////////////////////////////////////////////////

inline float FromMs(mm1000_t ms)
{
	return ms / (1.0 / M_PI*2.0*1000.0);
}

/////////////////////////////////////////////////////////

inline mm1000_t ToMs(float angle)
{

	// 1000 => 90 (1024 => 90)
	// PI/2 == 1000

	return (mm1000_t)(angle * (1.0 / M_PI*2.0*1000.0));
}

/////////////////////////////////////////////////////////

void CMyMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src,dest);

	FromAngle(FromMs(dest[0]), FromMs(dest[1]), FromMs(dest[2]), dest[0], dest[1], dest[2]);
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	float angle1, angle2, angle3;
	
	if (!super::TransformPosition(src, dest) || !ToAngle(dest[0], dest[1], dest[2], angle1, angle2, angle3))
		return false;

	dest[0] = ToMs(angle1);
	dest[1] = ToMs(angle2);
	dest[2] = ToMs(angle3);

	return true;
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::ToAngle(mm1000_t ix, mm1000_t iy, mm1000_t iz, float& angle1, float& angle2, float& angle3)
{
	float y = iy;
	float x = ix;
	float z = iz;

	float s = sqrt(x*x + y*y);

	float c2 = (s-E)*(s-E) + (z-H)*(z-H);
	float c = sqrt(c2);										// triangle for first and second segment

	float alpha1 = atan((z-H) / (s-E));						// "base" angle of c
	float alpha  = acos((B*B + c2 - A*A) / (2.0*B*c));
	float gamma  = acos((A*A + B*B - c2) / (2.0*A*B));

	angle1 = (alpha + alpha1) + ANGLE1ADD;
	angle2 = gamma + ANGLE2ADD;;
	angle3 = atan(y / x) + ANGLE3ADD;;

	return true;
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::FromAngle(float angle1, float angle2, float angle3, mm1000_t& x, mm1000_t& y, mm1000_t& z)
{
	float c2 = (A*A + B*B - 2.0 * A * B * cos(angle2 - ANGLE2ADD));
	float c = sqrt(c2);
	
	float alpha = acos((c2 + B*B - A*A) / (2.0*B*c));
	float alpha1 = angle1 - ANGLE1ADD - alpha;

	float s = cos(alpha1) * c + E;

	x = cos(angle3 - ANGLE3ADD) * s;
	y = sin(angle3 - ANGLE3ADD) * s;

	z = H + sin(alpha1)*c;

	return true;
}


/////////////////////////////////////////////////////////

#ifdef _MSC_VER

bool CMyMotionControl::Test(mm1000_t src1, mm1000_t src2, mm1000_t src3)
{
	float angle1, angle2, angle3;

	if (!ToAngle(src1, src2, src3, angle1, angle2, angle3))
		return false;

	float a1, a2, a3;

	if (false)
	{
		mm1000_t tmp1 = ToMs(angle1);
		mm1000_t tmp2 = ToMs(angle2);
		mm1000_t tmp3 = ToMs(angle3);

		a1 = FromMs(tmp1);
		a2 = FromMs(tmp2);
		a3 = FromMs(tmp3);
	}
	else
	{
		a1 = angle1;
		a2 = angle2;
		a3 = angle3;
	}

	mm1000_t dest1, dest2, dest3;

	FromAngle(a1, a2, a3, dest1, dest2, dest3);

	return true;
}

#endif
