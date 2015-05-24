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
#include "MotionControl.h"

/////////////////////////////////////////////////////////

CMotionControl::CMotionControl()
{
}

/////////////////////////////////////////////////////////

void CMotionControl::SetRotate(float rad, const mm1000_t vect[NUM_AXIS], const mm1000_t ofs[NUM_AXIS])
{
	_angle = rad;

	if (rad==0.0)
	{
		_rotateType = NoRotate;
	}
	else
	{
		_rotateType = Rotate;
		memcpy(_rotateOffset,ofs,sizeof(_rotateOffset));
		memcpy(_vect,vect,sizeof(_vect));
		_rotate3D.Set(rad,vect);
	}
}

/////////////////////////////////////////////////////////

void CMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src, dest);

	if (_rotateType != NoRotate)
	{
		if (_rotateType != RotateInvert)
		{
			_rotateType = RotateInvert;
			_rotate3D.Set(-_angle,_vect);
		}
		_rotate3D.Rotate(dest,_rotateOffset,dest);
	}
}

/////////////////////////////////////////////////////////

bool CMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	if (!super::TransformPosition(src, dest))
		return false;

	if (_rotateType != NoRotate)
	{
		if (_rotateType != Rotate)
		{
			_rotateType = Rotate;
			_rotate3D.Set(_angle,_vect);
		}
		_rotate3D.Rotate(dest,_rotateOffset,dest);
	}

	return true;
}

/////////////////////////////////////////////////////////

void CMotionControl::SRotate3D::Set(float rad, const mm1000_t vect[NUM_AXIS])
{
	float n1=vect[0];
	float n2=vect[1];
	float n3=vect[2];

	float vectorlenght = sqrt(n1*n1 + n2*n2 + n3*n3);
	n1 = n1 / vectorlenght;
	n2 = n2 / vectorlenght;
	n3 = n3 / vectorlenght;

	float cosa = cos(rad);
	float sina = sin(rad);

	_vect[0][0] = n1*n1*(1-cosa) + cosa;
	_vect[0][1] = n1*n2*(1-cosa) - n3*sina;
	_vect[0][2] = n1*n3*(1-cosa) + n2*sina;

	_vect[1][0] = n1*n2*(1-cosa) + n3*sina;
	_vect[1][1] = n2*n2*(1-cosa) + cosa;
	_vect[1][2] = n2*n3*(1-cosa) - n1*sina;

	_vect[2][0] = n1*n3*(1-cosa) - n2*sina;
	_vect[2][1] = n2*n3*(1-cosa) + n1*sina;
	_vect[2][2] = n3*n3*(1-cosa) + cosa;
}

/////////////////////////////////////////////////////////

void CMotionControl::SRotate3D::Rotate(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	// rotate with positive angle
	float fx = (float) (src[0] - ofs[0]);
	float fy = (float) (src[1] - ofs[1]);
	float fz = (float) (src[2] - ofs[2]);

	dest[0] = (mm1000_t) (fx*_vect[0][0] + fy*_vect[0][1] + fz*_vect[0][2]) + ofs[0];
	dest[1] = (mm1000_t) (fx*_vect[1][0] + fy*_vect[1][1] + fz*_vect[1][2]) + ofs[1];
	dest[2] = (mm1000_t) (fx*_vect[2][0] + fy*_vect[2][1] + fz*_vect[2][2]) + ofs[2];
}

/////////////////////////////////////////////////////////

