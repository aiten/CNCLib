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

#define FORMAT_MM "%.0f:%.0f:%.0f"
#define FORMAT_GRAD "%.0f:%.0f:%.0f"

void CDenavitHartenberg::TestConvert(CMatrix4x4<float>&m, float inout[4], bool out)
{
	float d[4];

	m.Mul(inout, d);
	
	if (out)
		memcpy(inout, d, sizeof(d));

	printf(FORMAT_MM"\n", d[0], d[1], d[2]);
}
