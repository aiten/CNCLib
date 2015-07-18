#include "stdafx.h"
#include <math.h>

#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"
#include "DenavitHartenberg.h"

//////////////////////////////////////////////////////////////////////////////////////////////////////

CDenavitHartenberg::CDenavitHartenberg()
{
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

CDenavitHartenberg::~CDenavitHartenberg()
{
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

void CDenavitHartenberg::InitMatrix(CMatrix4x4<float>& m, float in[NUM_AXIS])
{
	CMatrix4x4<float> A;

	float servo1 = in[0];
	float servo2 = in[1];
	float servo3 = in[2];

	float pos1 = servo1;
	float pos2 = (M_PI)+servo2;
	float pos3 = servo3;

	float pos4 = M_PI + (M_PI - pos1 - pos2);

	float l0 = 105;
	float l1 = 140;
	float l2 = 152;
	float l3 = 30;

	m.InitDenavitHartenberg(0, M_PI / 2, l0, pos3);
	//TestConvert(AX, v);
	m *= A.InitDenavitHartenberg1Rot3Trans(l1, pos1);
	//TestConvert(AX, v);
	m *= A.InitDenavitHartenberg1Rot3Trans(l2, pos2);
	//TestConvert(AX, v);
	m *= A.InitDenavitHartenberg1Rot3Trans(l3, pos4);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

void CDenavitHartenberg::ToPosition(float in[NUM_AXIS], float out[3])
{
	float v[4] = { 0, 0, 0, 1 };
	CMatrix4x4<float> A;
	
	InitMatrix(A, in);

	TestConvert(A, v);

	float d[4];

	A.Mul(v, d);

	memcpy(out, d, sizeof(float) * 3);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

void CDenavitHartenberg::FromPosition(float posxyz[3], float angles[NUM_AXIS],float epsilon)
{
	float angle = M_PI + 0.1;

	SSearchDef search[] =
	{
		{ 0 , angle, angle/10 },
		{ 0, angle, angle / 10 },
		{ 0, angle, angle / 10 }
	};

	for (unsigned char i = 0; i < 1000; i++)
	{
		for (unsigned char j = 0; j < 3; j++)
		{
			if (SearchMin(posxyz, angles, j, search,epsilon) < epsilon)
				return;
		}
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

float CDenavitHartenberg::SearchMin(float pos[3], float inout[NUM_AXIS], unsigned char idx, SSearchDef def[NUM_AXIS], float epsilon)
{
	inout[idx] = def[idx].min;

	float dist = (def[idx].max - def[idx].min) / 10;
	float oldiff = CalcDist(pos, inout);
	float diff = oldiff;

	while (true)
	{
		float oldpos = inout[idx];
		float newpos = oldpos + dist;

		if (oldpos == newpos)		// dist < FLT_EPSILON
			break;

		if (newpos > def[idx].max)
		{
			if (oldpos == def[idx].max) break;
			newpos = def[idx].max;
		}
		else if (newpos < def[idx].min)
		{
			if (oldpos == def[idx].min) break;
			newpos = def[idx].min;
		}

		inout[idx] = newpos;

		diff = CalcDist(pos, inout);

		//printf("%f:%f:%f => %f\n", inout[0], inout[1], inout[2], diff);

		if (diff < epsilon) 
			break;

		if (diff > oldiff)
		{
			inout[idx] = oldpos;
			dist = -dist / 2;
		}
		oldiff = diff;
	}
	return diff;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

float CDenavitHartenberg::CalcDist(float pos[3], float in[NUM_AXIS])
{
	float v[4] = { 0, 0, 0, 1 };
	CMatrix4x4<float> A;

	InitMatrix(A, in);
	TestConvert(A, v,true);

	float dist = 0;
	for (unsigned char i = 0; i < 3; i++)
		dist += (pos[i] - v[i]) * (pos[i] - v[i]);

	return dist;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

#define FORMAT_MM "%.0f:%.0f:%.0f"
#define FORMAT_GRAD "%.0f:%.0f:%.0f"

void CDenavitHartenberg::TestConvert(CMatrix4x4<float>&m, float inout[4], bool out)
{
	float d[4];

	m.Mul(inout, d);
	
	if (out)
		memcpy(inout, d, sizeof(d));

	//printf(FORMAT_MM"\n", d[0], d[1], d[2]);
}
