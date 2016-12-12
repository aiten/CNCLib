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

#include <stdio.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>

#include <GCodeParserBase.h>
#include <ControlTemplate.h>

#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControlBase MotionControl;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

void CMyControl::Init()
{

#ifdef DISABLELEDBLINK
	DisableBlinkLed();
#endif

	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

#ifdef SETDIRECTION
	CStepper::GetInstance()->SetDirection(SETDIRECTION);
#endif

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CControlTemplate::SetLimitMinMax(MYNUM_AXIS, X_MAXSIZE, Y_MAXSIZE, Z_MAXSIZE, A_MAXSIZE,0,0);
	CControlTemplate::InitReference(X_USEREFERENCE, Y_USEREFERENCE, Z_USEREFERENCE, A_USEREFERENCE);

#ifdef CONTROLLERFAN_FAN_PIN
	#ifdef CONTROLLERFAN_ANALOGSPEED
		_controllerfan.Init(128);
	#else
		_controllerfan.Init();
	#endif
#endif

#ifdef SPINDEL_ENABLE_PIN
	_spindel.Init();
#endif

#ifdef PROBE_PIN
	_probe.Init();
#endif

#ifdef KILL_PIN
	_kill.Init();
#endif

#ifdef COOLANT_PIN
	_coolant.Init();
#endif

#if defined(HOLD_PIN) && defined(RESUME_PIN)
	_hold.SetPin(HOLD_PIN);
	_resume.SetPin(RESUME_PIN);
#endif

	CGCodeParserBase::Init(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED, CNC_ACC, CNC_DEC);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
#ifdef SPINDEL_ENABLE_PIN
		case Spindel:			
			if (level != 0)
			{
#ifdef SPINDEL_ANALOGSPEED
				_spindel.On((uint8_t) MulDivU32(abs(level),255, SPINDEL_MAXSPEED));
#else        
				_spindel.On();
#endif
#ifdef SPINDEL_DIR_PIN
				_spindelDir.Set(((short)level)>0);
#endif
			}
			else
			{
			  _spindel.Off();
			}
			return;
#endif
#ifdef COOLANT_PIN
	    case Coolant:     _coolant.Set(level>0); return;
#endif
#if defined(CONTROLLERFAN_FAN_PIN) && !defined(CONTROLLERFAN_ANALOGSPEED)
		case ControllerFan:		_controllerfan.Set(level>0);	return;
#elif defined(CONTROLLERFAN_FAN_PIN) && defined(CONTROLLERFAN_ANALOGSPEED)
		case ControllerFan:		_controllerfan.Level = (uint8_t)level;		return;
#endif
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
#ifdef PROBE_PIN
		case Probe:			{ return _probe.IsOn(); }
#endif
#ifdef SPINDEL_ENABLE_PIN
		case Spindel:		{ return _spindel.IsOn(); }
#endif
#ifdef COOLANT_PIN
		case Coolant:		{ return _coolant.IsOn(); }
#endif
#if defined(CONTROLLERFAN_FAN_PIN) && !defined(CONTROLLERFAN_ANALOGSPEED)
		case ControllerFan: { return _controllerfan.IsOn(); }
#elif defined(CONTROLLERFAN_FAN_PIN) && defined(CONTROLLERFAN_ANALOGSPEED)
		case ControllerFan: { return _controllerfan.Level; }
#endif	
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
#ifdef SPINDEL_ENABLE_PIN
	_spindel.Off();
#endif
#ifdef COOLANT_PIN
	_coolant.Set(false);
#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
#ifdef KILL_PIN
	return _kill.IsOn();
#else
	return false;
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
#ifdef HOLD_PIN
	_hold.Check();
#endif
#ifdef RESUME_PIN
	_resume.Check();
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
#if defined(CONTROLLERFAN_FAN_PIN) && defined(CONTROLLERFAN_ANALOGSPEED)
	_controllerfan.Level=128;
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

#if defined(HOLD_PIN) && defined(RESUME_PIN)

	if (IsHold())
	{
		if (_resume.IsOn())
		{
			Resume();
		}
	} 
	else if (_hold.IsOn())
	{
		Hold();
	}
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
#ifdef NOGOTOREFERENCEATBOOT

#pragma message ("for test purpose only, not gotoReference at boot")

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	// force linking to see size used in sketch
	if (IsHold())
		super::GoToReference(X_AXIS, CMotionControlBase::FeedRateToStepRate(X_AXIS, 300000), true);

#else

	GotoReference(REFMOVE_1_AXIS, REFMOVE_2_AXIS, REFMOVE_3_AXIS, REFMOVE_4_AXIS);

#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
	return super::GoToReference(axis, STEPRATERATE_REFMOVE, toMinRef);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParserBase gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
#ifdef CONTROLLERFAN_FAN_PIN
	switch (eventtype)
	{
		case OnStartEvent:
			_controllerfan.On();
			break;
		case OnIdleEvent:
			if (millis()-CStepper::GetInstance()->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}
#endif

	return super::OnEvent(eventtype, addinfo);
}
