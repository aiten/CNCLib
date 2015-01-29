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

////////////////////////////////////////////////////////

#include <GCodeParser.h>
#include <SPI.h>
#include <SD.h>

////////////////////////////////////////////////////////////

#define MAXPATHNAME	128
#define MAXFILENAME	8
#define MAXEXTNAME	3
#define MAXFILEEXTNAME	(MAXFILENAME+1+MAXEXTNAME)

////////////////////////////////////////////////////////
//
// GCode Parser for 3d printer extensions
//
class CGCode3DParser : public CGCodeParser
{
private:

	typedef CGCodeParser super;

public:

	CGCode3DParser(CStreamReader* reader,Stream* output) : super(reader,output)		{  }

	static File& GetExecutingFile()								{ return _state._file; }

	static unsigned long GetExecutingFilePosition()				{ return _state._printfilepos; }
	static unsigned long GetExecutingFileLine()					{ return _state._printfileline; }
	static unsigned long GetExecutingFileSize()					{ return _state._printfilesize; }
	static const char* GetExecutingFileName()					{ return _state._printfilename; }

	static void  SetExecutingFilePosition(unsigned long pos, unsigned long line)	
																{ _state._printfilepos = pos; _state._printfileline = line; }

	static void Init()											{ super::Init(); _state.Init(); }

protected:

	// overrides to exend parser

	virtual bool InitParse();						// begin parsing of a command (override for prechecks)
	virtual bool GCommand(unsigned char gcode);
	virtual bool MCommand(unsigned char mcode);
	virtual bool Command(unsigned char ch);

private:

	////////////////////////////////////////////////////////
	// global State

	struct GCodeState
	{
		unsigned long		_printfilepos;
		unsigned long		_printfileline;
		unsigned long		_printfilesize;
		File				_file;

		bool				_isM28;						// SD write mode
		char				_printfilename[MAXFILEEXTNAME + 1];

		void Init()
		{
			_printfilesize = 0;
			_printfilepos = 0;
			_printfileline = 0;
			_isM28 = false;
			_printfilename[0] = 0;
		}
	};

	static struct GCodeState _state;

	////////////////////////////////////////////////////////

	void M20Command();		// List content of SD card
	void M21Command();		// Init SD card
	void M22Command();		// Release SD card
	void M23Command();		// Select file on SD card
	void M24Command();		// Start file on SD card
	void M25Command();
	void M26Command();		// Set file position on SD card
	void M27Command();		// Status of SD print - position/size 
	void M28Command();		// Start write to SD file
	void M29Command();		// Stop write to SD file
	void M30Command();		// Delete file on SD

	void M111Command();		// Set debug level

	void CommandEscape();

	bool GetFileName(char*buffer);
	bool CheckSD();
	bool DeleteSDFile(char*buffer, bool errorifnotexists);

	static void PrintSDFileListRecurse(class File& dir, unsigned char depth, unsigned short&count, char* filenamebuffer, char seperatorchar);

	/////////////////
	// OK Message

	static void PrintPosition();
	static void PrintVersion();
};

////////////////////////////////////////////////////////

