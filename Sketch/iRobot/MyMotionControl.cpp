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
#include <math.h>

#include "CNCLib.h"
#include <GCodeParserBase.h>
#include "MyMotionControl.h"
#include "StepperServo.h"

/////////////////////////////////////////////////////////

#define SEGMENT1	140000.0
#define SEGMENT2	152000.0
#define SEGMENT3	30000.0


#define A SEGMENT2
#define B SEGMENT1
#define H 105000.0	//  105.0   // height start first segement
#define E SEGMENT3	//  30.0	// 3. segment - hor dist from point bb

// c		=> tryangle A/B/C
// s		=> diagonale x/y 
// alpha	=> angle of triangle
// alpha1	=> angle horizontal and c

// segment 2 moves paralell to surface if angle 1 is chaged (

#define SEGMENT2PARALLEL	true


// pos 1.300ms => 55 Grad (from xy pane)

#define DEFAULTANGLE (CENTER_LIMIT / MsForPI * M_PI)
#define ANGLE1OFFSET (DEFAULTANGLE - (55*M_PI/180))

// pos 1.300ms => 80 Grad (between A and B)
//#define ANGLE2OFFSET (DEFAULTANGLE - (80*M_PI/180))
#define ANGLE2OFFSET (DEFAULTANGLE - ((80-55+20)*M_PI/180))
#define ANGLE1TOANGLE2 (M_PI/2)

#define ANGLE3OFFSET DEFAULTANGLE

/////////////////////////////////////////////////////////

CMyMotionControl::CMyMotionControl()
{
#ifdef _MSC_VER

	Test(SEGMENT1 + SEGMENT2 + SEGMENT3, 0, H, true);		// max dist
	Test(SEGMENT2 + SEGMENT3, 0, SEGMENT1  + H, true);		// max height

	Test(SEGMENT2 + SEGMENT3, 0, SEGMENT1  + H, true);		// max height

	Test(200000, 0, H, true);
	Test(200000, 100000, H, true);
	Test(100000, 100000, H, true);

	// test for H = 105

	mm1000_t x, y, z;

	const mm1000_t step = 10000;

	if (true)
	{
		for (x = 0; x <= A + B + E + 1000; x += step)
		{
			Test(x, 0, H, true);
		}
	}

	if (false)
	{
		for (x = 00000; x <= 300000; x += step)
		{
			for (y = 0; y <= 300000; y += step)
			{
				for (z = 0; z <= 300000; z += step)
				{
					Test(x, y, z, true);
				}
			}
		}
	}

	Test(200000, 0, 50000, true);
	Test(200000, 0, 150000, true);
	Test(200000, 50000, 150000, true);
	Test(300000, 150000, 200000, true);
#endif
}

/////////////////////////////////////////////////////////

inline float FromMs(mm1000_t ms,axis_t axis)
{
	return ms / MsForPI * M_PI;
}

/////////////////////////////////////////////////////////

inline mm1000_t ToMs(float angle,axis_t axis)
{
	float msforpi=MsForPI;
	return (mm1000_t)(angle * MsForPI / M_PI);
}

/////////////////////////////////////////////////////////

void CMyMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src,dest);

	float angle1 = FromMs(dest[0],X_AXIS);
	float angle2 = FromMs(dest[1],Y_AXIS);
	float angle3 = FromMs(dest[2],Z_AXIS);

	angle1 -= ANGLE1OFFSET;
	angle2 -= ANGLE2OFFSET;
	angle3 -= ANGLE3OFFSET;

	if (SEGMENT2PARALLEL)
	{
		angle2 -= (angle1-ANGLE1TOANGLE2);
	}

	FromAngle(angle1, angle2, angle3, dest[0], dest[1], dest[2]);
}

/////////////////////////////////////////////////////////

inline bool IsFloatOK(float val)
{
	return !isnan(val) && !isinf(val);
}

