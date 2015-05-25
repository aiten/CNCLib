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

#define SEGMENTCOUNT 3

#define SEGMENT1	140000.0
#define SEGMENT2	152000.0
#define SEGMENT3	30000.0

#define A SEGMENT2
#define B SEGMENT1
#define H 105000.0	//  105.0   // height start first segement
#define E SEGMENT3	

// c		=> tryangle A/B/C
// s		=> diagonale x/y 
// alpha	=> angle of triangle
// alpha1	=> angle horizontal and c

// segment 2 moves paralell to surface if angle 1 is chaged 

#define SEGMENT2PARALLEL

// pos 1.300ms => 55 Grad (from xy pane)
#define DEFAULTANGLE (CENTER_LIMIT / MsForPI * M_PI)
#define ANGLE1OFFSET (DEFAULTANGLE - (55*M_PI/180))

// pos 1.300ms => 80 Grad (between A and B)
#define ANGLE1TOANGLE2 (M_PI/2)
#define ANGLE2OFFSET ((DEFAULTANGLE - ((80-55+20)*M_PI/180)) - ANGLE1TOANGLE2)

#define ANGLE3OFFSET DEFAULTANGLE

#define SPLITMOVEDIST	10000

/////////////////////////////////////////////////////////

CMyMotionControl::CMyMotionControl()
{
}

/////////////////////////////////////////////////////////

inline float FromMs(mm1000_t ms,axis_t /* axis */)
{
	return ms / MsForPI * M_PI;
}

/////////////////////////////////////////////////////////

inline mm1000_t ToMs(float angle,axis_t /* axis */)
{
	return (mm1000_t)(angle * MsForPI / M_PI);
}

/////////////////////////////////////////////////////////

inline bool IsFloatOK(float val)
{
	return !isnan(val) && !isinf(val);
}

/////////////////////////////////////////////////////////

inline int ToGRADRound(float a)
{
	return (int) (a*180.0 / M_PI + 0.5);
}

/////////////////////////////////////////////////////////

inline float ToAngleRAD(mm1000_t angle)
{
	return angle / 1000.0 / 180.0 * M_PI;
}

/////////////////////////////////////////////////////////

void CMyMotionControl::AdjustToAngle(float angle[NUM_AXIS])
{
#ifdef SEGMENT2PARALLEL
	angle[1] += angle[0];
#endif

	angle[0] += ANGLE1OFFSET;
	angle[1] += ANGLE2OFFSET;
	angle[2] += ANGLE3OFFSET;
}

/////////////////////////////////////////////////////////

void CMyMotionControl::AdjustFromAngle(float angle[NUM_AXIS])
{
	angle[0] -= ANGLE1OFFSET;
	angle[1] -= ANGLE2OFFSET;
	angle[2] -= ANGLE3OFFSET;

#ifdef SEGMENT2PARALLEL
	angle[1] -= angle[0];
#endif
}

/////////////////////////////////////////////////////////

void CMyMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src,dest);

	float angle[NUM_AXIS];

	for (axis_t i = 0; i < SEGMENTCOUNT; i++)
		angle[i] = FromMs(dest[i], i);

	AdjustFromAngle(angle);
	FromAngle(angle, dest);
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	float angle[NUM_AXIS];
	
	if (!super::TransformPosition(src, dest))
		return false;
		
	if (!ToAngle(dest, angle))
	{
		Error(F("TransformPosition: geometry"));
		return false;
	}

	AdjustToAngle(angle);

	for (axis_t i = 0; i < SEGMENTCOUNT; i++)
		dest[i] = ToMs(angle[i], X_AXIS);

	return true;
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::ToAngle(const mm1000_t pos[NUM_AXIS], float angle[NUM_AXIS])
{
	float x = (float) pos[0];
	float y = (float) pos[1];
	float z = (float) pos[2];

	float s = sqrt(x*x + y*y);

	float c2 = (s - E)*(s - E) + (z - H)*(z - H);
	float c = sqrt(c2);										// triangle for first and second segment

	float alpha1 = (s - E) == 0.0 ? 0.0 : atan((z - H) / (s - E));	// "base" angle of c
	float alpha = acos((B*B + c2 - A*A) / (2.0*B*c));
	float gamma = acos((A*A + B*B - c2) / (2.0*A*B));

	angle[0] = (alpha + alpha1);
	angle[1] = gamma;
	if (x==0.0)
	{
		angle[2] = y>0.0 ? (M_PI/2.0) : -(M_PI/2.0);
	}
	else
	{
		angle[2] = atan(y / x);
	}
	if (x<0.0) 
	{
		angle[2] = M_PI + angle[2];
		if (angle[2] >= M_PI)
			angle[2] -= 2.0 * M_PI;
	}

	if (!IsFloatOK(angle[0]))	return false;
	if (!IsFloatOK(angle[1]))	return false;
	if (!IsFloatOK(angle[2]))	return false;

	return true;
}

/////////////////////////////////////////////////////////

