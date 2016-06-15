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

#pragma once

////////////////////////////////////////////////////////

#include <StepperLib.h>
#include "ConfigurationCNCLib.h"
#include "Parser.h"
#include "Lcd.h"
#include "MenuBase.h"

////////////////////////////////////////////////////////
// Control: read from USB (or File) and pass string to the Parser

class CControl : public CSingleton<CControl>
{
public:

	CControl();

	virtual void Run();				// run Controller => do not return
	virtual void Kill();			// stop everyting => Emergency Stop
	virtual void Resurrect();		// Call after Kill to restart again

	bool IsKilled()												{ return CStepper::GetInstance()->IsEmergencyStop(); }

	void StopProgram(bool checkconditional);		// see m00 / m01

	void SetConditionalStop(bool stop);				// see m00 / m01
	bool IsConditionalStop();

	void Delay(unsigned long ms);	// delay with idle processing

	void Hold();					// stop executing programm (with down-ramp), must not be called in timerinterrupt
	void Resume();					// continue executing (start queue from stepper)
	bool IsHold()												{ return CStepper::GetInstance()->IsPauseMove(); }

	//////////////////////////////////////////

	enum EIOTools
	{
		// Output
		Spindel,
		Coolant,
		ControllerFan,
		Vacuum,
		Laser,

		// input
		Probe,				// Probe for tool lenght
		ProgramStop			// see M01
	};

	enum ECoolantState
	{
		CoolantOff = 0,
		CoolantOn = 1,
		CoolantFlood = 2
	};

	enum EVacuumState
	{
		VacuumOff = 0,
		VacuumOn = 1
	};

	virtual void IOControl(uint8_t /* tool */, unsigned short /*level */)	{ };
	virtual unsigned short IOControl(uint8_t /* tool */)				{ return 0; };

	//////////////////////////////////////////

	virtual void GoToReference()=0;								// Goto Refernce during Initialisation
	virtual bool GoToReference(axis_t axis,steprate_t steprate, bool toMinRef);

	//////////////////////////////////////////

	void StartPrintFromSD()				{ _printFromSDFile = true; }
	void ClearPrintFromSD()				{ _printFromSDFile = false; }
	bool PrintFromSDRunnding()			{ return _printFromSDFile; }

	//////////////////////////////////////////

	bool PostCommand(const __FlashStringHelper* cmd, Stream* output=NULL);
	bool PostCommand(char* cmd, Stream* output=NULL);

	//////////////////////////////////////////

	const char* GetBuffer()				{ return _buffer; }
	virtual bool IsEndOfCommandChar(char ch);					// override default End of command char, default \n

protected:

	bool SerialReadAndExecuteCommand();							// read from serial an execut command, return true if command pending (buffer not empty)
	void FileReadAndExecuteCommand(Stream* stream, Stream* output);// read command until "IsEndOfCommandChar" and execute command (NOT Serial)

	virtual void Init();
	virtual void Initialized();									// called if Init() is done

	virtual bool Parse(CStreamReader* reader, Stream* output)=0;// abstract: specify Parser
	virtual bool Command(char* xbuffer, Stream* output);		// execute Command (call parser)
	virtual void Idle(unsigned int idletime);					// called after TIMEOUTCALLIDEL in idle state
	virtual void Poll();										// call in Idle and at least e.g. 100ms (not in interrupt), see CheckIdlePoll
	virtual void ReadAndExecuteCommand();						// read and execute commands from other source e.g. SD.File

	virtual void TimerInterrupt();								// called from timer (timer0 on AVR) 

	virtual bool IsKill() = 0;

	bool ParseAndPrintResult(CParser* parser, Stream* output);

	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo);

	bool IsResurrectCommand(const char*buffer)					{ return buffer[0] == '!' && buffer[1] == '!' && buffer[2] == '!' && (buffer[3] == 0 || (buffer[3] == '\r' && buffer[4] == 0)); }

	void DisableBlinkLed()										{ _timeBlink = 0xffffffff;  }

private:

	void ReadAndExecuteCommand(Stream* stream, Stream* output, bool filestream);	// read command until "IsEndOfCommandChar" and execute command (Serial or SD.File)

	void CheckIdlePoll(bool isidle);							// check idle time and call Idle every 100ms


	uint8_t	_bufferidx;									// read Buffer index , see SERIALBUFFERSIZE

	unsigned long	_lasttime;									// time last char received
	unsigned long	_timeBlink;									// time to change blink state
	unsigned long	_timePoll;									// time call poll next

	CStepper::SEvent _oldStepperEvent;

	bool			_dummy;										// see gcode m01 & m02
	bool			_printFromSDFile;

	char			_buffer[SERIALBUFFERSIZE];					// serial input buffer

	static void HandleInterrupt()								{ GetInstance()->TimerInterrupt(); }

	static bool MyStepperEvent(CStepper*stepper, void* param, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo) { return ((CControl*)param)->OnStepperEvent(stepper, eventtype, addinfo); }

	CStreamReader		_reader;

	void PrintError(Stream* output)								{ output->print(MESSAGE_ERROR); }
};

////////////////////////////////////////////////////////