bool CMyMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	float angle1, angle2, angle3;
	
	if (!super::TransformPosition(src, dest))
		return false;
		
	if (!ToAngle(dest[0], dest[1], dest[2], angle1, angle2, angle3))
	{
		Error(F("TransformPosition: geometry"));
		return false;
	}

	if (SEGMENT2PARALLEL)
	{
		angle2 += (angle1-ANGLE1TOANGLE2);
	}

	angle1 += ANGLE1OFFSET;
	angle2 += ANGLE2OFFSET;
	angle3 += ANGLE3OFFSET;

	dest[0] = ToMs(angle1,X_AXIS);
	dest[1] = ToMs(angle2,Y_AXIS);
	dest[2] = ToMs(angle3,Z_AXIS);

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

	float alpha1 = (s-E)==0.0 ? 0.0 : atan((z-H)/(s-E));	// "base" angle of c
	float alpha  = acos((B*B + c2 - A*A) / (2.0*B*c));
	float gamma  = acos((A*A + B*B - c2) / (2.0*A*B));

	angle1 = (alpha + alpha1);
	angle2 = gamma;
	angle3 = x==0 ? 0 : atan(y / x);

	if (!IsFloatOK(angle1))	return false;
	if (!IsFloatOK(angle2))	return false;
	if (!IsFloatOK(angle3))	return false;

	return true;
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::FromAngle(float angle1, float angle2, float angle3, mm1000_t& x, mm1000_t& y, mm1000_t& z)
{
	float c2 = (A*A + B*B - 2.0 * A * B * cos(angle2));
	float c = sqrt(c2);
	
	float alpha = acos((c2 + B*B - A*A) / (2.0*B*c));
	float alpha1 = angle1 - alpha;

	float s = cos(alpha1) * c + E;

	x = cos(angle3) * s;
	y = sin(angle3) * s;

	z = H + sin(alpha1)*c;

	return true;
}

/////////////////////////////////////////////////////////

inline int ToGRADRound(float a)   
{ 
	int ia = a*180.0 / M_PI + 0.5;
	return ia; 
}

inline float ToAngleRAD(mm1000_t angle)   
{ 
	return angle / 1000.0 / 180.0 * M_PI;
}

/////////////////////////////////////////////////////////

void CMyMotionControl::MoveAbs(const mm1000_t to[NUM_AXIS], feedrate_t feedrate)
{
	unsigned short movecount = 1;
	mm1000_t nextto[NUM_AXIS];
	mm1000_t totaldist[NUM_AXIS];

	mm1000_t maxdist=0;
	mm1000_t splitdist=10000;

	axis_t i;

	for (i = 0; i < NUM_AXIS; i++)
	{
		mm1000_t dist=to[i]-_current[i];
		totaldist[i] = dist;
		if (dist<0) dist = -dist;

		if (dist >  maxdist)
			maxdist = dist;
	}


	if (maxdist > splitdist)
	{
		movecount = maxdist / splitdist;
		if ((maxdist % splitdist) != 0)
			movecount++;
	}

	for (unsigned short j = movecount-1; j > 0; j--)
	{
		for (i = 0; i < NUM_AXIS; i++)
		{
			mm1000_t newxtpos = RoundMulDivI32(totaldist[i], j, movecount);
			nextto[i] = to[i] - newxtpos;
		}

		super::MoveAbs(nextto,feedrate);
		if (IsError()) return;
	}

	super::MoveAbs(to,feedrate);
}

/////////////////////////////////////////////////////////

void CMyMotionControl::MoveAngle(const mm1000_t dest[NUM_AXIS])
{
	udist_t		to[NUM_AXIS] = { 0 };

	to[0] = ToMs(ToAngleRAD(dest[0]),X_AXIS);
	to[1] = ToMs(ToAngleRAD(dest[1]),Y_AXIS);
	to[2] = ToMs(ToAngleRAD(dest[2]),Z_AXIS);

	for (axis_t i= 0;i<NUM_AXIS;i++)
	{
		if (to[i]==0)
			to[i] = CStepper::GetInstance()->GetCurrentPosition(i);
	}

	CStepper::GetInstance()->MoveAbs(to);
	SetPositionFromMachine();
}

/////////////////////////////////////////////////////////

