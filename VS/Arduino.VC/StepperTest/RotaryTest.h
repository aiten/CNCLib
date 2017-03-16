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

class CRotaryTest : public CTestClass
{
public:

	virtual void RunTest() override;

	void TestCW();
	void TestCCW();

	void TestOverrunCW();
	void TestOverrunCCW();

	void TestAccuracyCW();

	static uint8_t GetPinA(int pos);
	static uint8_t GetPinB(int pos);
};

