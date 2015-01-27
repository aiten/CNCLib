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
#include <MotionControlBase.h>

#include "GCode3DParser.h"
#include "MessageCNCLibEx.h"
#include <Control.h>

////////////////////////////////////////////////////////////

struct CGCode3DParser::GCodeState CGCode3DParser::_state;

////////////////////////////////////////////////////////////

bool CGCode3DParser::InitParse()
{
	if (!super::InitParse())
		return false;

	if (_state._isM28)
	{
		const char* linestart = _reader->GetBuffer();
		if (!ParseLineNumber(false))	return false;

		// m28 writes all subsequent commands to the sd file
		// m29 ends the writing => we have to check first
		if (!TryToken(F("M29"), false, true))
		{
			GetExecutingFile().println(linestart);
			_reader->MoveToEnd();
			return false;
		}
		_reader->ResetBuffer(linestart);
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::GCommand(unsigned char gcode)
{
	if (super::GCommand(gcode))
		return true;

	return false;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::MCommand(unsigned char mcode)
{
	if (super::MCommand(mcode))
		return true;

	switch (mcode)
	{
		case 20: M20Command(); return true;
		case 21: M21Command(); return true;
		case 22: M22Command(); return true;
		case 23: M23Command(); return true;
		case 24: M24Command(); return true;
		case 25: M25Command(); return true;
		case 26: M26Command(); return true;
		case 27: M27Command(); return true;
		case 28: M28Command(); return true;
		case 29: M29Command(); return true;
		case 30: M30Command(); return true;
		case 111: M111Command(); return true;
		case 114: _OkMessage = PrintPosition; return true;
		case 115: _OkMessage = PrintVersion; return true;
	}

	return false;
}

////////////////////////////////////////////////////////////

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

void CGCode3DParser::M20Command()
{
	char filenamebuffer[MAXPATHNAME];
	strcpy(filenamebuffer, "/");
	File root = SD.open(filenamebuffer);
	unsigned short count = 0;

	if (root)
	{
		root.rewindDirectory();
		StepperSerial.println(MESSAGE_PARSER3D_BEGIN_FILE_LIST);
		PrintSDFileListRecurse(root, 0, count, filenamebuffer, '\n');
		if (count > 0)
		{
			StepperSerial.println();
		}
		StepperSerial.println(MESSAGE_PARSER3D_END_FILE_LIST);
	}
	root.close();
}

////////////////////////////////////////////////////////////

void CGCode3DParser::PrintSDFileListRecurse(File& dir, unsigned char depth, unsigned short&count, char* filenamebuffer, char seperatorchar)
{
#pragma warning (suppress: 4127)
	while (true)
	{
		File entry = dir.openNextFile();
		if (!entry) break;

		if (entry.isDirectory())
		{
			unsigned int lastidx = strlen(filenamebuffer);
			strcat(filenamebuffer, entry.name());
			strcat_P(filenamebuffer, MESSAGE_PARSER3D_SLASH);
			PrintSDFileListRecurse(entry, depth + 1, count, filenamebuffer, seperatorchar);
			filenamebuffer[lastidx] = 0;
		}
		else
		{
			if (entry.name()[9] != '~')
			{
				if (count != 0)
				{
					StepperSerial.print(seperatorchar);
				}
				StepperSerial.print(filenamebuffer);
				StepperSerial.print(entry.name());
				count++;
			}
		}
		entry.close();
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M21Command()
{
	if (!SD.begin(53))
	{
		StepperSerial.println(MESSAGE_PARSER3D_INITIALIZATION_FAILED);
		return;
	}
	StepperSerial.println(MESSAGE_PARSER3D_INITIALIZATION_DONE);
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M22Command()
{
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M23Command()
{
	char filename[MAXPATHNAME];
	if (!CheckSD() || !GetFileName(filename))
		return;

	GetExecutingFile() = SD.open(filename, FILE_READ);
	if (!GetExecutingFile())
	{
		Error(MESSAGE_PARSER3D_ERROR_READING_FILE);
		return;
	}

	strcpy(_state._printfilename, filename);		//8.3
	_state._printfilepos = 0;
	_state._printfilesize = GetExecutingFile().size();

	StepperSerial.print(MESSAGE_PARSER3D_FILE_OPENED);
	StepperSerial.print(filename);
	StepperSerial.print(MESSAGE_PARSER3D_SIZE);
	StepperSerial.println(_state._printfilesize);

	StepperSerial.println(MESSAGE_PARSER3D_FILE_SELECTED);
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M24Command()
{
	if (GetExecutingFile())
	{
		CControl::GetInstance()->StartPrintFromSD();
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M25Command()
{
}
////////////////////////////////////////////////////////////

void CGCode3DParser::M26Command()
{
	// set sd pos

	if (!GetExecutingFile() || CControl::GetInstance()->PrintFromSDRunnding())
	{
		Error(MESSAGE_PARSER3D_NO_FILE_SELECTED);
		return;
	}

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		_state._printfilepos = GetUInt32();
		if (IsError()) return;

		GetExecutingFile().seek(_state._printfilepos);
	}
	else if (_reader->GetCharToUpper() == 'L')
	{
		_reader->GetNextChar();
		unsigned long lineNr = GetUInt32();
		if (IsError()) return;

		if (lineNr < 1)
		{
			Error(MESSAGE_PARSER3D_LINE_SEEK_ERROR);
			return;
		}

		GetExecutingFile().seek(0);
		unsigned long filepos = 0;

		for (unsigned long line = 1; line < lineNr; line++)
		{
			while (true)
			{
				if (GetExecutingFile().available() == 0)
				{
					Error(MESSAGE_PARSER3D_LINE_SEEK_ERROR);
					return;
				}

				filepos++;
				char ch = GetExecutingFile().read();

				if (ch=='\n')
//				if (CControl::GetInstance()->IsEndOfCommandChar(ch))	=> ignore '\r' => do not count \r\n as 2 lines
				{
					break;
				}
			}
		}

		_state._printfilepos = filepos;
	}
}
////////////////////////////////////////////////////////////

void CGCode3DParser::M27Command()
{
	if (GetExecutingFile())
	{
		StepperSerial.print(MESSAGE_PARSER3D_SD_PRINTING_BYTE);
		StepperSerial.print(_state._printfilepos);
		StepperSerial.print(MESSAGE_PARSER3D_COLON);
		StepperSerial.println(_state._printfilesize);
	}
	else
	{
		StepperSerial.println(MESSAGE_PARSER3D_NOT_SD_PRINTING);
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M28Command()
{
	if (!_state._isM28)
	{
		char filename[MAXPATHNAME];
		if (!CheckSD() || !GetFileName(filename) || !DeleteSDFile(filename, false))
			return;

		_state._file = SD.open(filename, FILE_WRITE);
		if (!GetExecutingFile())
		{
			Error(MESSAGE_PARSER3D_ERROR_CREATING_FILE);
			return;
		}

		StepperSerial.print(MESSAGE_PARSER3D_WRITING_TO_FILE);
		StepperSerial.println(filename);

		_state._isM28 = true;
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M29Command()
{
	if (_state._isM28)
	{
		if (GetExecutingFile())
			GetExecutingFile().close();

		_state._isM28 = false;
		StepperSerial.println(MESSAGE_PARSER3D_DONE_SAVE_FILE);
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M30Command()
{
	char filename[MAXPATHNAME];
	if (!CheckSD() || !GetFileName(filename))
		return;

	if (DeleteSDFile(filename, true))
	{
		StepperSerial.print(MESSAGE_PARSER3D_FILE_DELETED);
		StepperSerial.println(filename);
	}
}

////////////////////////////////////////////////////////////

void CGCode3DParser::M111Command()
{
	// set debug level

	unsigned char debuglevel = 0;

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		debuglevel = GetUInt8();
	}

	if (ExpectEndOfCommand())		{ return; }
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

////////////////////////////////////////////////////////////

void CGCode3DParser::PrintPosition()
{
	char tmp[16];
	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		if (i != 0)
			StepperSerial.print(MESSAGE_PARSER3D_COLON);
		StepperSerial.print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i), tmp, 3));
	}
}

void CGCode3DParser::PrintVersion()
{
	StepperSerial.print(MESSAGE_PARSER3D_VERSION);
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::CheckSD()
{
	if (GetExecutingFile())
	{
		Error(MESSAGE_PARSER3D_FILE_OCCUPIED);
		return false;
	}
	return true;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::DeleteSDFile(char*filename, bool errorifnotexists)
{
	GetExecutingFile() = SD.open(filename);
	if (GetExecutingFile())
	{
		if (GetExecutingFile().isDirectory())
		{
			GetExecutingFile().close();
			Error(MESSAGE_PARSER3D_DIRECOTRY_SPECIFIED);
			return false;
		}
		GetExecutingFile().close();
		if (!SD.remove(filename))
		{
			Error(MESSAGE_PARSER3D_CANNOT_DELETE_FILE);
			return false;
		}
		return true;
	}
	else if (errorifnotexists)
	{
		Error(MESSAGE_PARSER3D_FILE_NOT_EXIST);
		return false;
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::GetFileName(char*buffer)
{
	_reader->SkipSpaces();

	unsigned char dotidx = 0;
	unsigned char length = 0;

	char ch = _reader->GetChar();
	for (unsigned char i = 0; i < MAXPATHNAME; i++)
	{
		if (ch == '.')
		{
			length = 0;
			dotidx++;
			if (dotidx > 1)
			{
				// only one dot is allowed
				Error(MESSAGE_PARSER3D_ILLEGAL_FILENAME);
				return false;
			}
			*(buffer++) = ch;
		}
		else if (CStreamReader::IsDigit(ch) || CStreamReader::IsAlpha(ch) || ch == '/' || ch == '_' || ch == '~')
		{
			*(buffer++) = ch;
			length++;

			if ((dotidx == 0 && length > MAXFILENAME) ||
				(dotidx == 1 && length > MAXEXTNAME))
			{
				Error(MESSAGE_PARSER3D_ILLEGAL_FILENAME);
				return false;
			}
		}
		else if (ch == 0 || ch == ' ' || ch == '\t')
		{
			break;
		}
		else
		{
			Error(MESSAGE_PARSER3D_ILLEGAL_FILENAME);
			return false;
		}
		ch = _reader->GetNextChar();
	}
	*(buffer++) = 0;
	return true;
}
