#pragma once

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"

class CDenavitHartenberg
{
public:
	CDenavitHartenberg();
	~CDenavitHartenberg();


	void ToPosition(float in[NUM_AXIS], float out[3]);



private:

	void TestConvert(CMatrix4x4<float>&m, float inout[4], bool out=false);

};

