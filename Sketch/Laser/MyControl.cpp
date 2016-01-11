////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

	StepperSerial.println(MESSAGE_MYCONTROL_Laser_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

#ifdef SETDIRECTION
	CStepper::GetInstance()->SetDirection((1 << X_AXIS) + (1 << Y_AXIS));
#endif

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, X_MAXSIZE));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, Y_MAXSIZE));

#if MYNUM_AXIS > 2
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, Z_MAXSIZE));
#endif

#if MYNUM_AXIS > 3
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS, A_MAXSIZE));
#endif

#ifdef X_USEREFERENCE_MIN
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
#endif
#ifdef X_USEREFERENCE_MAX
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, false), true);
#endif

#ifdef Y_USEREFERENCE_MIN
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
#endif
#ifdef Y_USEREFERENCE_MAX
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, false), true);
#endif

#ifdef Z_USEREFERENCE_MIN
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true), true);
#endif
#ifdef Z_USEREFERENCE_MAX
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);
#endif

#ifdef A_USEREFERENCE_MIN
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, true), true);
#endif
#ifdef A_USEREFERENCE_MAX
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, false), true);
#endif

#ifdef CONTROLLERFAN_FAN_PIN
	#ifdef CONTROLLERFAN_ANALOGSPEED
		_controllerfan.Init(128);
	#else
		_controllerfan.Init();
	#endif
#endif

  _laser.Init();

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

	CGCodeParserBase::Init();

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE));
	CGCodeParserBase::SetG1FeedRate(STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE));
	CGCodeParserBase::SetG1MaxFeedRate(STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED, CNC_ACC, CNC_DEC);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
    case Laser:
      if (level != 0)
      {
#ifdef LASER_ANALOG
        _laser.On((unsigned char) level);
#else        
        _laser.On();
#endif
      }
      else
      {
        _laser.Off();
      }
      return;

#ifdef SPINDEL_ENABLE_PIN
		case Spindel:			
			if (level != 0)
			{
#ifdef SPINDEL_ANALOGSPEED
				_spindel.On((unsigned char) MulDivU32(abs(level),255, SPINDEL_MAXSPEED));
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
		case ControllerFan:		_controllerfan.Level = (unsigned char)level;		return;
#endif
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
    case Laser:   { return _laser.IsOn(); }

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

  _laser.Off();
  
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

#ifdef REFMOVE_1_AXIS
	GoToReference(REFMOVE_1_AXIS, 0, CStepper::GetInstance()->IsUseReference(REFMOVE_1_AXIS, true));
#endif
#ifdef REFMOVE_2_AXIS
	GoToReference(REFMOVE_2_AXIS, 0, CStepper::GetInstance()->IsUseReference(REFMOVE_2_AXIS, true));
#endif
#ifdef REFMOVE_3_AXIS
	GoToReference(REFMOVE_3_AXIS, 0, CStepper::GetInstance()->IsUseReference(REFMOVE_3_AXIS, true));
#endif
#ifdef REFMOVE_4_AXIS
	GoToReference(REFMOVE_4_AXIS, 0, CStepper::GetInstance()->IsUseReference(REFMOVE_4_AXIS, true));
#endif

#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
  return CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMinRef), toMinRef, STEPRATERATE_REFMOVE,0,MOVEAWAYFROMREF_STEPS);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParserBase gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
#ifdef CONTROLLERFAN_FAN_PIN
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis()-stepper->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}
#endif

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
