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
	}

	return false;
}

////////////////////////////////////////////////////////////
/*
bool CGCode3DParser::Command(unsigned char ch)
{
	if (super::Command(ch))
		return true;

	switch (ch)
	{
		case '!':
		case '-':
		case '?':
		case '$': CommandEscape(); return true;
	}

	return false;
}

////////////////////////////////////////////////////////////

void CGCode3DParser::CommandEscape()
{
	if (_reader->GetChar() == '$')
		_reader->GetNextChar();

	CHelpParser mycommand(GetReader(),GetOutput());
	mycommand.ParseCommand();

	if (mycommand.IsError()) Error(mycommand.GetError());
	_OkMessage = mycommand.GetOkMessage();
}
*/
////////////////////////////////////////////////////////////

void CMyParser::PrintInfo()
{
//	PrintPosition();
	((CMyMotionControl*) CMotionControlBase::GetInstance())->PrintInfo();
}

