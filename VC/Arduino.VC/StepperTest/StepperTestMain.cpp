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
#include "..\MsvcStepper\MsvcStepper.h"
#include "RingBufferTest.h"
#include "ToStringTest.h"
#include "Matrix4x4Test.h"
#include "StepperTest.h"
#include "LinearLookupTest.h"

#pragma warning(disable: 4127)

int _tmain(int argc, _TCHAR* argv[])
{
	argc; argv;

	{
		CRingBufferTest test;
		test.RunTest();
	}

	{
		CToStringTest test;
		test.RunTest();
	}

	{
		CMatrix4x4Test test;
		test.RunTest();
	}

	{
		CLinearLookupTest test;
		test.RunTest();
	}

	{
		CStepperTest test;
		test.RunTest();
	}

	return 0;
}
