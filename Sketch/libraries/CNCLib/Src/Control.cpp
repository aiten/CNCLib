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
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Control.h"
#include "Lcd.h"

#include "GCodeParserBase.h"

////////////////////////////////////////////////////////////

template<> CControl* CSingleton<CControl>::_instance = NULL;

////////////////////////////////////////////////////////

CControl::CControl()
{
	_bufferidx = 0;
}

////////////////////////////////////////////////////////////

void CControl::Init()
{
	CStepper::GetInstance()->Init();
	CStepper::GetInstance()->AddEvent(MyStepperEvent, this, _oldStepperEvent);

#ifdef _USE_LCD
	
	if (CLcd::GetInstance())
		CLcd::GetInstance()->Init();

#endif
	
	if (_timeBlink==0)
		CHAL::pinMode(BLINK_LED, OUTPUT);

	CHAL::InitTimer0(HandleInterrupt);
	CHAL::StartTimer0(IDLETIMER0VALUE);
}

////////////////////////////////////////////////////////////

void CControl::Initialized()
{
	StepperSerial.println(MESSAGE_OK);
	GoToReference();
	CMotionControlBase::GetInstance()->SetPositionFromMachine();
}

////////////////////////////////////////////////////////////

bool CControl::GoToReference(axis_t axis, steprate_t steprate, bool toMinRef)
{
	if (steprate == 0)
		steprate = CStepper::GetInstance()->GetDefaultVmax();
	// goto min/max
	return CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMinRef), toMinRef, steprate);
}

////////////////////////////////////////////////////////////

void CControl::Kill()
{
	// may be in ISR context, do not print anything
	CStepper::GetInstance()->EmergencyStop();
	CMotionControlBase::GetInstance()->SetPositionFromMachine();
}

////////////////////////////////////////////////////////

void CControl::Resurrect()
{
	CStepper::GetInstance()->EmergencyStopResurrect();
	CMotionControlBase::GetInstance()->SetPositionFromMachine();

#ifdef _USE_LCD
	
	if (CLcd::GetInstance())
		CLcd::GetInstance()->ClearDiagnostic();

#endif

	_bufferidx = 0;
	StepperSerial.println(MESSAGE_OK);
}

////////////////////////////////////////////////////////////

void CControl::StopProgram(bool /*checkconditional*/)
{
}

////////////////////////////////////////////////////////////

void CControl::Idle(unsigned int /*idletime*/)
{
}

////////////////////////////////////////////////////////////

void CControl::Hold()
{
	CStepper::GetInstance()->PauseMove();
}

////////////////////////////////////////////////////////////

void CControl::Resume()
{
	CStepper::GetInstance()->ContinueMove();
}

////////////////////////////////////////////////////////////

void CControl::Poll()
{
#ifdef _USE_LCD
	if (CLcd::GetInstance())
		CLcd::GetInstance()->Poll();
#endif
}

////////////////////////////////////////////////////////////

bool CControl::ParseAndPrintResult(CParser *parser, Stream* output)
{
	bool ret = true;

#define SENDOKIMMEDIATELY
#undef SENDOKIMMEDIATELY

#ifdef SENDOKIMMEDIATELY 
	///////////////////////////////////////////////////////////////////////////
	// send OK pre Parse => give PC time to send next

	if (output) output->println(MESSAGE_OK);

	parser->ParseCommand();

	if (parser->GetError() != NULL)
	{
		if (output)
		{
			PrintError(output);
			output->print(parser->GetError());
			output->print(MESSAGE_CONTROL_RESULTS);
			output->println(_buffer);
		}
		ret = false;
	}

	if (parser->GetOkMessage() != NULL)
	{
		if (output)
		{
			output->print(MESSAGE_OK);
			output->print(F(" "));
		}
		parser->GetOkMessage()();
		if (output) output->println();
	}

#else

	///////////////////////////////////////////////////////////////////////////
	// send OK after Parse

	parser->ParseCommand();

	if (parser->GetError() != NULL)
	{
		if (output) 
		{
			PrintError(output);
			output->print(parser->GetError());
			output->print(MESSAGE_CONTROL_RESULTS);
			output->println(_buffer);
		}
		ret = false;
	}

	if (output) output->print(MESSAGE_OK);
	if (parser->GetOkMessage() != NULL)
	{
		if (output)
		{
			output->print(F(" "));
		}
		parser->GetOkMessage()();
	}

	if (output) output->println();

#endif

	return ret;
}

