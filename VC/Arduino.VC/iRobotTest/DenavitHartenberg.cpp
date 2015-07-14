#include "stdafx.h"
#include <math.h>

#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"
#include "DenavitHartenberg.h"


CDenavitHartenberg::CDenavitHartenberg()
{
}


CDenavitHartenberg::~CDenavitHartenberg()
{
}

void CDenavitHartenberg::ToPosition(float in[NUM_AXIS], float out[3])
{
	CMatrix4x4<float> A1;
	CMatrix4x4<float> A2;
	CMatrix4x4<float> A3;
	CMatrix4x4<float> A4;
	CMatrix4x4<float> A5;
	CMatrix4x4<float> A6;
	CMatrix4x4<float> A7;
	CMatrix4x4<float> A8;
	CMatrix4x4<float> AX;
	AX.Zero();


	float servo1 = M_PI / 2 - in[0];
	float servo2 = in[1];
	float servo3 = in[2];

	servo1 = 45 * (M_PI / 180.0);		// from xy0 pane
	servo2 = -M_PI/2; // 20 * (M_PI / -180.0) + M_PI / 2 - servo1;
	servo3 = 0;

	servo1 = 1.00801647;
	servo2 = 1.24050128;
	servo3 = 1.56579638;
	// out => 1,200,105


	float pos2 = -servo2 + M_PI - servo1;
	float pos4 = M_PI + (M_PI - servo1 - servo2);


	float v[4] = { 0, 0, 0, 1 };

	AX = A1.InitDenavitHartenberg(0, M_PI / 2, 55, servo3);
	TestConvert(AX, v);
	AX *= A2.InitDenavitHartenberg(140, 0, 0, servo1);
	TestConvert(AX,v);
	AX *= A3.InitDenavitHartenberg(152,0,0, pos2);
	TestConvert(AX, v);
	AX *= A3.InitDenavitHartenberg(30, 0, 0, pos4);
	TestConvert(AX, v);

	float d[4];

	AX.Mul(v, d);

	memcpy(out, d, sizeof(float) * 3);
}

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