void CMotionControl::UnitTest()
{
#ifdef _MSC_VER

	InitConversion(ToMm1000_1_1000, ToMachine_1_1000);

	mm1000_t ofs[3] = { 0,0,0 };

	mm1000_t srcX[3] = { 1000,0,0 };
	mm1000_t srcY[3] = { 0,1000,0 };
	mm1000_t srcZ[3] = { 0,0,1000 };
	mm1000_t srcXY[3] = { 1000,2000,3000 };
	mm1000_t dest[3] = { 0,0,0 };

	mm1000_t vectX[3] = { 100,0,0 };
	mm1000_t vectY[3] = { 0,100,0 };
	mm1000_t vectZ[3] = { 0,0,100 };

	mm1000_t vectXY[3] = { 100,100,0 };
	mm1000_t vectXZ[3] = { 100,0,100 };
	mm1000_t vectYZ[3] = { 0,100,100 };

	mm1000_t vectXYZ[3] = { 100,100,100 };

	float angle=M_PI/3;
	//angle=0;

	Test(srcX,ofs,dest,vectX,angle,true);
	Test(srcX,ofs,dest,vectY,angle,true);
	Test(srcX,ofs,dest,vectZ,angle,true);

	Test(srcX,ofs,dest,vectXY,angle,true);
	Test(srcX,ofs,dest,vectXZ,angle,true);
	Test(srcX,ofs,dest,vectYZ,angle,true);

	Test(srcX,ofs,dest,vectXYZ,angle,true);

	Test(srcY,ofs,dest,vectX,angle,true);
	Test(srcY,ofs,dest,vectY,angle,true);
	Test(srcY,ofs,dest,vectZ,angle,true);
			
	Test(srcY,ofs,dest,vectXY,angle,true);
	Test(srcY,ofs,dest,vectXZ,angle,true);
	Test(srcY,ofs,dest,vectYZ,angle,true);
			
	Test(srcY,ofs,dest,vectXYZ,angle,true);

	Test(srcZ,ofs,dest,vectX,angle,true);
	Test(srcZ,ofs,dest,vectY,angle,true);
	Test(srcZ,ofs,dest,vectZ,angle,true);
			
	Test(srcZ,ofs,dest,vectXY,angle,true);
	Test(srcZ,ofs,dest,vectXZ,angle,true);
	Test(srcZ,ofs,dest,vectYZ,angle,true);
			
	Test(srcZ,ofs,dest,vectXYZ,angle,true);

	Test(srcXY,ofs,dest,vectX,angle,true);
	Test(srcXY,ofs,dest,vectY,angle,true);
	Test(srcXY,ofs,dest,vectZ,angle,true);
			
	Test(srcXY,ofs,dest,vectXY,angle,true);
	Test(srcXY,ofs,dest,vectXZ,angle,true);
	Test(srcXY,ofs,dest,vectYZ,angle,true);
			
	Test(srcXY,ofs,dest,vectXYZ,angle,true);


	ClearRotate();
#endif
}

#ifdef _MSC_VER

inline bool CompareMaxDiff(mm1000_t a, mm1000_t b, mm1000_t diff = 3) { return  (abs(a - b) >= diff); }

bool CMotionControl::Test(const mm1000_t src[NUM_AXIS],const mm1000_t ofs[NUM_AXIS],mm1000_t dest[NUM_AXIS], mm1000_t vect[NUM_AXIS], float angle, bool printOK)
{
	SetRotate(angle,vect,ofs);

	udist_t	to_m[NUM_AXIS];
	mm1000_t toorig[NUM_AXIS];

	memcpy(dest,src,sizeof(toorig));

	bool isError = false;

	if (TransformPosition(src,dest))
	{
		ToMachine(dest, to_m);

		TransformFromMachinePosition(to_m,toorig);

		for (unsigned char i = 0; i < NUM_AXIS && !isError; i++)
			isError = CompareMaxDiff(src[i], toorig[i]);
	}
	else
	{
		isError = true;
	}

	if (printOK || isError)
	{
		DumpArray<mm1000_t, NUM_AXIS>(F("Src"), src, false);
		DumpArray<mm1000_t, NUM_AXIS>(F("Ofs"), ofs, false);
		DumpArray<mm1000_t, NUM_AXIS>(F("Vector"), vect, false);
		DumpType<float>(F("Angle"), angle, false);
		DumpArray<mm1000_t, NUM_AXIS>(F(" =>"), dest, false);
		DumpArray<mm1000_t, NUM_AXIS>(F("Back"), toorig, false);

		if (isError)
			printf(" ERROR");

		printf("\n");
	}

	return isError;
}

#endif