void CMyMotionControl::MoveAngleLog(const mm1000_t dest[NUM_AXIS])
{
	float angle1 = FromMs(CStepper::GetInstance()->GetCurrentPosition(X_AXIS),X_AXIS);
	float angle2 = FromMs(CStepper::GetInstance()->GetCurrentPosition(Y_AXIS),Y_AXIS);
	float angle3 = FromMs(CStepper::GetInstance()->GetCurrentPosition(Z_AXIS),Z_AXIS);

	angle1 -= ANGLE1OFFSET;
	angle2 -= ANGLE2OFFSET;
	angle3 -= ANGLE3OFFSET;

	if (SEGMENT2PARALLEL)
	{
		angle2 -= (angle1-ANGLE1TOANGLE2);
	}

	mm1000_t to[NUM_AXIS];
	memcpy(to,_current,sizeof(_current));

	if (dest[0]!=0)  angle1 = ToAngleRAD(dest[0]);
	if (dest[1]!=0)  angle2 = ToAngleRAD(dest[1]);
	if (dest[2]!=0)  angle3 = ToAngleRAD(dest[2]);

	FromAngle(angle1,angle2,angle3, to[0], to[1], to[2]);

	MoveAbs(to,CGCodeParserBase::GetG0FeedRate());
}

/////////////////////////////////////////////////////////

void CMyMotionControl::PrintInfo()
{
	float angle1, angle2, angle3;

	
	if (!ToAngle(_current[0], _current[1], _current[2], angle1, angle2, angle3))
	{
		Error(F("TransformPosition: geometry"));
	}

	char tmp[16];

	StepperSerial.print(ToGRADRound(angle1)); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle2)); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle3)); StepperSerial.print(F("=>"));

	if (SEGMENT2PARALLEL)
	{
		angle2 += (angle1-ANGLE1TOANGLE2);
	}

	angle1 += ANGLE1OFFSET;
	angle2 += ANGLE2OFFSET;
	angle3 += ANGLE3OFFSET;

	StepperSerial.print(ToGRADRound(angle1)); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle2)); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle3)); StepperSerial.print(F("=>"));

	StepperSerial.print(CMm1000::ToString(ToMs(angle1,X_AXIS), tmp, 3)); StepperSerial.print(F(":"));
	StepperSerial.print(CMm1000::ToString(ToMs(angle2,Y_AXIS), tmp, 3)); StepperSerial.print(F(":"));
	StepperSerial.print(CMm1000::ToString(ToMs(angle3,Z_AXIS), tmp, 3));
}


/////////////////////////////////////////////////////////

#ifdef _MSC_VER

inline float ToRAD(float a)   { return (a*180.0 / M_PI); }
inline float ToMM(mm1000_t a) { return (a / 1000.0); }
inline bool CompareMaxDiff(mm1000_t a, mm1000_t b, mm1000_t diff = 10) { return  (abs(a - b) >= diff); }

#define FORMAT_MM "%.0f:%.0f:%.0f"
#define FORMAT_GRAD "%.0f:%.0f:%.0f"

bool CMyMotionControl::Test(mm1000_t src1, mm1000_t src2, mm1000_t src3, bool printOK)
{
	float angle1, angle2, angle3;

	if (!ToAngle(src1, src2, src3, angle1, angle2, angle3))
		return false;

	float a1, a2, a3;

	if (false)
	{
		mm1000_t tmp1 = ToMs(angle1, X_AXIS);
		mm1000_t tmp2 = ToMs(angle2, Y_AXIS);
		mm1000_t tmp3 = ToMs(angle3, Z_AXIS);

		a1 = FromMs(tmp1, X_AXIS);
		a2 = FromMs(tmp2, Y_AXIS);
		a3 = FromMs(tmp3, Z_AXIS);
	}
	else
	{
		a1 = angle1;
		a2 = angle2;
		a3 = angle3;
	}

	mm1000_t dest1, dest2, dest3;

	FromAngle(a1, a2, a3, dest1, dest2, dest3);

	bool isError = CompareMaxDiff(src1, dest1) || CompareMaxDiff(src2, dest2) || CompareMaxDiff(src3, dest3);

	if (printOK || isError)
	{
		printf(FORMAT_MM" => ", ToMM(src1), ToMM(src2), ToMM(src3));
		printf(FORMAT_GRAD" = > ", ToRAD(angle1), ToRAD(angle2), ToRAD(angle3));
		printf(FORMAT_MM, ToMM(dest1), ToMM(dest2), ToMM(dest3));

		if (isError)
			printf(" ERROR");

		printf("\n");
	}

	return isError;
}

#endif
