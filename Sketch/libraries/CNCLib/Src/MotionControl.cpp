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
#include "Matrix3x3.h"
#include "Matrix4x4.h"

/////////////////////////////////////////////////////////

CMotionControl::CMotionControl()
{
	SetOffset2D(NULL);
}

/////////////////////////////////////////////////////////

void CMotionControl::SetRotate(float rad, const mm1000_t vect[NUM_AXISXYZ], const mm1000_t ofs[NUM_AXISXYZ])
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

void CMotionControl::SetRotate2D(float alpha, float beta, float gamma, const mm1000_t ofs[NUM_AXISXYZ])
{
	SetRotate2D(X_AXIS, alpha);
	SetRotate2D(Y_AXIS, beta);
	SetRotate2D(Z_AXIS, gamma);
	SetOffset2D(ofs);
}

/////////////////////////////////////////////////////////

void CMotionControl::SetRotate2D(axis_t axis, float rad)
{
	if (rad != 0.0)
	{
		if (axis==Y_AXIS) rad = -rad;
		BitSet(_rotateEnabled2D,axis);
		_rotate2D[axis].Set(rad);
	}
	else
	{
		BitClear(_rotateEnabled2D,axis);
	}
}

/////////////////////////////////////////////////////////

void CMotionControl::SetOffset2D(const mm1000_t ofs[NUM_AXISXYZ])
{
	if (ofs!=NULL)
	{
		memcpy(_rotateOffset2D,ofs,sizeof(_rotateOffset2D));
	}
	else
	{
		memset(_rotateOffset2D,0,sizeof(_rotateOffset2D));
	}
}


/////////////////////////////////////////////////////////

void CMotionControl::TransformFromMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	super::TransformFromMachinePosition(src, dest);
	
	if (_rotateEnabled2D)
	{
		float x = (float)(dest[X_AXIS] - _rotateOffset2D[X_AXIS]);
		float y = (float)(dest[Y_AXIS] - _rotateOffset2D[Y_AXIS]);
		float z = (float)(dest[Z_AXIS] - _rotateOffset2D[Z_AXIS]);

		if (IsBitSet(_rotateEnabled2D, Z_AXIS))
		{
			_rotate2D[Z_AXIS].RotateInvert(x, y);
		}
		if (IsBitSet(_rotateEnabled2D, Y_AXIS))
		{
			_rotate2D[Y_AXIS].RotateInvert(z, x);
		}
		if (IsBitSet(_rotateEnabled2D, X_AXIS))
		{
			_rotate2D[X_AXIS].RotateInvert(y, z);
		}


		dest[X_AXIS] = CMm1000::Convert(x) + _rotateOffset2D[X_AXIS];
		dest[Y_AXIS] = CMm1000::Convert(y) + _rotateOffset2D[Y_AXIS];
		dest[Z_AXIS] = CMm1000::Convert(z) + _rotateOffset2D[Z_AXIS];
	}

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
		
		_rotate3D.Rotate(dest, _rotateOffset, dest);
	}

	if (_rotateEnabled2D)
	{
		float x = (float)(dest[X_AXIS] - _rotateOffset2D[X_AXIS]);
		float y = (float)(dest[Y_AXIS] - _rotateOffset2D[Y_AXIS]);
		float z = (float)(dest[Z_AXIS] - _rotateOffset2D[Z_AXIS]);
		
		if (IsBitSet(_rotateEnabled2D, X_AXIS))
		{
			_rotate2D[X_AXIS].Rotate(y, z);
		}
		if (IsBitSet(_rotateEnabled2D, Y_AXIS))
		{
			_rotate2D[Y_AXIS].Rotate(z, x);
		}
		if (IsBitSet(_rotateEnabled2D, Z_AXIS))
		{
			_rotate2D[Z_AXIS].Rotate(x, y);
		}

		dest[X_AXIS] = CMm1000::Convert(x) + _rotateOffset2D[X_AXIS];
		dest[Y_AXIS] = CMm1000::Convert(y) + _rotateOffset2D[Y_AXIS];
		dest[Z_AXIS] = CMm1000::Convert(z) + _rotateOffset2D[Z_AXIS];
	}

	return true;
}

/////////////////////////////////////////////////////////

