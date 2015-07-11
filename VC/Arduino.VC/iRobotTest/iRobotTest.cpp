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

#include "stdafx.h"
#include <math.h>

#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"


int _tmain(int /* argc */, _TCHAR* /* argv */ [])
{
	float t[4][4] = { { 1.0, 2.0, 3.0, 4.0 }, { 5.0, 6.0, 7.0, 8.0 }, { 9.0, 10.0, 11.0, 12.0 }, { 13.0, 14.0, 15.0, 16.0 } };

	//////////////////////////////////////////
	//Test Compare
	{
		CMatrix4x4<float> T1(t);
		CMatrix4x4<float> T2(t);

		if (T1 == T2)
		{
			if (T1 != T2)
			{
				printf("Error Compare\n");
			}
		}
		else
		{
			printf("Error Compare\n");
		}

		T1.Set(3, 3, 1.2345);

		if (T1 == T2)
		{
			printf("Error Compare\n");
		}
		else
		{
		}
	}

	//////////////////////////////////////////
	//Test Mul
	{
		CMatrix4x4<float> T1(t);
		CMatrix4x4<float> T2(t);

		CMatrix4x4<float> T3 = T1*T2;

		float r[4][4] = { { 90.0, 100.0, 110.0, 120.0 }, { 202.0, 228.0, 254.0, 280.0 }, { 314.0, 356.0, 398.0, 440.0 }, { 426.0, 484.0, 542.0, 600.0 } };

		CMatrix4x4<float> T4(r);

		if (T3 != T4)
		{
			printf("Error Mul\n");
		}
	}

	//////////////////////////////////////////
	{
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg1Rot(M_PI/5);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg1Rot(-M_PI/5);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (T3 == T4)
		{
			printf("Error InitDenavitHartenberg1Rot\n");
		}

	}
	//////////////////////////////////////////



	CMatrix4x4<float> A1;
	CMatrix4x4<float> A2;
	CMatrix4x4<float> A3;
	CMatrix4x4<float> A4;
	CMatrix4x4<float> A5;
	CMatrix4x4<float> A6;
	CMatrix4x4<float> A7;
	CMatrix4x4<float> A8;
	CMatrix4x4<float> AX;

	float servo1 = 1;
	float servo2 = 1;
	float servo3 = 1;


	AX = A1.InitDenavitHartenberg1Rot(servo1);
	AX *= A2.InitDenavitHartenberg2Trans(55);
	AX *= A3.InitDenavitHartenberg4Rot(servo2);
	AX *= A4.InitDenavitHartenberg2Trans(140);
	AX *= A5.InitDenavitHartenberg4Rot(servo3-servo2);
	AX *= A6.InitDenavitHartenberg2Trans(152);
	AX *= A7.InitDenavitHartenberg4Rot(servo3);		// auf 180
	AX *= A8.InitDenavitHartenberg2Trans(30);

	AX =
		A8.InitDenavitHartenberg2Trans(30)*
		A7.InitDenavitHartenberg4Rot(servo3)*
		A6.InitDenavitHartenberg2Trans(152)*
		A5.InitDenavitHartenberg4Rot(servo3 - servo2)*
		A4.InitDenavitHartenberg2Trans(140)*
		A3.InitDenavitHartenberg4Rot(servo2)*
		A2.InitDenavitHartenberg2Trans(55)*
		A1.InitDenavitHartenberg1Rot(servo1);
		
		

	float v[4] = { 100, 100, 100, 0 };
	float d[4];

	AX.Mul(v, d);
	

}
