#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "StreamReader.h"

////////////////////////////////////////////////////////////

CStreamReader::CStreamReader()
{
}

////////////////////////////////////////////////////////////

char CStreamReader::SkipSpaces()
{
	while (IsSpace(*_buffer))
		_buffer++;

	return GetChar();
}

