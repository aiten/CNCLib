#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Configuration.h"
#include "Parser.h"
#include "Stepper.h"

////////////////////////////////////////////////////////////

void CParser::ParseCommand()
{
	if (InitParse())
	{
		Parse();
	}
	CleanupParse();
}

////////////////////////////////////////////////////////////

bool CParser::CheckError()
{
	if (IsError())
	{
		return true;
	}
	if (_reader->IsError())
	{
		_error = _reader->GetError();
		return true;
	}
	if (CStepper::GetInstance()->IsError())
	{
		_error = CStepper::GetInstance()->GetError();
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CParser::ExpectEndOfCommand()
{
	if (IsError()) return true;

	if (_reader->IsEOC(SkipSpacesOrComment()))
		return false;

	Error(MESSAGE_PARSER_EndOfCommandExpected);
	return true;
}

////////////////////////////////////////////////////////////

unsigned char CParser::GetUInt8()			{ return GetUInt<unsigned char>(); }
unsigned short CParser::GetUInt16()			{ return GetUInt<unsigned short>(); }
unsigned long CParser::GetUInt32()			{ return GetUInt<unsigned long>(); }
char CParser::GetInt8()						{ return GetInt<char>(); }
short CParser::GetInt16()					{ return GetInt<short>(); }
long CParser::GetInt32()					{ return GetInt<long>(); }
sdist_t CParser::GetSDist()					{ return GetInt<sdist_t>(); }

////////////////////////////////////////////////////////////

long CParser::GetInt32Scale(long minvalue, long maxvalue, unsigned char scale, unsigned char maxscale)
{
	// ignore digits between scale and maxscale (first digit after scale is used for round)
	// 1.2345 with scale=3 and maxscale=5 is ok => return 1235 (calculated with scale - round)

	bool negativ;
	long value = 0;
	unsigned char thisscale = 0;
	unsigned char ch = _reader->GetChar();

	if ((negativ = CStreamReader::IsMinus(ch))!=0)
	{
		ch = _reader->GetNextChar();
	}

	if (!CStreamReader::IsDigit(ch) && !CStreamReader::IsDot(ch))
	{
		ErrorNotANumber();
		return 0;
	}

	while (CStreamReader::IsDigit(ch))
	{
		value *= 10l;
		value += ch - '0';
		ch = _reader->GetNextChar();
	}

	if (CStreamReader::IsDot(ch))
	{
		ch = _reader->GetNextChar();

		if (!CStreamReader::IsDigit(ch))
		{
			Error(MESSAGE_PARSER_NotANumber_MissingScale);
			return 0;
		}

		while (CStreamReader::IsDigit(ch))
		{
			if (thisscale < scale)
			{
				value *= 10l;
				value += ch - '0';
			}
			else if (thisscale == scale)
			{
				// check round
				if (ch >= '5')
				{
					value++;
				}
			}

			ch = _reader->GetNextChar();
			thisscale++;
		}
	}

	if (thisscale > maxscale)
	{
		Error(MESSAGE_PARSER_NotANumber_MaxScaleExceeded);
		return 0;
	}
	else if (thisscale < scale)
	{
		for (; thisscale != scale; thisscale++)
			value *= 10l;
	}

	if (negativ)
		value = -(value);

	if (value < minvalue)
		Error(MESSAGE_PARSER_ValueLessThanMin);
	else if (value > maxvalue)
		Error(MESSAGE_PARSER_ValueGreaterThanMax);

	return value;
}

////////////////////////////////////////////////////////////

expr_t CParser::GetDouble()
{
	unsigned char ch = _reader->GetChar();
	char*start = (char*)_reader->GetBuffer();

	while (CStreamReader::IsDigitDot(ch))
	{
		ch = _reader->GetNextChar();
	}

	// check for scientific notation like "2.3e-4" or "1.23e50"
	if (toupper(ch) == 'E')
	{
		ch = _reader->GetNextChar();

		if (ch == '+' || ch == '-')
		{
			ch = _reader->GetNextChar();
		}

		while (CStreamReader::IsDigit(ch))
		{
			ch = _reader->GetNextChar();
		}
	}

	// temporary terminat buffer with '\00'
	CStreamReader::CSetTemporary terminate(_reader->GetBuffer());
	return (expr_t)strtod(start, NULL);
}

////////////////////////////////////////////////////////////

bool CParser::IsToken(const __FlashStringHelper* b, bool expectdel, bool ignorecase)
{
	const char* tmp;
	if (!TryToken(_reader->GetBuffer(), b, expectdel, ignorecase, tmp))
		return false;

	_reader->ResetBuffer(tmp);

	_reader->SkipSpaces();
	return true;
}

////////////////////////////////////////////////////////////

bool CParser::TryToken(const char* buffer, const __FlashStringHelper* b, bool expectdel, bool ignorecase)
{
	const char* dummy;
	return TryToken(buffer, b, expectdel, ignorecase, dummy);
}

////////////////////////////////////////////////////////////

bool CParser::TryToken(const char* buffer, const __FlashStringHelper* b, bool ignorecase)
{
	const char PROGMEM *p = (const char PROGMEM *)b;

	//const char* tmp = _reader->GetBuffer();
	char c = pgm_read_byte(p);

	while (c && ConvertChar(*buffer, ignorecase) == ConvertChar(c, ignorecase))
	{
		p++; buffer++;
		c = pgm_read_byte(p);
	}

	return c == 0 && *buffer == 0;
}

////////////////////////////////////////////////////////////

bool CParser::TryToken(const char* buffer, const __FlashStringHelper* b, bool expectdel, bool ignorecase, const char*&nextchar)
{
	const char PROGMEM *p = (const char PROGMEM *)b;

	//const char* tmp = _reader->GetBuffer();
	char c = pgm_read_byte(p);

	while (c && ConvertChar(*buffer, ignorecase) == ConvertChar(c, ignorecase))
	{
		p++; buffer++;
		c = pgm_read_byte(p);
	}

	switch (c)
	{
		case '\001':	if (!isdigit(*buffer)) return false;
			c = 0;
			break;
		case '\002':	if (*buffer != 0 && !CStreamReader::IsSpace(*buffer) && isdigit(*buffer)) return false;
			c = 0;
			break;
		case '\003':	if (*buffer != 0) return false;
			c = 0;
			break;
	}

	if (c) return false;

	// check if lineend or blank

	if (expectdel)
	{
		if (!CStreamReader::IsSpaceOrEnd(*buffer) && !CStreamReader::IsEOC(*buffer))
			return false;
	}

	nextchar = buffer;

	return true;
}
