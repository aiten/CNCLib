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

#pragma once

#include "TestClass.h"
#include "..\..\..\sketch\libraries\CNCLib\src\Matrix4x4.h"


class CMatrix4x4Test : public CTestClass
{
public:

	virtual void RunTest() override;

	static bool CompareMatrix(CMatrix4x4<float>& m, float in1[][4], float out1[][4], unsigned char size);

};

