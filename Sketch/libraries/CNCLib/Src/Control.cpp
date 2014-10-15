////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Control.h"
#include "Lcd.h"

#ifdef _MSC_VER
#include "GCodeParserBase.h"
#endif

////////////////////////////////////////////////////////////

template<> CControl* CSingleton<CControl>::_instance = NULL;

////////////////////////////////////////////////////////

CControl::CControl()
{
	_bufferidx = 0;
	_pause = false;

	_oldStepperEvent = NULL;
}

////////////////////////////////////////////////////////////

void CControl::Init()
{
	CStepper::GetInstance()->Init();
	CStepper::GetInstance()->AddEvent(MyStepperEvent, this, _oldStepperEvent, _oldStepperEventParam);

#ifndef _NO_LCD
	
	if (CLcd::GetInstance())
		CLcd::GetInstance()->Init();

#endif
		
	CHAL::pinMode(BLINK_LED, OUTPUT);

	CHAL::InitTimer0(HandleInterrupt);
	CHAL::StartTimer0(IDLETIMER0VALUE);
}

////////////////////////////////////////////////////////////

void CControl::Initialized()
{
	StepperSerial.println(MESSAGE_OK);
}

////////////////////////////////////////////////////////////

void CControl::Kill()
{
	// may be in ISR context, do not print anything
	CStepper::GetInstance()->EmergencyStop();
}

////////////////////////////////////////////////////////

void CControl::Resurrect()
{
	CStepper::GetInstance()->EmergencyStopResurrect();
	_bufferidx = 0;
	StepperSerial.println(MESSAGE_OK);
}

////////////////////////////////////////////////////////////

void CControl::Pause()
{
	_pause = true;
}

////////////////////////////////////////////////////////////

void CControl::Continue()
{
	_pause = false;
}

////////////////////////////////////////////////////////////


void CControl::Idle(unsigned int idletime)
{
#ifndef _NO_LCD
	if (CLcd::GetInstance())
		CLcd::GetInstance()->Idle(idletime);
#endif
}

////////////////////////////////////////////////////////////

bool CControl::ParseAndPrintResult(CParser *parser, Stream* output)
{
// send OK pre Parse => give PC time to send next
#define SENDOKIMMEDIATELY
#undef SENDOKIMMEDIATELY

#ifdef SENDOKIMMEDIATELY 
	if (output) output->println(MESSAGE_OK);
#endif

	bool ret = true;

	parser->ParseCommand();

	if (parser->GetError() != NULL)
	{
		if (output) 
		{
			output->print(MESSAGE_ERROR);
			output->print(parser->GetError());
			output->print(MESSAGE_CONTROL_RESULTS);
			output->println(_buffer);
		}
		ret = false;
	}

#ifndef SENDOKIMMEDIATELY 
	if (output) output->print(MESSAGE_OK);
#endif
	if (parser->GetOkMessage() != NULL)
	{
		if (output)
		{
#ifdef SENDOKIMMEDIATELY 
			output->print(MESSAGE_OK);
#endif
			output->print(F(" "));
		}
		parser->GetOkMessage()();
#ifdef SENDOKIMMEDIATELY 
		if (output) output->println();
#endif
	}

#ifndef SENDOKIMMEDIATELY 
	if (output) output->println();
#endif

	return ret;
}

////////////////////////////////////////////////////////////

bool CControl::Command(char* buffer, Stream* output)
{
	if (IsKilled())
	{
		if (IsResurrectCommand(buffer))		// restart with "!!!"
		{
			Resurrect();
			return true;
		}
		if (output)
		{
			output->print(MESSAGE_ERROR); output->println(MESSAGE_CONTROL_KILLED);
		}
		return false;
	}

	// if one Parse failes, return false
	
	bool ret = true;
	
	_reader.Init(buffer);

	while (_reader.GetChar())
	{
		if (!Parse(&_reader,output))
			ret = false;
	}
	
	return ret;
}

////////////////////////////////////////////////////////////

bool CControl::IsEndOfCommandChar(char ch)
{
	return ch == '\n' || ch == '\r' || ch == -1;
}

////////////////////////////////////////////////////////////

