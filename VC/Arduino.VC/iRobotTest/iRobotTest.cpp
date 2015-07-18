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
#include "DenavitHartenberg.h"

////////////////////////////////////////////////////////////////////////////////////////

bool CompareMatrix(CMatrix4x4<float>& m, float in1[][4], float out1[][4], unsigned char size)
{
	for (unsigned char i = 0; i < size; i++)
	{
		float v2[4];
		m.Mul(in1[i], v2);

		for (unsigned char n = 0; n < 4; n++)
		{
			if (!CMatrix4x4<float>::IsEqual(out1[i][n], v2[n], 0.00001))
			{
				return false;
			}
		}
	}
	return true;
}

////////////////////////////////////////////////////////////////////////////////////////

int _tmain(int /* argc */, _TCHAR* /* argv */ [])
{
	float out[4];

	float t[4][4] = { { 1.0, 2.0, 3.0, 4.0 }, { 5.0, 6.0, 7.0, 8.0 }, { 9.0, 10.0, 11.0, 12.0 },  { 13.0, 14.0, 15.0, 16.0 } };
	float t2[4][4] = { { 5.0, 4.0, 3.0, 2.0 }, { 9.0, 8.0, 7.0, 6.0 }, { 13.0, 12.0, 11.0, 10.0 }, { 17.0, 16.0, 15.0, 14.0 } };

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
		CMatrix4x4<float> T1(t);
		CMatrix4x4<float> T2(t2);
		CMatrix4x4<float> T3;

		T3 = T2*T1;
		T2 *= T1;

		if (T2 != T3)
		{
			printf("Error Mul\n");
		}
	}

	//////////////////////////////////////////
	{
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg1Rot(M_PI/4);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg1Rot(-M_PI/4);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (!T3.IsEqual(T4,0.0000001))
		{
			printf("Error InitDenavitHartenberg1Rot\n");
		}
	}

	//////////////////////////////////////////
	{
		const float angle = M_PI / 5;
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg1Rot(angle);

		float in1[][4] = { 
				{ 1, 0, 0, 1 },
				{ 0, 1, 0, 1 }, 
				{ 0, 0, 1, 1 },
				{ 1, 1, 0, 1 }
		};
		float out1[][4] = {
				{ cos(angle), sin(angle), 0, 1 },
				{ -sin(angle), cos(angle), 0, 1 },
				{ 0, 0, 1, 1 },
				{ cos(angle) - sin(angle), cos(angle) + sin(angle), 0, 1 }
		};

		if (!CompareMatrix(T1, in1, out1, sizeof(in1) / sizeof(float[4])))
		{
			printf("Error InitDenavitHartenberg1Rot#2\n");
		}
	}

	//////////////////////////////////////////

	{
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg2Trans(M_PI / 4);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg2Trans(-M_PI / 4);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (!T3.IsEqual(T4, 0.0000001))
		{
			printf("Error InitDenavitHartenberg2Trans\n");
		}
	}

	//////////////////////////////////////////
	{
		const float d = M_PI / 5;
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg2Trans(d);

		float in1[][4] = {
			{ 1, 0, 0, 1 },
			{ 0, 1, 0, 1 },
			{ 0, 0, 1, 1 },
			{ 1, 1, 0, 1 }
		};
		float out1[][4] = {
			{ 1, 0, d, 1 },
			{ 0, 1, d, 1 },
			{ 0, 0, 1+d, 1 },
			{ 1, 1, 0+d, 1 }
		};

		if (!CompareMatrix(T1, in1, out1, sizeof(in1) / sizeof(float[4])))
		{
			printf("Error InitDenavitHartenberg2Trans#2\n");
		}
	}

	//////////////////////////////////////////

	{
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg3Trans(M_PI / 4);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg3Trans(-M_PI / 4);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (!T3.IsEqual(T4, 0.0000001))
		{
			printf("Error InitDenavitHartenberg3Trans\n");
		}
	}

	//////////////////////////////////////////
	{
		const float a = M_PI / 5;
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg3Trans(a);

		float in1[][4] = {
			{ 1, 0, 0, 1 },
			{ 0, 1, 0, 1 },
			{ 0, 0, 1, 1 },
			{ 1, 1, 0, 1 }
		};
		float out1[][4] = {
			{ 1+a, 0, 0, 1 },
			{ 0+a, 1, 0, 1 },
			{ 0+a, 0, 1, 1 },
			{ 1+a, 1, 0, 1 }
		};

		if (!CompareMatrix(T1, in1, out1, sizeof(in1) / sizeof(float[4])))
		{
			printf("Error InitDenavitHartenberg3Trans#2\n");
		}
	}

	//////////////////////////////////////////

	{
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg4Rot(M_PI / 4);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg4Rot(-M_PI / 4);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (!T3.IsEqual(T4, 0.0000001))
		{
			printf("Error InitDenavitHartenberg4Rot\n");
		}
	}

	//////////////////////////////////////////
	{
		const float angle = M_PI / 5;
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg4Rot(angle);

		float in1[][4] = {
			{ 1, 0, 0, 1 },
			{ 0, 1, 0, 1 },
			{ 0, 0, 1, 1 },
			{ 1, 1, 1, 1 }
		};
		float out1[][4] = {
			{ 1, 0, 0, 1 },
			{ 0, cos(angle), sin(angle), 1 },
			{ 0, -sin(angle), cos(angle), 1 },
			{ 1, cos(angle) - sin(angle), cos(angle) + sin(angle), 1 }
		};

		if (!CompareMatrix(T1, in1, out1, sizeof(in1) / sizeof(float[4])))
		{
			printf("Error InitDenavitHartenberg4Rot#2\n");
		}
	}

	//////////////////////////////////////////

	{
		float alpha = M_PI / 5;
		float theta = M_PI / 6;
		float a = 1.123456;
		float d = 4.321;

		CMatrix4x4<float> T1; T1.InitDenavitHartenberg(a,alpha,theta,d);
		CMatrix4x4<float> T2; T2.InitDenavitHartenbergInverse( a, alpha, theta, d);

		CMatrix4x4<float> T3 = T1*T2;
		CMatrix4x4<float> T4 = T4.InitDenavitHartenbergNOP();


		if (!T3.IsEqual(T4, 0.00001))
		{
			printf("Error InitDenavitHartenberg\n");
		}
	}

	//////////////////////////////////////////

	{
		float alpha = M_PI / 5;
		float theta = M_PI / 6;
		float a = 1.123456;
		float d = 4.321;

		CMatrix4x4<float> T10; T10.InitDenavitHartenberg(a, alpha, d, theta);

		CMatrix4x4<float> T1; T1.InitDenavitHartenberg1Rot(theta);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg2Trans(d);
		CMatrix4x4<float> T3; T3.InitDenavitHartenberg3Trans(a);
		CMatrix4x4<float> T4; T4.InitDenavitHartenberg4Rot(alpha);

		CMatrix4x4<float> T5 = T1*T2*T3*T4;

		if (!T5.IsEqual(T10, 0.00001))
		{
			printf("Error InitDenavitHartenberg sequenze\n");
		}
	}

	//////////////////////////////////////////

	{
		float alpha = M_PI / 2;
		float v[4] = { 1, 1, 1, 1 };


		CMatrix4x4<float> T10; T10.InitDenavitHartenberg(0, alpha, 0, 0);
		CMatrix4x4<float> T4; T4.InitDenavitHartenberg4Rot(alpha);

		if (!T4.IsEqual(T10, 0.00001))
		{
			printf("Error InitDenavitHartenberg alpha\n");
		}
		
		T10.Mul(v, out);

		if (!CMatrix4x4<float>::IsEqual(out[0], 1, 0.001) || 
			!CMatrix4x4<float>::IsEqual(out[1], -1, 0.001) || 
			!CMatrix4x4<float>::IsEqual(out[2], 1, 0.001) || 
			!CMatrix4x4<float>::IsEqual(out[3], 1, 0.001))
		{
			printf("Error InitDenavitHartenberg rotate a\n");
		}

	}




	//////////////////////////////////////////

	{
		/*
		float alpha = M_PI / 5;
		float theta = M_PI / 6;
		float a = 1.123456;
		float d = 4.321;
		CMatrix4x4<float> T1; T1.InitDenavitHartenberg1Rot(theta);
		CMatrix4x4<float> T2; T2.InitDenavitHartenberg3Trans(a);

		CMatrix4x4<float> T3 = T1*T2;

		CMatrix4x4<float> T4; T4.InitDenavitHartenberg1Rot3Trans(theta, a);

		if (!T4.IsEqual(T3, 0.00001))
		{
			printf("Error InitDenavitHartenberg Combi\n");
		}
		*/
	}

	//////////////////////////////////////////
	{
		CDenavitHartenberg dh;

		struct STest
		{
			float posxyz[3];
			float angles[3];
			float allowdiff;
		};
		float allowdiff = 0.0001;

		STest values[] = {

			{ { 1.000000, 200.000000, 105.000000 }, { 1.008016, 1.240501, 1.565796 }, allowdiff },
			{ { 0.000000, 200.000000, 105.000000 }, { 1.008028, 1.240480, 1.570796 }, allowdiff },
			{ { -1.000000, 200.000000, 105.000000 }, { 1.008016, 1.240501, 1.575796 }, allowdiff },
			{ { 1.000000, -200.000000, 105.000000 }, { 1.008016, 1.240501, -1.565796 }, allowdiff },
			{ { 0.000000, -200.000000, 105.000000 }, { 1.008028, 1.240480, -1.570796 }, allowdiff },
			{ { -1.000000, -200.000000, 105.000000 }, { 1.008016, 1.240501, -1.575796 }, allowdiff },
			{ { 322.000000, 0.000000, 105.000000 }, { 0.000000, 3.1415926, 0.000000 }, allowdiff },
			{ { 182.000000, 0.000000, 245.000000 }, { 1.5707963, 1.5707963, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 105.000000 }, { 1.008028, 1.240480, 0.000000 }, allowdiff },
			{ { 200.000000, 100.000000, 105.000000 }, { 0.893337, 1.447827, 0.463648 }, allowdiff },
			{ { 100.000000, 100.000000, 105.000000 }, { 1.281145, 0.778904, 0.785398 }, allowdiff },
			{ { 50.000000, 0.000000, 105.000000 }, { 2.158301, 0.109737, 0.000000 }, allowdiff },
			{ { 60.000000, 0.000000, 105.000000 }, { 1.885989, 0.188764, 0.000000 }, allowdiff },
			{ { 70.000000, 0.000000, 105.000000 }, { 1.741626, 0.262326, 0.000000 }, allowdiff },
			{ { 80.000000, 0.000000, 105.000000 }, { 1.642572, 0.334292, 0.000000 }, allowdiff },
			{ { 90.000000, 0.000000, 105.000000 }, { 1.565082, 0.405774, 0.000000 }, allowdiff },
			{ { 100.000000, 0.000000, 105.000000 }, { 1.499511, 0.477271, 0.000000 }, allowdiff },
			{ { 110.000000, 0.000000, 105.000000 }, { 1.441148, 0.549075, 0.000000 }, allowdiff },
			{ { 120.000000, 0.000000, 105.000000 }, { 1.387389, 0.621401, 0.000000 }, allowdiff },
			{ { 130.000000, 0.000000, 105.000000 }, { 1.336663, 0.694426, 0.000000 }, allowdiff },
			{ { 140.000000, 0.000000, 105.000000 }, { 1.287949, 0.768320, 0.000000 }, allowdiff },
			{ { 150.000000, 0.000000, 105.000000 }, { 1.240540, 0.843252, 0.000000 }, allowdiff },
			{ { 160.000000, 0.000000, 105.000000 }, { 1.193915, 0.919401, 0.000000 }, allowdiff },
			{ { 170.000000, 0.000000, 105.000000 }, { 1.147671, 0.996961, 0.000000 }, allowdiff },
			{ { 180.000000, 0.000000, 105.000000 }, { 1.101470, 1.076153, 0.000000 }, allowdiff },
			{ { 190.000000, 0.000000, 105.000000 }, { 1.055015, 1.157228, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 105.000000 }, { 1.008028, 1.240480, 0.000000 }, allowdiff },
			{ { 210.000000, 0.000000, 105.000000 }, { 0.960228, 1.326261, 0.000000 }, allowdiff },
			{ { 220.000000, 0.000000, 105.000000 }, { 0.911315, 1.414998, 0.000000 }, allowdiff },
			{ { 230.000000, 0.000000, 105.000000 }, { 0.860954, 1.507220, 0.000000 }, allowdiff },
			{ { 240.000000, 0.000000, 105.000000 }, { 0.808743, 1.603603, 0.000000 }, allowdiff },
			{ { 250.000000, 0.000000, 105.000000 }, { 0.754183, 1.705034, 0.000000 }, allowdiff },
			{ { 260.000000, 0.000000, 105.000000 }, { 0.696615, 1.812717, 0.000000 }, allowdiff },
			{ { 270.000000, 0.000000, 105.000000 }, { 0.635121, 1.928367, 0.000000 }, allowdiff },
			{ { 280.000000, 0.000000, 105.000000 }, { 0.568332, 2.054579, 0.000000 }, allowdiff },
			{ { 290.000000, 0.000000, 105.000000 }, { 0.494008, 2.195630, 0.000000 }, allowdiff },
			{ { 300.000000, 0.000000, 105.000000 }, { 0.407949, 2.359574, 0.000000 }, allowdiff },
			{ { 310.000000, 0.000000, 105.000000 }, { 0.300103, 2.565728, 0.000000 }, allowdiff },
			{ { 320.000000, 0.000000, 105.000000 }, { 0.122047, 2.907177, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 50.000000 }, { 0.653714, 1.314741, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 150.000000 }, { 1.238929, 1.290373, 0.000000 }, allowdiff },
			{ { 200.000000, 50.000000, 150.000000 }, { 1.201563, 1.342109, 0.244979 }, allowdiff },
			{ { -1234 } } };

		for (STest* v = values; v->posxyz[0] != -1234; v++)
		{
			float out[3];
			dh.ToPosition(v->angles, out);

			bool print = false;

			for (unsigned char n = 0; n < 3; n++)
			{
				if (!CMatrix4x4<float>::IsEqual(out[n], v->posxyz[n], v->allowdiff))
				{
					if (!print)
						printf("Error InitDenavitHartenberg #20: %.2f:%.2f:%.2f ", v->posxyz[0], v->posxyz[1], v->posxyz[2]);
					printf("%f=>%f(%f) ", out[n], v->angles[n], out[n] - v->angles[n]);
					print = true;
				}
			}
			if (print)
				printf("\n");
		}
	}

	//////////////////////////////////////////
	{
		CDenavitHartenberg dh;
		struct STest
		{
			float posxyz[3];
			float angles[3];
			float allowdiff;
		};
		float allowdiff = 0.001;

		STest values[] = {

			{ { 320.000000, 0.000000, 105.000000 }, { 0.122047, 2.907177, 0.000000 }, allowdiff },
			{ { 322.000000, 0.000000, 105.000000 }, { 0.000000, 3.1415926, 0.000000 }, allowdiff },

			{ { 1.000000, 200.000000, 105.000000 }, { 1.008016, 1.240501, 1.565796 }, allowdiff },
			{ { 0.000000, 200.000000, 105.000000 }, { 1.008028, 1.240480, 1.570796 }, allowdiff },
			{ { -1.000000, 200.000000, 105.000000 }, { 1.008016, 1.240501, 1.575796 }, allowdiff },
			{ { 1.000000, -200.000000, 105.000000 }, { 1.008016, 1.240501, -1.565796 }, allowdiff },
			{ { 0.000000, -200.000000, 105.000000 }, { 1.008028, 1.240480, -1.570796 }, allowdiff },
			{ { -1.000000, -200.000000, 105.000000 }, { 1.008016, 1.240501, -1.575796 }, allowdiff },
			{ { 322.000000, 0.000000, 105.000000 }, { 0.000000, 3.1415926, 0.000000 }, allowdiff },
			{ { 182.000000, 0.000000, 245.000000 }, { 1.5707963, 1.5707963, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 105.000000 }, { 1.008028, 1.240480, 0.000000 }, allowdiff },
			{ { 200.000000, 100.000000, 105.000000 }, { 0.893337, 1.447827, 0.463648 }, allowdiff },
			{ { 100.000000, 100.000000, 105.000000 }, { 1.281145, 0.778904, 0.785398 }, allowdiff },
			{ { 50.000000, 0.000000, 105.000000 }, { 2.158301, 0.109737, 0.000000 }, allowdiff },
			{ { 60.000000, 0.000000, 105.000000 }, { 1.885989, 0.188764, 0.000000 }, allowdiff },
			{ { 70.000000, 0.000000, 105.000000 }, { 1.741626, 0.262326, 0.000000 }, allowdiff },
			{ { 80.000000, 0.000000, 105.000000 }, { 1.642572, 0.334292, 0.000000 }, allowdiff },
			{ { 90.000000, 0.000000, 105.000000 }, { 1.565082, 0.405774, 0.000000 }, allowdiff },
			{ { 100.000000, 0.000000, 105.000000 }, { 1.499511, 0.477271, 0.000000 }, allowdiff },
			{ { 110.000000, 0.000000, 105.000000 }, { 1.441148, 0.549075, 0.000000 }, allowdiff },
			{ { 120.000000, 0.000000, 105.000000 }, { 1.387389, 0.621401, 0.000000 }, allowdiff },
			{ { 130.000000, 0.000000, 105.000000 }, { 1.336663, 0.694426, 0.000000 }, allowdiff },
			{ { 140.000000, 0.000000, 105.000000 }, { 1.287949, 0.768320, 0.000000 }, allowdiff },
			{ { 150.000000, 0.000000, 105.000000 }, { 1.240540, 0.843252, 0.000000 }, allowdiff },
			{ { 160.000000, 0.000000, 105.000000 }, { 1.193915, 0.919401, 0.000000 }, allowdiff },
			{ { 170.000000, 0.000000, 105.000000 }, { 1.147671, 0.996961, 0.000000 }, allowdiff },
			{ { 180.000000, 0.000000, 105.000000 }, { 1.101470, 1.076153, 0.000000 }, allowdiff },
			{ { 190.000000, 0.000000, 105.000000 }, { 1.055015, 1.157228, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 105.000000 }, { 1.008028, 1.240480, 0.000000 }, allowdiff },
			{ { 210.000000, 0.000000, 105.000000 }, { 0.960228, 1.326261, 0.000000 }, allowdiff },
			{ { 220.000000, 0.000000, 105.000000 }, { 0.911315, 1.414998, 0.000000 }, allowdiff },
			{ { 230.000000, 0.000000, 105.000000 }, { 0.860954, 1.507220, 0.000000 }, allowdiff },
			{ { 240.000000, 0.000000, 105.000000 }, { 0.808743, 1.603603, 0.000000 }, allowdiff },
			{ { 250.000000, 0.000000, 105.000000 }, { 0.754183, 1.705034, 0.000000 }, allowdiff },
			{ { 260.000000, 0.000000, 105.000000 }, { 0.696615, 1.812717, 0.000000 }, allowdiff },
			{ { 270.000000, 0.000000, 105.000000 }, { 0.635121, 1.928367, 0.000000 }, allowdiff },
			{ { 280.000000, 0.000000, 105.000000 }, { 0.568332, 2.054579, 0.000000 }, allowdiff },
			{ { 290.000000, 0.000000, 105.000000 }, { 0.494008, 2.195630, 0.000000 }, allowdiff },
			{ { 300.000000, 0.000000, 105.000000 }, { 0.407949, 2.359574, 0.000000 }, allowdiff },
			{ { 310.000000, 0.000000, 105.000000 }, { 0.300103, 2.565728, 0.000000 }, allowdiff },
			{ { 320.000000, 0.000000, 105.000000 }, { 0.122047, 2.907177, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 50.000000 }, { 0.653714, 1.314741, 0.000000 }, allowdiff },
			{ { 200.000000, 0.000000, 150.000000 }, { 1.238929, 1.290373, 0.000000 }, allowdiff },
			{ { 200.000000, 50.000000, 150.000000 }, { 1.201563, 1.342109, 0.244979 }, allowdiff },
			{ { -1234 } } };


		for (STest* v = values; v->posxyz[0] != -1234; v++)
		{
			bool print = false;

			float out[3] = { 1, 1, 1 };
			dh.FromPosition(v->posxyz, out,0.001);

			for (unsigned char n = 0; n < 3; n++)
			{
				if (!CMatrix4x4<float>::IsEqual(out[n], v->angles[n], v->allowdiff))
				{
					if (!print)
						printf("Error InitDenavitHartenberg #21: %.2f:%.2f:%.2f\t", v->posxyz[0], v->posxyz[1], v->posxyz[2]);
					
					printf("%f=>%f(%f)\t", out[n], v->angles[n], out[n] - v->angles[n]);
					print = true;
				}
			}

			if (print)
				printf("\n");
			
			print = false;

			float posxyz[3];
			dh.ToPosition(out, posxyz);

			for (unsigned char n = 0; n < 3; n++)
			{
				if (!CMatrix4x4<float>::IsEqual(v->posxyz[n], posxyz[n], 000.1))
				{
					if (!print)
						printf("Error InitDenavitHartenberg #22: %.2f:%.2f:%.2f\t", v->posxyz[0], v->posxyz[1], v->posxyz[2]);

					printf("%f=>%f(%f)\t", posxyz[n], v->posxyz[n], posxyz[n] - v->posxyz[n]);
					print = true;

				}
			}

			if (print)
				printf("\n");
		}
	}
}