void CMotionControl::SRotate3D::Set(float rad, const mm1000_t vect[NUM_AXISXYZ])
{
	float n1 = (float) vect[0];
	float n2 = (float) -vect[1];		// Y_AXIS 
	float n3 = (float) vect[2];

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

void CMotionControl::SRotate3D::Rotate(float&x, float&y, float&z)
{
	float fx = x;
	float fy = y;
	float fz = z;

	x = fx*_vect[0][0] + fy*_vect[0][1] + fz*_vect[0][2];
	y = fx*_vect[1][0] + fy*_vect[1][1] + fz*_vect[1][2];
	z = fx*_vect[2][0] + fy*_vect[2][1] + fz*_vect[2][2];
}

/////////////////////////////////////////////////////////

void CMotionControl::SRotate3D::Rotate(const mm1000_t src[NUM_AXIS], const mm1000_t ofs[NUM_AXISXYZ], mm1000_t dest[NUM_AXIS])
{
	float x = (float)(src[X_AXIS] - ofs[X_AXIS]);
	float y = (float)(src[Y_AXIS] - ofs[Y_AXIS]);
	float z = (float)(src[Z_AXIS] - ofs[Z_AXIS]);

	Rotate(x,y,z);

	dest[X_AXIS] = CMm1000::Convert(x) + ofs[X_AXIS];
	dest[Y_AXIS] = CMm1000::Convert(y) + ofs[Y_AXIS];
	dest[Z_AXIS] = CMm1000::Convert(z) + ofs[Z_AXIS];
}

/////////////////////////////////////////////////////////

#ifdef _MSC_VER

void CMotionControl::UnitTest()
{
	return;

	InitConversion(ToMm1000_1_1000, ToMachine_1_1000);

	// matrix test

	float msrc[NUM_AXISXYZ][NUM_AXISXYZ] = { { 3 , 2 , 3 }, { 4, 5, 6}, { 7, 8, 9,} } ;
	float mdest[3][3];
	float mdest2[3][3];

	CMatrix3x3<float>::Invert(msrc,mdest);
	CMatrix3x3<float>::Invert(mdest,mdest2);

	float v1[3] = { 1.0, 2.0, 3.0 };
	float v2[3];

	CMatrix3x3<float>::Mul(mdest2,v1,v2);

	// see: http://www.math.kit.edu/iag2/~globke/media/koordinaten.pdf
	// see: http://de.wikipedia.org/wiki/Denavit-Hartenberg-Transformation

	float theta = (float)(15.0 * 180 / M_PI);	// phi ... einer Rotation \theta_n(Gelenkwinkel) um die z_{ n - 1 }-Achse, damit die x_{ n - 1 }-Achse parallel zu der x_n - Achse liegt
	float alpha = (float)(10.0 * 180 / M_PI);	// alpha ... einer Rotation \alpha_n (Verwindung) um die x_n-Achse, um die z_{n - 1}-Achse in die z_n-Achse zu überführen
	float di = 10;								// d ... einer Translation d_n (Gelenkabstand) entlang der z_{n - 1}- Achse bis zu dem Punkt, wo sich z_{n - 1} und x_n schneiden
	float ai = 5;								// a ... einer Translation a_n (Armelementlänge) entlang der x_n-Achse, um die Ursprünge der Koordinatensysteme in Deckung zu bringen

	float A1[4][4] = 
	{ 
		{ cos(theta),	-sin(theta)*cos(alpha),		sin(theta)*sin(alpha),		ai*cos(theta) },
		{ sin(theta),	cos(theta)*cos(alpha),		-cos(theta)*sin(alpha),		ai*sin(theta) },
		{ 0,			sin(alpha),					cos(alpha),					di },
		{ 0,			0,						0,						1	}
	};

	float A2[4][4];

	CMatrix4x4<float>::InitDenavitHartenberg(A2,alpha,theta,ai,di);


	float ps[4] = { 1, 2, 3, 1 };
	float pd[4];

	CMatrix4x4<float>::Mul(A1, ps, pd);

	// 3d Test

	mm1000_t ofs[3] = { 1000,2000,71000 };

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

	float angle=(float)(M_PI/6);
	//angle=0.001;

	Test3D(srcX,ofs,dest,vectX,angle,true);
	Test3D(srcX,ofs,dest,vectY,angle,true);
	Test3D(srcX,ofs,dest,vectZ,angle,true);

	Test3D(srcX,ofs,dest,vectXY,angle,true);
	Test3D(srcX,ofs,dest,vectXZ,angle,true);
	Test3D(srcX,ofs,dest,vectYZ,angle,true);

	Test3D(srcX,ofs,dest,vectXYZ,angle,true);

	Test3D(srcY,ofs,dest,vectX,angle,true);
	Test3D(srcY,ofs,dest,vectY,angle,true);
	Test3D(srcY,ofs,dest,vectZ,angle,true);
			
	Test3D(srcY,ofs,dest,vectXY,angle,true);
	Test3D(srcY,ofs,dest,vectXZ,angle,true);
	Test3D(srcY,ofs,dest,vectYZ,angle,true);
			
	Test3D(srcY,ofs,dest,vectXYZ,angle,true);

	Test3D(srcZ,ofs,dest,vectX,angle,true);
	Test3D(srcZ,ofs,dest,vectY,angle,true);
	Test3D(srcZ,ofs,dest,vectZ,angle,true);
			
	Test3D(srcZ,ofs,dest,vectXY,angle,true);
	Test3D(srcZ,ofs,dest,vectXZ,angle,true);
	Test3D(srcZ,ofs,dest,vectYZ,angle,true);
			
	Test3D(srcZ,ofs,dest,vectXYZ,angle,true);

	Test3D(srcXY,ofs,dest,vectX,angle,true);
	Test3D(srcXY,ofs,dest,vectY,angle,true);
	Test3D(srcXY,ofs,dest,vectZ,angle,true);
			
	Test3D(srcXY,ofs,dest,vectXY,angle,true);
	Test3D(srcXY,ofs,dest,vectXZ,angle,true);
	Test3D(srcXY,ofs,dest,vectYZ,angle,true);
			
	Test3D(srcXY,ofs,dest,vectXYZ,angle,true);

	ClearRotate();

	// 2D Test

	float angle2dX[3]={ angle,0,0 };
	float angle2dY[3]={ 0,angle, 0 };
	float angle2dZ[3]={ 0,0, angle };
	float angle2d[3]={ angle,angle, angle };

	Test2D(srcXY,ofs,dest,angle2dX,true);
	Test2D(srcXY,ofs,dest,angle2dY,true);
	Test2D(srcXY,ofs,dest,angle2dZ,true);

	Test2D(srcXY,ofs,dest,angle2d,true);

	//2d+3D

	SetRotate(angle,vectXYZ,ofs);
	Test2D(srcXY,ofs,dest,angle2d,true);

	ClearRotate2D();
	ClearRotate();
}

inline bool CompareMaxDiff(mm1000_t a, mm1000_t b, mm1000_t diff = 3) { return  (abs(a - b) >= diff); }

bool CMotionControl::Test3D(const mm1000_t src[NUM_AXIS],const mm1000_t ofs[NUM_AXIS],mm1000_t dest[NUM_AXIS], mm1000_t vect[NUM_AXIS], float angle, bool printOK)
{
	SetRotate(angle,vect,ofs);
	return Test(src,ofs,dest, printOK, [=] (void) -> void { 
		DumpArray<mm1000_t, NUM_AXIS>(F("Vector"), vect, false);
		DumpType<float>(F("Angle"), angle, false);
	}	);
}

bool CMotionControl::Test2D(const mm1000_t src[NUM_AXIS],const mm1000_t ofs[NUM_AXIS],mm1000_t dest[NUM_AXIS], float angle[NUM_AXIS], bool printOK)
{
	SetOffset2D(ofs);
	SetRotate2D(0,angle[0]);
	SetRotate2D(1,angle[1]);
	SetRotate2D(2,angle[2]);
	return Test(src,ofs,dest, printOK, [angle] () -> void { 
		DumpArray<float,NUM_AXIS>(F("Angle"), angle, false);
	}	);
}

bool CMotionControl::Test(const mm1000_t src[NUM_AXIS],const mm1000_t ofs[NUM_AXIS],mm1000_t dest[NUM_AXIS], bool printOK, std::function<void()> print)
{
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
		print();
		DumpArray<mm1000_t, NUM_AXIS>(F(" =>"), dest, false);
		DumpArray<mm1000_t, NUM_AXIS>(F("Back"), toorig, false);

		if (isError)
			printf(" ERROR");

		printf("\n");
	}

	return isError;
}

#endif