bool CMyMotionControl::FromAngle(const float angle[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	float c2 = (A*A + B*B - 2.0 * A * B * cos(angle[1]));
	float c = sqrt(c2);

	float alpha = acos((c2 + B*B - A*A) / (2.0*B*c));
	float alpha1 = angle[0] - alpha;

	float s = cos(alpha1) * c + E;

	dest[0] = (mm1000_t) lrint(cos(angle[2]) * s);
	dest[1] = (mm1000_t) lrint(sin(angle[2]) * s);

	dest[2] = (mm1000_t) lrint(H + sin(alpha1)*c);

	return true;
}

/////////////////////////////////////////////////////////

void CMyMotionControl::MoveAbs(const mm1000_t to[NUM_AXIS], feedrate_t feedrate)
{
	unsigned short movecount = 1;
	mm1000_t nextto[NUM_AXIS];
	mm1000_t totaldist[NUM_AXIS];

	mm1000_t maxdist=0;
	const mm1000_t splitdist=SPLITMOVEDIST;

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

	axis_t i;

	for (i = 0; i<SEGMENTCOUNT; i++)
		to[i] = ToMs(ToAngleRAD(dest[i]), i);

	for (i= 0;i<NUM_AXIS;i++)
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
	axis_t i;
	float angle[NUM_AXIS] = { 0.0 };

	for (i = 0; i<SEGMENTCOUNT; i++)
		angle[i] = FromMs(CStepper::GetInstance()->GetCurrentPosition(i), i);

	AdjustFromAngle(angle);

	mm1000_t to[NUM_AXIS];
	memcpy(to,_current,sizeof(_current));

	for (i = 0; i<SEGMENTCOUNT; i++)
		if (dest[i] != 0)  angle[i] = ToAngleRAD(dest[i]);

	FromAngle(angle, to);

	MoveAbs(to,CGCodeParserBase::GetG1FeedRate());
}

/////////////////////////////////////////////////////////

void CMyMotionControl::PrintInfo()
{
	float angle[NUM_AXIS];
	
	if (!ToAngle(_current, angle))
	{
		Error(F("TransformPosition: geometry"));
	}

	char tmp[16];

	StepperSerial.print(ToGRADRound(angle[0])); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle[1])); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle[2])); StepperSerial.print(F("=>"));

	AdjustToAngle(angle);

	StepperSerial.print(ToGRADRound(angle[0])); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle[1])); StepperSerial.print(F(":"));
	StepperSerial.print(ToGRADRound(angle[2])); StepperSerial.print(F("=>"));

	StepperSerial.print(CMm1000::ToString(ToMs(angle[0],X_AXIS), tmp, 3)); StepperSerial.print(F(":"));
	StepperSerial.print(CMm1000::ToString(ToMs(angle[1],Y_AXIS), tmp, 3)); StepperSerial.print(F(":"));
	StepperSerial.print(CMm1000::ToString(ToMs(angle[2],Z_AXIS), tmp, 3));
}


/////////////////////////////////////////////////////////

#ifdef _MSC_VER

void CMyMotionControl::UnitTest()
{
	Test(1, 200, H, true);		// max dist
	Test(0, 200, H, true);		// max dist
	Test(-1, 200, H, true);		// max dist

	Test(1, -200, H, true);		// max dist
	Test(0, -200, H, true);		// max dist
	Test(-1, -200, H, true);		// max dist


	Test(SEGMENT1 + SEGMENT2 + SEGMENT3, 0, H, true);		// max dist
	Test(SEGMENT2 + SEGMENT3, 0, SEGMENT1 + H, true);		// max height

	Test(SEGMENT2 + SEGMENT3, 0, SEGMENT1 + H, true);		// max height

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
}

inline float ToRAD(float a)   { return (a*180.0 / M_PI); }
inline float ToMM(mm1000_t a) { return (a / 1000.0); }
inline bool CompareMaxDiff(mm1000_t a, mm1000_t b, mm1000_t diff = 10) { return  (abs(a - b) >= diff); }

#define FORMAT_MM "%.0f:%.0f:%.0f"
#define FORMAT_GRAD "%.0f:%.0f:%.0f"

bool CMyMotionControl::Test(mm1000_t src1, mm1000_t src2, mm1000_t src3, bool printOK)
{
	float angle[NUM_AXIS];
	mm1000_t src[NUM_AXIS] = { src1, src2, src3 };
	axis_t i;

	if (!ToAngle(src, angle))
		return false;

	float a[NUM_AXIS] = { 0.0 };

	if (false)
	{
		for (i = 0; i < SEGMENTCOUNT; i++)
		{
			mm1000_t tmp = ToMs(angle[i], i);
			a[i] = FromMs(tmp, i);
		}
	}
	else
	{
		for (i = 0; i < SEGMENTCOUNT; i++)
			a[i] = angle[i];
	}

	mm1000_t dest[NUM_AXIS];

	FromAngle(a, dest);

	bool isError = false;
	for (i = 0; i < SEGMENTCOUNT && !isError; i++)
		isError = CompareMaxDiff(src[i], dest[i]);

	if (printOK || isError)
	{
		printf(FORMAT_MM" => ", ToMM(src1), ToMM(src2), ToMM(src3));
		printf(FORMAT_GRAD" = > ", ToRAD(angle[0]), ToRAD(angle[1]), ToRAD(angle[2]));
		printf(FORMAT_MM, ToMM(dest[0]), ToMM(dest[1]), ToMM(dest[2]));

		if (isError)
			printf(" ERROR");

		printf("\n");
	}

	return isError;
}

#endif
