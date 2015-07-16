#pragma once

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"

class CDenavitHartenberg
{
public:
	CDenavitHartenberg();
	~CDenavitHartenberg();


	void ToPosition(float in[NUM_AXIS], float out[3]);
	void FromPosition(float in[3], float out[NUM_AXIS]);

protected:

	 void InitMatrix(CMatrix4x4<float>&m, float in[NUM_AXIS]);

private:


	void TestConvert(CMatrix4x4<float>&m, float inout[4], bool out=false);

	float SearchMin(float pos[3], float inout[NUM_AXIS], unsigned char idx, float min, float max);

	float CalcDist(float pos[3], float in[NUM_AXIS]);

};

