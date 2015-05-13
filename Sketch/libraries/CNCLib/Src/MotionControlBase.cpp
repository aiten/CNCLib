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
#include "MotionControlBase.h"

#ifdef _MSC_VER
#include "Control.h"
#endif

/////////////////////////////////////////////////////////

ToMm1000_t CMotionControlBase::_ToMm1000;
ToMachine_t CMotionControlBase::_ToMachine;

/////////////////////////////////////////////////////////

template<> CMotionControlBase* CSingleton<CMotionControlBase>::_instance = NULL;

/////////////////////////////////////////////////////////

void CMotionControlBase::GetPositions(mm1000_t current[NUM_AXIS])
{
	memcpy(current, _current, sizeof(_current));
}

/////////////////////////////////////////////////////////

mm1000_t CMotionControlBase::GetPosition(axis_t axis)
{
	return _current[axis];
}

/////////////////////////////////////////////////////////

void CMotionControlBase::SetPositionFromMachine()
{
	TransformFromMachinePosition(CStepper::GetInstance()->GetPositions(), _current);
}

/////////////////////////////////////////////////////////

void CMotionControlBase::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	ToMm1000(src,dest);
}

/////////////////////////////////////////////////////////

bool CMotionControlBase::TransformPosition(const mm1000_t /* src */[NUM_AXIS], mm1000_t /* dest */[NUM_AXIS])
{
	return true;
}

/////////////////////////////////////////////////////////

void CMotionControlBase::MoveAbs(const mm1000_t to[NUM_AXIS], feedrate_t feedrate)
{
	// the ONLY methode to move!!!!!
	// do not call Stepper direct

#ifdef _MSC_VER
	CStepper::GetInstance()->MSCInfo = CControl::GetInstance()->GetBuffer();
#endif

	mm1000_t	to_proj[NUM_AXIS];
	udist_t		to_m[NUM_AXIS];

	memcpy(to_proj, to, sizeof(_current));

	if (TransformPosition(to, to_proj))
	{
		ToMachine(to_proj, to_m);

		CStepper::GetInstance()->MoveAbs(to_m, GetFeedRate(to_proj, feedrate));

		if (!CStepper::GetInstance()->IsError())
			memcpy(_current, to, sizeof(_current));
	}
}

/////////////////////////////////////////////////////////
// based on:
//	motion_control.c - high level interface for issuing motion commands
//	Part of Grbl

// Arc with axis_0 and axis_1
// all other linear
// calculate Segments with f = k * x + d (x... radius, f ... segments)

#define SEGMENTS_K	4.0

#define SEGMENTS_D	18.0		// 20 Grad 
//#define SEGMENTS_D	20.0		// 18 Grad
//#define SEGMENTS_D	22.0		// 16.3
//#define SEGMENTS_D	24.0		// 15
//#define SEGMENTS_D	36.0		// 10

// Use: Small angle approximation from http://en.wikipedia.org/wiki/Small-angle_approximation
/*
Figure 3 shows the relative errors of the small angle approximations.The angles at which the relative error exceeds 1 % are as follows :
tan O near O at about 0.176 radians(10Grad).
sin O near O at about 0.244 radians(14Grad).
cos O near 1 - O2 / 2 at about 0.664 radians(38Grad).

=> To be save => not more than 10Grad for Arc_Correction
*/

#define ARCCORRECTION	( 10.0 * M_PI / 180.0)		// every 10grad

