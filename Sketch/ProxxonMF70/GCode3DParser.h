#pragma once

////////////////////////////////////////////////////////

#include "GCodeParser.h"
#include <SPI.h>
#include <SD.h>

////////////////////////////////////////////////////////
//
// GCode Parser for 3d printer extensions
//
class CGCode3DParser : public CGCodeParser
{
private:

	typedef CGCodeParser super;

public:

	CGCode3DParser(CStreamReader* reader) : super(reader)		{  }

	static File& GetExecutingFile()								{ return _state._file; }
	static void  SetExecutingFilePosition(unsigned long pos)	{ _state._printfilepos = pos; }

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
		bool				_isM28;						// SD write mode
		File				_file;
		unsigned long		_printfilepos;
		unsigned long		_printfilesize;

		void Init()
		{
			_printfilesize = 0;
			_printfilepos = 0;
			_isM28 = false;
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