void CControl::ReadAndExecuteCommand(Stream* stream, Stream* output, bool filestream)
{
	// call this methode if ch is available in stream

	if (stream->available() > 0)
	{
		while (stream->available() > 0)
		{
			_buffer[_bufferidx] = stream->read();

			if (IsEndOfCommandChar(_buffer[_bufferidx]))
			{
				_buffer[_bufferidx] = 0;			// remove from buffer 
				Command(_buffer, output);
				_bufferidx = 0;

				_lasttime = millis();

				return;
			}

			_bufferidx++;
			if (_bufferidx > sizeof(_buffer))
			{
				if (output)
				{
					output->print(MESSAGE_ERROR); output->println(MESSAGE_CONTROL_FLUSHBUFFER);
				}
				_bufferidx = 0;
			}
		}

		if (filestream)						// e.g. SD card => execute last line without "EndOfLine"
		{
			if (_bufferidx > 0)
			{
				_buffer[_bufferidx + 1] = 0;
				Command(_buffer,output);
				_bufferidx = 0;
			}
		}
		_lasttime = millis();
	}
}

////////////////////////////////////////////////////////////

bool CControl::SerialReadAndExecuteCommand()
{
	if (StepperSerial.available() > 0)
	{
		ReadAndExecuteCommand(&StepperSerial, &StepperSerial, false);			
	}

	return _bufferidx > 0;		// command pending, buffer not empty
}

////////////////////////////////////////////////////////

void CControl::FileReadAndExecuteCommand(Stream* stream, Stream* output)
{
	if (!_pause)
		ReadAndExecuteCommand(stream, output, true);
}

////////////////////////////////////////////////////////////

void CControl::Run()
{
	_bufferidx = 0;
	_lasttime = _timeBlink = 0;

	Init();
	Initialized();

#ifdef _MSC_VER
	while (!CGCodeParserBase::_exit)
#else
	while (true)
#endif
	{
		while (SerialReadAndExecuteCommand())
		{
			// wait until serial command processed
			CheckIdle();
		}

		CheckIdle();

		ReadAndExecuteCommand();
	}
}

////////////////////////////////////////////////////////////

void CControl::CheckIdle()
{
	unsigned long time = millis();

	if (_lasttime + TIMEOUTCALLIDEL < time)
	{
		Idle(time - _lasttime);
	}

	if (_timeBlink < time)
	{
		HALFastdigitalWrite(BLINK_LED, digitalRead(BLINK_LED) == HIGH ? LOW : HIGH);
		_timeBlink = time + TIMEOUTBLINK;
	}
}

////////////////////////////////////////////////////////////

void CControl::ReadAndExecuteCommand()
{
	// override for alternative command source e.g. File
}

////////////////////////////////////////////////////////////

bool CControl::PostCommand(const __FlashStringHelper* cmd, Stream* output)
{
	if (_bufferidx > 0) return false;
	const char* cmd1 = (const char*) cmd;

	for (;_bufferidx<sizeof(_buffer);_bufferidx++)
	{
		_buffer[_bufferidx] = pgm_read_byte(&cmd1[_bufferidx]);

		if (_buffer[_bufferidx] == 0)
		{
			bool ret = Command(_buffer,output);
			_bufferidx=0;
			return ret;
		}
	}

	_bufferidx = 0;
	return false;
}

////////////////////////////////////////////////////////////

bool CControl::PostCommand(char* cmd, Stream* output)
{
	if (_bufferidx > 0) return false;

	for (;_bufferidx<sizeof(_buffer);_bufferidx++)
	{
		_buffer[_bufferidx] = cmd[_bufferidx];

		if (_buffer[_bufferidx] == 0)
		{
			bool ret = Command(_buffer,output);
			_bufferidx=0;
			return ret;
		}
	}

	_bufferidx = 0;
	return false;
}


////////////////////////////////////////////////////////////

void CControl::TimerInterrupt()
{
	CHAL::EnableInterrupts();	// enable irq for timer1 (Stepper)

#ifndef _NO_LCD

	if (CLcd::GetInstance())
		CLcd::GetInstance()->TimerInterrupt();

#endif
}

////////////////////////////////////////////////////////////

void CControl::Delay(unsigned long ms)
{
	unsigned long expected_end = millis() + ms;

	while (expected_end > millis())
	{
#ifndef _NO_LCD
		if (CLcd::GetInstance())
			CLcd::GetInstance()->Idle(0);
#endif
	}
}

////////////////////////////////////////////////////////////

bool CControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
#ifndef _NO_LCD
	switch (eventtype)
	{
		case CStepper::OnWaitEvent:

			if (CStepper::WaitTimeCritical > (CStepper::EWaitType) (unsigned int) addinfo && CLcd::GetInstance())
				CLcd::GetInstance()->DrawRequest(false, CLcd::DrawStepperPos);
			break;
	}
#endif
	return _oldStepperEvent ? _oldStepperEvent(stepper, _oldStepperEventParam, eventtype, addinfo) : true;
}
