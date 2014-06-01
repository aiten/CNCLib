#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <HAStepper.h>

#include "Settings.h"

#include "Global.h"
#include "CommandBase.h"
#include "Tools.h"

////////////////////////////////////////////////////////////

CCommandBase::CCommandBase()
{
}

////////////////////////////////////////////////////////////

void CCommandBase::SkipSpaces()
{
  while (*_buffer == ' ' || *_buffer == '\t' )
    _buffer++;
}

////////////////////////////////////////////////////////////

bool CCommandBase::SkipSeperator()
{
  bool ret = false;
  while (*_buffer == ' ' || *_buffer == '\t' || *_buffer == ',' )
  {
    _buffer++;
    ret = true;
  }

  return ret;
}

////////////////////////////////////////////////////////////

bool CCommandBase::Error()
{
  StepperSerial.println(F("Error:"));
  return false;
}

bool CCommandBase::ErrorExpectedEnd()
{
  StepperSerial.println(F("Unexpected End"));
  return false;
}

bool CCommandBase::ErrorMissingOnInvalidParameter()
{
  StepperSerial.println(F("ErrorMissingOnInvalidParameter"));
  return false;
}

////////////////////////////////////////////////////////////

bool CCommandBase::IsDelOrEnd() 
{
  return *_buffer == 0 || *_buffer == ',' || *_buffer == ' ' || *_buffer == ';';
}

bool CCommandBase::IsDelimiter() 
{
  return *_buffer == ',';
}

bool CCommandBase::GetInt(long* value)
{
  unsigned char negativ;
  SkipSpaces();
  if ((negativ = (*_buffer == '-')))
  {
    _buffer++;
  }
  if (!isdigit(*_buffer)) return false;

  *value = 0;
  while (isdigit(*_buffer))
  {
    *value *= 10l;
    *value += *_buffer - '0';
    _buffer++;
  }
  if (negativ)
    *value = -(*value);

  return IsDelOrEnd();
}

////////////////////////////////////////////////////////////

bool CCommandBase::IsCommand(char* b, bool resumePen) 
{
  if (_buffer[0] == b[0] && _buffer[1] == b[1])
  {
    //Plotter.Resume(resumePen);
    _buffer += 2;
    SkipSpaces();

    return true;
  }
  if (_buffer[0] == b[0] && _buffer[1] == 0 && b[1]== ' ')
  {
    //Plotter.Resume(resumePen);
    _buffer += 1;                
    return true;
  }

  return false;
}

////////////////////////////////////////////////////////////

bool CCommandBase::IsEndCommand() 
{
  SkipSpaces();
  return *_buffer == ';' || *_buffer == 0;
}

////////////////////////////////////////////////////////////

bool CCommandBase::IsSingleLineCommand() 
{
  SkipSpaces();
  if (IsEndCommand()) return false;
  return *_buffer == '-' || !isdigit(*_buffer);
}

////////////////////////////////////////////////////////////

bool CCommandBase::GetIntWithSeperator(long* value)
{
  if (GetInt(value))
  {
    _cnt++;
    return SkipSeperator();
  }
  return false;
}

////////////////////////////////////////////////////////////

void CCommandBase::ScanOldParam()
{
	_x=_y=_z=_t=_u=0;
	_cnt=0;

	GetIntWithSeperator(&_x) && 
	GetIntWithSeperator(&_y) && 
	GetIntWithSeperator(&_z) && 
	GetIntWithSeperator(&_t) && 
	GetIntWithSeperator(&_u); 

  //	cnt=sscanf(buffer,"%i %i %i %i %i",&x,&y,&z,&t,&u);
	_d[0]=_x;
	_d[1]=_y;
	_d[2]=_z;
	_d[3]=_t;
	_ud[0]=_x;
	_ud[1]=_y;
	_ud[2]=_z;
	_ud[3]=_t;
}

////////////////////////////////////////////////////////////

bool CCommandBase::Command(char* xbuffer)
{
  return false;
}




