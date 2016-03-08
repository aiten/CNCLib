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
// 
// class for scanning an commandstream
//

class CStreamReader
{
public:

	CStreamReader();
	void Init(char* buffer)					{ _buffer = buffer; _error = NULL; }

	bool IsError()							{ return _error != NULL; };
	const __FlashStringHelper * GetError()	{ return _error; }

	char SkipSpaces();
	char SkipSpacesToUpper()				{ return Toupper(SkipSpaces()); }
	char GetNextCharSkipScaces()			{ _buffer++; return SkipSpaces(); }		// move to next and skip spaces

	char GetCharToUpper()					{ return (char)Toupper(*_buffer); }
	char GetChar()							{ return *_buffer; }
	char GetNextChar()						{ return *(++_buffer); }					// skip current and move to next
	char GetNextCharToUpper()				{ return Toupper(GetNextChar()); }			// skip current and move to next

	bool IsNextChar(const char ch)			{ if (ch!=GetChar()) return false; GetNextChar(); return true; }

	void MoveToEnd()						{ while (*_buffer) _buffer++; }				// move to "no more char in stream"

	const char*	GetBuffer()					{ return _buffer; }
	void ResetBuffer(const char* buffer)	{ _buffer = buffer; }

	static char Toupper(char ch)			{ return IsLowerAZ(ch) ? ch + 'A' - 'a' : ch; }
	static bool IsEOC(char ch)				{ return ch == 0 || ch == ';'; }			// is EndOfCommand

	static bool IsSpace(char ch)			{ return ch == ' ' || ch == '\t' || ch == '\r'; }
	static bool IsSpaceOrEnd(char ch)		{ return ch == 0 || IsSpace(ch); }

	static bool IsMinus(char ch)			{ return ch == '-'; }
	static bool IsDot(char ch)				{ return ch == '.'; }
	static bool IsDigit(char ch)			{ return ch>='0'&&ch<='9'; }
	static bool IsDigitDot(char ch)			{ return IsDigit(ch) || IsDot(ch); }

	static bool IsAlpha(char ch)			{ ch = Toupper(ch); return ch == '_' || IsUpperAZ(ch); }
	static bool IsLowerAZ(char ch)			{ return ch >= 'a' && ch <= 'z'; }
	static bool IsUpperAZ(char ch)			{ return ch >= 'A' && ch <= 'Z'; }

	void Error(const __FlashStringHelper * error)		{ _error = error; MoveToEnd(); }

	const char* _buffer;
	const __FlashStringHelper * _error;

	class CSetTemporary
	{
	private:
		char* _buffer;
		char  _oldch;

	public:
		CSetTemporary(const char*buffer)			{ _buffer = (char*)buffer;  _oldch = *_buffer;  *_buffer = 0; }
		CSetTemporary(const char*buffer, char ch)	{ _buffer = (char*)buffer;  _oldch = *_buffer;  *_buffer = ch; }
		~CSetTemporary()							{ *_buffer = _oldch; }
	};
};

////////////////////////////////////////////////////////


