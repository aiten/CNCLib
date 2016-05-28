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

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include <MotionControl.h>

#include "GCode3DParser.h"
#include "MessageCNCLibEx.h"
#include <Control3D.h>

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

bool CGCode3DParser::GCommand(gcode_t gcode)
{
	if (super::GCommand(gcode))
		return true;

	return false;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::MCommand(mcode_t mcode)
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
		case 115: _OkMessage = PrintVersion; return true;
		case 300: M300Command(); return true;
	}

	return false;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::Command(unsigned char ch)
{
	if (super::Command(ch))
		return true;

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
#ifdef _MSC_VER
#pragma warning (suppress: 4127)
#endif
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
	((CControl3D*)CControl::GetInstance())->ReInitSD();
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
	_state._printFilePos = 0;
	_state._printFileLine = 1;
	_state._printFileSize = GetExecutingFile().size();

	StepperSerial.print(MESSAGE_PARSER3D_FILE_OPENED);
	StepperSerial.print(filename);
	StepperSerial.print(MESSAGE_PARSER3D_SIZE);
	StepperSerial.println(_state._printFileSize);

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
		_state._printFilePos = GetUInt32();
		_state._printFileLine = 1;					// TO DO => count line 
		if (IsError()) return;

		GetExecutingFile().seek(_state._printFilePos);
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

		for (unsigned long line = 1; line < lineNr; line++)
		{
			// read line until \n
			char ch;
			do
			{
				if (GetExecutingFile().available() == 0)
				{
					Error(MESSAGE_PARSER3D_LINE_SEEK_ERROR);
					return;
				}

				ch = GetExecutingFile().read();
			} 
			while (ch != '\n');
		}
		
		_state._printFileLine = lineNr;
		_state._printFilePos = GetExecutingFile().position();
	}
}
////////////////////////////////////////////////////////////

void CGCode3DParser::M27Command()
{
	if (GetExecutingFile())
	{
		StepperSerial.print(MESSAGE_PARSER3D_SD_PRINTING_BYTE);
		StepperSerial.print(_state._printFilePos);
		StepperSerial.print(MESSAGE_PARSER_COLON);
		StepperSerial.print(_state._printFileSize);
		StepperSerial.print(MESSAGE_PARSER3D_SD_PRINTING_LINE);
		StepperSerial.println(_state._printFileLine);
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

void CGCode3DParser::M300Command()
{
	SPlayTone tone[2];
	const SPlayTone* mytone=tone;
	bool fromprogmem = false;

	tone[0].Tone = ToneA4;
	tone[0].Duration = MilliSecToDuration(500); 

	tone[1].Tone = ToneEnd;

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		unsigned int freq = GetUInt16();
		tone[0].Tone = (ETone) FreqToTone(freq);
		if (IsError()) return;

		switch (freq)
		{
			case 1: mytone = SPlayTone::PlayOK; fromprogmem = true; break;
			case 2: mytone = SPlayTone::PlayError; fromprogmem = true; break;
			case 3: mytone = SPlayTone::PlayInfo; fromprogmem = true; break;
		}
	}
	if (!fromprogmem && _reader->SkipSpacesToUpper() == 'P')
	{
		_reader->GetNextChar();
		tone[0].Duration = MilliSecToDuration(GetUInt16());
		if (IsError()) return;
	}

	if (!ExpectEndOfCommand())		{ return; }

	if (CLcd::GetInstance())
	{
		CLcd::GetInstance()->Beep(mytone,fromprogmem);
	}
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
		else if (CStreamReader::IsSpaceOrEnd(ch))
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

////////////////////////////////////////////////////////////

void CGCode3DParser::PrintVersion()
{
	StepperSerial.print(MESSAGE_PARSER3D_VERSION);
}
