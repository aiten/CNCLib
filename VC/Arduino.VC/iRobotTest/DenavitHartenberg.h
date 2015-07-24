#pragma once

#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"

class CDenavitHartenberg
{
public:
	CDenavitHartenberg();
	~CDenavitHartenberg();


	void ToPosition(float in[NUM_AXIS], float out[3]);
	void FromPosition(float in[3], float out[NUM_AXIS], float epsilon);

protected:

	 void InitMatrix(CMatrix4x4<float>&m, float in[NUM_AXIS]);

private:

	struct SSearchDef
	{
		float min;
		float max;
		float dist;
		float changetoprev;
	};

	void TestConvert(CMatrix4x4<float>&m, float inout[4], bool out=false);

	float SearchMinOld(float pos[3], float inout[NUM_AXIS], unsigned char idx, struct SSearchDef& def , float epsilon);

	float CalcDist(float pos[3], float in[NUM_AXIS]);

	float SearchStep(float pos[3], float inout[NUM_AXIS], unsigned char idx, float diff, struct SSearchDef& def);

};

