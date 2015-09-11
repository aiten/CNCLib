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


class CTestClass
{
public:
	CTestClass();
	~CTestClass();

	bool BreakOnAssert = true;

	virtual void RunTest()=0;

protected:

	void CheckBreakOnAssert()
	{
		if (BreakOnAssert)
			*((int*)0) = 0;	// force GP
	}

public:


	void Assert(bool expect, bool actual)
	{
		if (expect != actual)
		{
			printf("Assert failed, expected = %i, actual = %i\n", (int) expect, (int) actual);
			CheckBreakOnAssert();
		}
	}

	void Assert(int expect, int actual)
	{
		if (expect != actual)
		{
			printf("Assert failed, expected = %i, actual = %i\n", (int)expect, (int)actual);
			CheckBreakOnAssert();
		}
	}

	void Assert(const char* expect, const char* actual)
	{
		if (strcmp(expect,actual)!=0)
		{
			printf("Assert failed, expected = \'%s\', actual = \'%s\'\n", expect, actual);
			CheckBreakOnAssert();
		}
	}

	void Assert(bool isOK)
	{
		if (!isOK)
		{
			printf("Assert failed\n");
			CheckBreakOnAssert();
		}
	}

	void Assert(const char* msg)
	{
		printf(msg);
		CheckBreakOnAssert();
	}
};