void CMotionControlBase::Arc(const mm1000_t to[NUM_AXIS], mm1000_t offset0, mm1000_t offset1, axis_t  axis_0, axis_t axis_1, bool isclockwise, feedrate_t feedrate)
{
	// start from current position!

	mm1000_t current[NUM_AXIS];
	GetPositions(current);

	mm1000_t center_axis0 = current[axis_0] + offset0;
	mm1000_t center_axis1 = current[axis_1] + offset1;

	mm1000_t linear_travel_max = 0;
	mm1000_t dist_linear[NUM_AXIS] = { 0 };

	for (axis_t x = 0; x < NUM_AXIS; x++)
	{
		if (x != axis_0 && x != axis_1)
		{
			dist_linear[x] = to[x] - current[x];
			if (dist_linear[x] > linear_travel_max)
				linear_travel_max = dist_linear[x];
		}
	}

	float radius = hypot((float)offset0, (float)offset1);

	float r_axis0 = (float) -offset0;  // Radius vector from center to current location
	float r_axis1 = (float) -offset1;
	mm1000_t rt_axis0 = to[axis_0] - center_axis0;
	mm1000_t rt_axis1 = to[axis_1] - center_axis1;

	// CCW angle between position and target from circle center. Only one atan2() trig computation required.
	float angular_travel = atan2(r_axis0*rt_axis1 - r_axis1*rt_axis0, r_axis0*rt_axis0 + r_axis1*rt_axis1);
	if (angular_travel == 0.0 || angular_travel == -0.0)
	{
		// 360Grad
		if (isclockwise)
			angular_travel = float(-2.0 * M_PI);
		else
			angular_travel = float(2.0 * M_PI);
	}
	else
	{
		if (angular_travel < 0.0)	{ angular_travel += float(2.0 * M_PI); }
		if (isclockwise)			{ angular_travel -= float(2.0 * M_PI); }
	}

#if defined(_MSC_VER)
#pragma warning (suppress:4189)
	float mm1000_of_travel = hypot(angular_travel*radius, (float) abs(linear_travel_max));
	mm1000_of_travel;
#endif
	if (hypot(angular_travel*radius, (float) abs(linear_travel_max)) < 1)
	{
		return;
	}

	// difference to Grbl => use dynamic calculation of segements => suitable for small r
	//
	// segments for full circle => (CONST_K * r * M_PI * b + CONST_D)		(r in mm, b ...2?)

	unsigned short segments = (unsigned short) abs(floor((ToDouble((const mm1000_t) (2 * SEGMENTS_K*M_PI))*radius + SEGMENTS_D) * angular_travel / (2.0*M_PI)));

#if defined(_MSC_VER)
	double segments_full = ToDouble((const mm1000_t)(2 * SEGMENTS_K*M_PI))*radius + SEGMENTS_D;
	segments_full;
	Trace("Gx command with\tr=%f\tfull_segments=%f\tangular=%f\tangularG=%f\tsegments=%i\n", radius, segments_full, angular_travel, angular_travel/M_PI*180, segments);
#endif

	if (segments > 1)
	{
		float theta_per_segment = angular_travel / segments;

		signed char arc_correction = (signed char)(ARCCORRECTION / theta_per_segment);
		if (arc_correction < 0) arc_correction = -arc_correction;

		// Vector rotation matrix values
		float cos_T = float(1.0 - 0.5*theta_per_segment*theta_per_segment);
		float sin_T = theta_per_segment;

		float sin_Ti;
		float cos_Ti;
		float r_axisi;
		unsigned short i;
		unsigned char count = 0;

		for (i = 1; i < segments; i++)
		{
			if (count < arc_correction)
			{
				// Apply vector rotation matrix 
				r_axisi = r_axis0*sin_T + r_axis1*cos_T;
				r_axis0 = r_axis0*cos_T - r_axis1*sin_T;
				r_axis1 = r_axisi;
				count++;
			}
			else
			{
				// Arc correction to radius vector. Computed only every N_ARC_CORRECTION increments.
				// Compute exact location by applying transformation matrix from initial radius vector(=-offset).
				cos_Ti = cos(i*theta_per_segment);
				sin_Ti = sin(i*theta_per_segment);
				r_axis0 = -offset0 * cos_Ti + offset1 * sin_Ti;
				r_axis1 = -offset0 * sin_Ti - offset1 * cos_Ti;
				count = 0;
			}

			// Update arc_target location

			current[axis_0] = (mm1000_t)(center_axis0 + r_axis0);
			current[axis_1] = (mm1000_t)(center_axis1 + r_axis1);

			for (axis_t x = 0; x < NUM_AXIS; x++)
			{
				if (dist_linear[x])
				{
					current[x] = to[x] - RoundMulDivI32(dist_linear[x], segments - i, segments);
				}
			}

			MoveAbs(current, feedrate);
		}
	}

	// Ensure last segment arrives at target location.
	MoveAbs(to, feedrate);
}

/////////////////////////////////////////////////////////

steprate_t CMotionControlBase::GetFeedRate(const mm1000_t to[NUM_AXIS], feedrate_t feedrate)
{
	// feedrate < 0 => no arc correction (allowable max for all axis)
	// from current position

#define AvoidOverrun 256

	axis_t   maxdistaxis = 0;

	if (feedrate >= 0)
	{
		mm1000_t maxdist = 0;
		mm1000_t sum = 0;
		mm1000_t sumOverRun = 0;
		bool useOverrun = false;

		for (register axis_t x = 0; x < NUM_AXIS; x++)
		{
			mm1000_t dist = GetPosition(x);
			dist = to[x] > dist ? (to[x] - dist) : (dist - to[x]);

			if (dist != 0)
			{
				if (dist > maxdist)
				{
					maxdistaxis = x;
					maxdist = dist;
				}

				if (dist > 0xffff) useOverrun = true;
				else
				{
					mm1000_t oldsum = sum;
					sum += dist*dist;
					useOverrun |= oldsum > sum;
				}
				sumOverRun += (dist / AvoidOverrun)*(dist / AvoidOverrun);
			}
		}
		if (maxdist > 0)
		{
			if (useOverrun)
			{
				maxdist = maxdist / AvoidOverrun;
				sum = _ulsqrt_round(sumOverRun);
			}
			else
			{
				sum = _ulsqrt_round(sum);
			}

			// avoid overrun: feedrate * maxdist
			if (ToPrecisionU2((unsigned long)feedrate) + ToPrecisionU2((unsigned long)maxdist) > 30)
			{
				// remark: maxdist < sum
				if (maxdist > 1024)
				{
					maxdist /= 256;
				}
				else
				{
					feedrate /= 256;
				}
				sum /= 256;
			}
			if (sum)
				feedrate = RoundMulDivU32(feedrate, maxdist, sum);
		}
	}

	return FeedRateToStepRate(maxdistaxis, feedrate);
}

/////////////////////////////////////////////////////////

steprate_t CMotionControlBase::FeedRateToStepRate(axis_t axis, feedrate_t feedrate)	
{
	// 60 because of min=>sec (feedrate in mm1000/min)
	if (feedrate < 0) feedrate = -feedrate;
	steprate_t steprate = (steprate_t)_ToMachine(axis, feedrate / 60); 
	return steprate ? steprate : 1; 
}
