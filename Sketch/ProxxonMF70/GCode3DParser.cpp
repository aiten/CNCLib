#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "HelpParser.h"
#include "MotionControl.h"

#include "GCode3DParser.h"
#include "Control.h"

////////////////////////////////////////////////////////////

#define MAXPATHNAME	128

////////////////////////////////////////////////////////////

struct CGCode3DParser::GCodeState CGCode3DParser::_state;

////////////////////////////////////////////////////////////

bool CGCode3DParser::InitParse()
{
	if (super::InitParse())
		return true;

	if (_state._isM28)
	{
		const char* linestart = _reader->GetBuffer();
		if (!ParseLineNumber(false))	return true;

		// m28 writes all subsequent commands to the sd file
		// m29 ends the writing => we have to check first
		if (!TryToken(F("M29"), false, true))
		{
			GetExecutingFile().println(linestart);
			_reader->MoveToEnd();
			return true;
		}
		_reader->ResetBuffer(linestart);
	}

	return false;
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
		StepperSerial.println(F("Begin file list"));
		PrintSDFileListRecurse(root, 0, count, filenamebuffer, '\n');
		if (count > 0)
		{
			StepperSerial.println();
		}
		StepperSerial.println(F("End file list"));
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
			strcat(filenamebuffer, "/");
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
		StepperSerial.println(F("initialization failed!"));
		return;
	}
	StepperSerial.println(F("initialization done."));
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
		Error(F("error reading file"));
		return;
	}

	_state._printfilepos = 0;
	_state._printfilesize = GetExecutingFile().size();

	StepperSerial.print(F("File opened: "));
	StepperSerial.print(filename);
	StepperSerial.print(F(" Size: "));
	StepperSerial.println(_state._printfilesize);

	StepperSerial.println(F("File selected"));
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
		Error(F("No file selected for execution or running"));
		return;
	}

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		_state._printfilepos = GetUInt32();
		if (IsError()) return;

		GetExecutingFile().seek(_state._printfilepos);
	}
}
////////////////////////////////////////////////////////////

void CGCode3DParser::M27Command()
{
	if (GetExecutingFile())
	{
		StepperSerial.print(F("SD printing byte "));
		StepperSerial.print(_state._printfilepos);
		StepperSerial.print(F("/"));
		StepperSerial.println(_state._printfilesize);
	}
	else
	{
		StepperSerial.println(F("Not SD printing"));
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
			Error(F("error creating/writing file"));
			return;
		}

		StepperSerial.print(F("Writing to file: "));
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
		StepperSerial.println(F("Done saving file."));
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
		StepperSerial.print(F("File deleted: "));
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

	CHelpParser mycommand(_reader);
	mycommand.Parse();

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
			StepperSerial.print(F(":"));
		StepperSerial.print(CMm1000::ToString(CMotionControl::ToMm1000(i, CStepper::GetInstance()->GetPosition(i)),tmp,3));
	}
}

void CGCode3DParser::PrintVersion()
{
	StepperSerial.print(F("PROTOCOL_VERSION:1.0 FIRMWARE_URL:http//xx.com FIRMWARE_NAME:ProxxonMF70 MACHINE_TYPE:ProxxonMF70 EXTRUDER_COUNT:0"));
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::CheckSD()
{
	if (GetExecutingFile())
	{
		Error(F("File occupied"));
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
			Error(F("directory specified"));
			return false;
		}
		GetExecutingFile().close();
		if (!SD.remove(filename))
		{
			Error(F("cannot delete file"));
			return false;
		}
		return true;
	}
	else if (errorifnotexists)
	{
		Error(F("file not exists"));
		return false;
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CGCode3DParser::GetFileName(char*buffer)
{
	_reader->SkipSpaces();

#pragma message ("TODO => check 8.3!")

	char ch = _reader->GetChar();
	for (unsigned char i = 0; i < MAXPATHNAME; i++)
	{
		if (ch == '.' || isdigit(ch) || isalpha(ch) || ch == '/')
		{
			*(buffer++) = ch;
		}
		else if (ch == 0 || ch == ' ' || ch == '\t')
		{
			break;
		}
		else
		{
			Error(F("Illegal Filename"));
			return false;
		}
		ch = _reader->GetNextChar();
	}
	*(buffer++) = 0;
	return true;
}
