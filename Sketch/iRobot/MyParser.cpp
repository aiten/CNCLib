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

#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <HelpParser.h>

#include "MyMotionControl.h"
#include "MyParser.h"

////////////////////////////////////////////////////////////

bool CMyParser::MCommand(unsigned char mcode)
{
	if (super::MCommand(mcode))
		return true;

	switch (mcode)
	{
		case 116: _OkMessage = PrintInfo; return true;
		case 117: M117Command(); return true;
		case 118: M118Command(); return true;
	}

	return false;
}

////////////////////////////////////////////////////////////

void CMyParser::PrintInfo()
{
//	PrintPosition();
	((CMyMotionControl*) CMotionControlBase::GetInstance())->PrintInfo();
}

////////////////////////////////////////////////////////////

void CMyParser::M117Command()
{
	SAxisMove move(false);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, AbsolutWithZeroShiftPosition);
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes)
	{
		CMyMotionControl* pMC = (CMyMotionControl*) CMotionControlBase::GetInstance();
		pMC->MoveAngle(move.newpos);
	}
}

////////////////////////////////////////////////////////////

void CMyParser::M118Command()
{
	SAxisMove move(false);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, AbsolutWithZeroShiftPosition);
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes)
	{
		CMyMotionControl* pMC = (CMyMotionControl*) CMotionControlBase::GetInstance();
		pMC->MoveAngleLog(move.newpos);
	}
}


