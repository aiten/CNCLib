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

	static unsigned long GetExecutingFilePosition()				{ return _state._printFilePos; }
	static unsigned long GetExecutingFileLine()					{ return _state._printFileLine; }
	static unsigned long GetExecutingFileSize()					{ return _state._printFileSize; }
	static const char* GetExecutingFileName()					{ return _state._printfilename; }

	static void  SetExecutingFilePosition(unsigned long pos)	{ _state._printFilePos = pos; }
	static void  SetExecutingFileLine(unsigned long line)		{ _state._printFileLine = line; }

	static void Init()											{ super::Init(); _state.Init(); }

protected:

	// overrides to exend parser

	virtual bool InitParse() override;						// begin parsing of a command (override for prechecks)
	virtual bool GCommand(gcode_t gcode) override;
	virtual bool MCommand(mcode_t mcode) override;
	virtual bool Command(char ch) override;

private:

	////////////////////////////////////////////////////////
	// global State

	struct GCodeState
	{
		unsigned long		_printFilePos;
		unsigned long		_printFileLine;
		unsigned long		_printFileSize;
		File				_file;

		bool				_isM28;						// SD write mode
		char				_printfilename[MAXFILEEXTNAME + 1];

		void Init()
		{
			_printFileSize = 0;
			_printFilePos = 0;
			_printFileLine = 0;
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

	bool GetFileName(char*buffer);
	bool CheckSD();
	bool DeleteSDFile(char*buffer, bool errorifnotexists);

	static void PrintSDFileListRecurse(class File& dir, uint8_t depth, unsigned short&count, char* filenamebuffer, char seperatorchar);

protected:

	static void PrintVersion();

};

////////////////////////////////////////////////////////

