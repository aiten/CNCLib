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

#define RAD2GRAD(a) ((a)*180.0/M_PI)

/////////////////////////////////////////////////////////


CMyMotionControl::CMyMotionControl()
{
}

/////////////////////////////////////////////////////////

void CMyMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src,dest);
}

/////////////////////////////////////////////////////////

void CMyMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformPosition(src, dest);

	float angle1, angle2, angle3;
	ToAngle(dest[0], dest[1], dest[2], angle1, angle2, angle3);

	dest[0] = angle1 * 1024;
	dest[1] = angle2 * 1024;
	dest[2] = angle3 * 1024;
}

/////////////////////////////////////////////////////////

void CMyMotionControl::ToAngle(mm1000_t x, mm1000_t y, mm1000_t z, float& angle1, float& angle2, float& angle3)
{
	float a = 140000;	//  140.0;	// second segment
	float b = 152000;	//  152.0;	// first segment
	float dz = 55000;	//  55.0    // height start first segement
	float dx = 30000;	//  30.0	// 3. segment

	float a2 = a*a;
	float b2 = b*b;

	float fy = y;
	float fx = x;
	float fz = z-dz;

	float fxx2 = fx*fx + fy*fy;

	float c2 = fxx2 + fz*fz;
	float c = sqrt(c2);								 // triangle for first and second segment

	float alpha1 = atan(fz / fx);					 // "base" angle of c
	float alpha  = acos((b2 + c2 - a2) / (2.0*b*c));
	//float beta = acos((c2 + a2 - b2) / (2.0*c*a));
	float gamma  = acos((a2 + b2 - c2) / (2.0*a*b));

	//float test = alpha + beta + gamma; // must be M_PI

	angle1 = RAD2GRAD(alpha + alpha1) + 90.0;
	angle2 = RAD2GRAD(gamma);
	angle3 = RAD2GRAD(tan(fy/fx)) + 90.0;
}
