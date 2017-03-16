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
#include "TestClass.h"

char CTestClass::TestResultOKDir[_MAX_PATH] = {};
char CTestClass::TestResultDir[_MAX_PATH] = {};


CTestClass::CTestClass()
{
	TestResultOKFile[0] = 0;
	TestResultFile[0] = 0;
}


CTestClass::~CTestClass()
{
}

void CTestClass::Init(char*exename)
{
	::GetTempPathA(_MAX_PATH, TestResultDir);

	strcpy(TestResultOKDir, exename);
	*(strrchr(TestResultOKDir, '\\') + 0) = 0;
	*(strrchr(TestResultOKDir, '\\') + 1) = 0;
	strcat(TestResultOKDir, "TestResult\\");
}

char* CTestClass::AddFileName(char*dest, const char* start, const char*filename)
{
	strcpy(dest, start);
	strcat(dest, "Test_");
	strcat(dest, filename);

	return dest;
}
