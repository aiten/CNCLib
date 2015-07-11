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

void CDenavitHartenberg::ToPosition(float to[3])
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

	float servo1 = 1;
	float servo2 = 1;
	float servo3 = 1;

	AX = A1.InitDenavitHartenberg1Rot(servo1);
	AX *= A2.InitDenavitHartenberg2Trans(55);
	AX *= A3.InitDenavitHartenberg4Rot(servo2);
	AX *= A4.InitDenavitHartenberg2Trans(140);
	AX *= A5.InitDenavitHartenberg4Rot(servo3 - servo2);
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



	float v[4] = { 0, 0, 0, 1 };
	float d[4];

	AX.Mul(v, d);

	memcpy(to, d, sizeof(float) * 3);
}
