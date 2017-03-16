////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

#pragma once

#include <StepperLib.h>
#include <CNCLib.h>

void PenUp()
{
}

void PenDown()
{
}

////////////////////////////////////////////////////////////

void ToPoint(int x, int y, int * dx, int * dy, float rad)
{
	float mysin=sin(rad);
	float mycos=cos(rad);

	*dx = (int) ((float)x * mycos + (float)y * mysin);
	*dy = (int) ((float)y * mycos - (float)x * mysin);
}

////////////////////////////////////////////////////////////

long Dist(unsigned  x1, unsigned y1, unsigned x2, unsigned y2)
{
	long dx = (x1 > x2) ? x1-x2 : x2-x1;
	long dy = (y1 > y2) ? y1-y2 : y2-y1;
	return dx*dx+dy*dy;
//	return sqrt(dx*dx + dy*dy);
}

void Polygon(CMsvcStepper& Stepper, mdist_t x, mdist_t y, mdist_t radius, int n, unsigned int grad, steprate_t penmovespeed )
{
  // mittelpunkt!
  if (n>1)
  {
    int i;
    int dxx,dyy;
    int penDiff=0;
	udist_t z=Stepper.GetCurrentPosition(2);

    PenUp();

    for (i=0;i<=n;i++)
    {
      float rad = float((360.0/n*i)+grad) / float(180.0) * float(M_PI); 

      ToPoint(radius,0,&dxx,&dyy,rad); 

	  long long maxd = max(abs(dxx),abs(dyy));
	  long long move = (int)sqrt(double(dxx)*double(dxx) + double(dyy)*double(dyy));
	  steprate_t vmax = (steprate_t)(((long long)penmovespeed)*maxd / move);

//	  Stepper.MoveOrigin3(dxx,dyy,penDiff,penmovespeed);
	  Stepper.MoveAbs3(x + dxx, y + dyy, z + penDiff,  vmax);

      if (i==0)
      {
        penDiff = 0; // Settings.penDown-Settings.penUp;
        PenDown();
      }
    }

    PenUp();
  }
}
