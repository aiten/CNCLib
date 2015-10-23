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
#include "ToStringTest.h"
#include "..\MsvcStepper\MsvcStepper.h"


void CToStringTest::RunTest()
{
	char tmp[20];

	Assert("           0",	CSDist::ToString(0, tmp, 12));
	Assert("           1",	CSDist::ToString(1, tmp, 12));
	Assert("          -1",	CSDist::ToString(-1, tmp, 12));
	Assert("          10",	CSDist::ToString(10, tmp, 12));
	Assert("         -10",	CSDist::ToString(-10, tmp, 12));
	Assert("  2147483647",	CSDist::ToString(LONG_MAX, tmp, 12));
	Assert(" -2147483648",	CSDist::ToString(LONG_MIN, tmp, 12));
	Assert("2147483647",	CSDist::ToString(LONG_MAX, tmp, 10));
	Assert("-2147483648",	CSDist::ToString(LONG_MIN, tmp, 11));
	Assert("xxxxxxxxx",		CSDist::ToString(LONG_MIN, tmp, 9));
	Assert("xxxxxxxxxx",	CSDist::ToString(LONG_MIN, tmp, 10));
	Assert("    0.000",		CMm1000::ToString(0, tmp, 9, 3));
	Assert("    0.001",		CMm1000::ToString(1, tmp, 9, 3));
	Assert("    1.000",		CMm1000::ToString(1000, tmp, 9, 3));
	Assert("   12.345",		CMm1000::ToString(12345, tmp, 9, 3));
	Assert("   -0.001",		CMm1000::ToString(-1, tmp, 9, 3));
	Assert("   -1.000",		CMm1000::ToString(-1000, tmp, 9, 3));
	Assert("  -12.345",		CMm1000::ToString(-12345, tmp, 9, 3));
	Assert("   0.0000",		CMm1000::ToString(0, tmp, 9, 4));
	Assert("   0.0010",		CMm1000::ToString(1, tmp, 9, 4));
	Assert("   1.0000",		CMm1000::ToString(1000, tmp, 9, 4));
	Assert("  12.3450",		CMm1000::ToString(12345, tmp, 9, 4));
	Assert("  -0.0010",		CMm1000::ToString(-1, tmp, 9, 4));
	Assert("  -1.0000",		CMm1000::ToString(-1000, tmp, 9, 4));
	Assert(" -12.3450",		CMm1000::ToString(-12345, tmp, 9, 4));
	Assert("     0.00",		CMm1000::ToString(0, tmp, 9, 2));
	Assert("     0.00",		CMm1000::ToString(1, tmp, 9, 2));
	Assert("     1.00",		CMm1000::ToString(1000, tmp, 9, 2));
	Assert("    12.35",		CMm1000::ToString(12345, tmp, 9, 2));
	Assert("    -0.00",		CMm1000::ToString(-1, tmp, 9, 2));
	Assert("    -1.00",		CMm1000::ToString(-1000, tmp, 9, 2));
	Assert("   -12.35",		CMm1000::ToString(-12345, tmp, 9, 2));
	Assert("      0.0",		CMm1000::ToString(0, tmp, 9, 1));
	Assert("      0.0",		CMm1000::ToString(1, tmp, 9, 1));
	Assert("      1.0",		CMm1000::ToString(1000, tmp, 9, 1));
	Assert("     12.3",		CMm1000::ToString(12345, tmp, 9, 1));
	Assert("     -0.0",		CMm1000::ToString(-1, tmp, 9, 1));
	Assert("     -1.0",		CMm1000::ToString(-1000, tmp, 9, 1));
	Assert("    -12.3",		CMm1000::ToString(-12345, tmp, 9, 1));
	Assert("        0",		CMm1000::ToString(0, tmp, 9, 0));
	Assert("        0",		CMm1000::ToString(1, tmp, 9, 0));
	Assert("        1",		CMm1000::ToString(1000, tmp, 9, 0));
	Assert("       12",		CMm1000::ToString(12345, tmp, 9, 0));
	Assert("       -0",		CMm1000::ToString(-1, tmp, 9, 0));
	Assert("       -1",		CMm1000::ToString(-1000, tmp, 9, 0));
	Assert("      -12",		CMm1000::ToString(-12345, tmp, 9, 0));
	Assert("2147483.647",	CMm1000::ToString(LONG_MAX, tmp, 11, 3));
	Assert("-2147483.648",	CMm1000::ToString(LONG_MIN, tmp, 12, 3));
	Assert("        1",		CMm1000::ToString(500, tmp, 9, 0));
	Assert("        1",		CMm1000::ToString(501, tmp, 9, 0));
	Assert("        1",		CMm1000::ToString(999, tmp, 9, 0));

	Assert("      0.0",		CInch100000::ToString(0, tmp, 9, 1));
	Assert(" 0.000000",		CInch100000::ToString(0, tmp, 9, 6));
	Assert(" 1.000000",		CInch100000::ToString(100000, tmp, 9, 6));
	Assert("   0.6667",		CInch100000::ToString(66666, tmp, 9, 4));
	Assert("  0.98765",		CInch100000::ToString(98765, tmp, 9, 5));
	Assert(" -0.98765",		CInch100000::ToString(-98765, tmp, 9, 5));

}
