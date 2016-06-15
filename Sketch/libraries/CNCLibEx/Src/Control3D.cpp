////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

////////////////////////////////////////////////////////////

#include <stdio.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <SPI.h>
#include <SD.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include "GCode3DParser.h"
#include "Control3D.h"

////////////////////////////////////////////////////////////

void CControl3D::Init()
{
	super::Init();
	CGCode3DParser::Init();
	ClearPrintFromSD ();
}

////////////////////////////////////////////////////////////

void CControl3D::Initialized()
{
	super::Initialized();
	char tmp[8 + 3 + 1];
	strcpy_P(tmp, PSTR("startup.nc"));

	CGCode3DParser::GetExecutingFile() = SD.open(tmp, FILE_READ);

	if (CGCode3DParser::GetExecutingFile())
	{
		StepperSerial.println(MESSAGE_CONTROL3D_ExecutingStartupNc);
		StartPrintFromSD();
	}
	else
	{
		StepperSerial.println(MESSAGE_CONTROL3D_NoStartupNcFoundOnSD);
	}
}

////////////////////////////////////////////////////////////

void CControl3D::InitSD(pin_t sdEnablePin)
{
	StepperSerial.print(MESSAGE_CONTROL3D_InitializingSDCard);

	ClearPrintFromSD ();

	CHAL::pinModeOutput(sdEnablePin);
	CHAL::digitalWrite(sdEnablePin, HIGH);

	_sdEnablePin = sdEnablePin;
	ReInitSD();
}

////////////////////////////////////////////////////////////

void CControl3D::ReInitSD()
{
	if (!SD.begin(_sdEnablePin))
	{
		StepperSerial.println(MESSAGE_CONTROL3D_initializationFailed);
	}
	else
	{
		StepperSerial.println(MESSAGE_CONTROL3D_initializationDone);
	}
}

////////////////////////////////////////////////////////////

bool CControl3D::Parse(CStreamReader* reader, Stream* output)
{
	CGCode3DParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

void CControl3D::ReadAndExecuteCommand()
{
	super::ReadAndExecuteCommand();

	File file = CGCode3DParser::GetExecutingFile();
	if (PrintFromSDRunnding() && file)
	{
		if (IsKilled())
		{
			ClearPrintFromSD();
			file.close();
		}
		else
		{
			FileReadAndExecuteCommand(&file,NULL);			// one line!!! Output goes to NULL

			if (file.available() == 0)
			{
				ClearPrintFromSD();
				file.close();
				StepperSerial.println(MESSAGE_CONTROL3D_ExecutingStartupNcDone);
			}
			else
			{
				CGCode3DParser::SetExecutingFilePosition(file.position());
				CGCode3DParser::SetExecutingFileLine(CGCode3DParser::GetExecutingFileLine() + 1);
			}
		}
	}
}

////////////////////////////////////////////////////////////