////////////////////////////////////////////////////////////

bool CControl::Command(char* buffer, Stream* output)
{
#ifdef _USE_LCD

	if (CLcd::GetInstance())
		CLcd::GetInstance()->Command(buffer);

#endif

	if (IsKilled())
	{
#ifndef REDUCED_SIZE
		if (IsResurrectCommand(buffer))		// restart with "!!!"
		{
			Resurrect();
			return true;
		}
#endif
		if (output)
		{
			PrintError(output); output->println(MESSAGE_CONTROL_KILLED);
		}
		return false;
	}

	// if one Parse failes, return false
	
	bool ret = true;
	
	_reader.Init(buffer);

	if (_reader.GetChar())
	{
		while (_reader.GetChar())
		{
			if (!Parse(&_reader,output))
				ret = false;
		}
	}
	else if (output)
	{
		// send OK on empty line (command)
		output->println(MESSAGE_OK);
	}
	
	return ret;
}

////////////////////////////////////////////////////////////

bool CControl::IsEndOfCommandChar(char ch)
{
	//return ch == '\n' || ch == '\r' || ch == -1;
	return ch == '\n' || ch == -1;
}

////////////////////////////////////////////////////////////

void CControl::ReadAndExecuteCommand(Stream* stream, Stream* output, bool filestream)
{
	// call this methode if ch is available in stream

	if (stream->available() > 0)
	{
		while (stream->available() > 0)
		{
			char ch = _buffer[_bufferidx] = stream->read();

			if (IsEndOfCommandChar(ch))
			{
				_buffer[_bufferidx] = 0;			// remove from buffer 
				Command(_buffer, output);
				_bufferidx = 0;

				_lasttime = millis();

				return;
			}

			_bufferidx++;
			if (_bufferidx >= sizeof(_buffer))
			{
				if (output)
				{
					PrintError(output); output->println(MESSAGE_CONTROL_FLUSHBUFFER);
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
	ReadAndExecuteCommand(stream, output, true);
}

////////////////////////////////////////////////////////////

void CControl::Run()
{
	_bufferidx = 0;
	_lasttime = _timeBlink = _timePoll = 0;

	Init();
	Initialized();

#ifdef _MSC_VER
	while (!CGCodeParserBase::_exit)
#else
	while (true)
#endif
	{
		if (IsHold())
		{
			CheckIdlePoll(true);
		}
		else
		{
			while (SerialReadAndExecuteCommand())
			{
				// wait until serial command processed
				CheckIdlePoll(true);
			}

			CheckIdlePoll(true);

			ReadAndExecuteCommand();
		}
	}
}

////////////////////////////////////////////////////////////

void CControl::CheckIdlePoll(bool isidle)
{
	unsigned long time = millis();

	if (isidle && _lasttime + TIMEOUTCALLIDEL < time)
	{
		Idle(time - _lasttime);
		Poll();
		_timePoll = time;
	}
	else if(_timePoll + TIMEOUTCALLPOLL < time)
	{
		Poll();
		_timePoll = time;
	}

	if (_timeBlink < time)
	{
		HALFastdigitalWrite(BLINK_LED, CHAL::digitalRead(BLINK_LED) == HIGH ? LOW : HIGH);
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

	if (!IsKilled())
	{
		if (IsKill())
		{
			Kill();
		}
	}

#ifdef _USE_LCD

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
#ifdef _USE_LCD
		if (CLcd::GetInstance())
			CLcd::GetInstance()->Poll();
#endif
	}
}

////////////////////////////////////////////////////////////

bool CControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	switch (eventtype)
	{
		case CStepper::OnWaitEvent:

			if (CStepper::WaitTimeCritical > (CStepper::EWaitType) (unsigned int)addinfo)
			{
				CheckIdlePoll(false);
			}
			break;

		case CStepper::OnIoEvent:
			
			IOControl(((CStepper::SIoControl*) addinfo)->_tool, ((CStepper::SIoControl*) addinfo)->_level);
			break;
	}
	return _oldStepperEvent.Call(stepper, eventtype, addinfo);
}
