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
#include <ConfigEeprom.h>

#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControlBase MotionControl;
CConfigEeprom Eprom;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

float scaleToMm;
float scaleToMachine;

static const CMyControl::SCNCEeprom eepromFlash PROGMEM =
{
  0x21436587,
  { X_MAXSIZE,        Y_MAXSIZE,      Y_MAXSIZE,      A_MAXSIZE },
  (1000.0 / X_STEPSPERMM),
  { X_USEREFERENCE,   Y_USEREFERENCE, Z_USEREFERENCE, A_USEREFERENCE },
  { REFMOVE_1_AXIS,   REFMOVE_2_AXIS, REFMOVE_3_AXIS, REFMOVE_4_AXIS },
  CNC_MAXSPEED,CNC_ACC,CNC_DEC,0
};

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(SCNCEeprom), &eepromFlash, 0x21436587);

	scaleToMm = CConfigEeprom::GetSlotFloat(EConfigSlot::ScaleMmToMachine);
	scaleToMachine = 1.0 / scaleToMm;

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

	CControlTemplate::SetLimitMinMax(MYNUM_AXIS, 
		CConfigEeprom::GetSlotU32(EConfigSlot::MaxSize + X_AXIS),
		CConfigEeprom::GetSlotU32(EConfigSlot::MaxSize + Y_AXIS),
		CConfigEeprom::GetSlotU32(EConfigSlot::MaxSize + Z_AXIS),
		CConfigEeprom::GetSlotU32(EConfigSlot::MaxSize + A_AXIS),
		0, 0);
	CControlTemplate::InitReference(
		(EReverenceType) CConfigEeprom::GetSlotU8(EConfigSlot::ReferenceType, X_AXIS),
		(EReverenceType) CConfigEeprom::GetSlotU8(EConfigSlot::ReferenceType, Y_AXIS),
		(EReverenceType) CConfigEeprom::GetSlotU8(EConfigSlot::ReferenceType, Z_AXIS),
		(EReverenceType) CConfigEeprom::GetSlotU8(EConfigSlot::ReferenceType, A_AXIS));

	_controllerfan.Init(128);

	_spindel.Init();
	_probe.Init();
	_kill.Init();
	_coolant.Init();

#if defined(HOLD_PIN)
  _hold.SetPin(HOLD_PIN);
#endif
#if defined(RESUME_PIN)
	_resume.SetPin(RESUME_PIN);
#endif

	CGCodeParserBase::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));
	CStepper::GetInstance()->SetDefaultMaxSpeed(
		((steprate_t)CConfigEeprom::GetSlotU32(EConfigSlot::MaxStepRate)),
		((steprate_t)CConfigEeprom::GetSlotU32(EConfigSlot::Acc)),
		((steprate_t)CConfigEeprom::GetSlotU32(EConfigSlot::Dec)));

	CSingleton<CConfigEeprom>::GetInstance()->PrintConfig();
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:		_spindel.On(ConvertSpindelSpeedToIO(level)); _spindelDir.Set(((short)level) > 0);	return;
		case Coolant:		_coolant.Set(level > 0); return;
		case ControllerFan:	_controllerfan.SetLevel((uint8_t)level); return;
	}

	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case Spindel: { return _spindel.IsOn(); }
		case Probe: { return _probe.IsOn(); }
		case Coolant: { return _coolant.IsOn(); }
		case ControllerFan: { return _controllerfan.GetLevel(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();

	_spindel.Off();
	_coolant.Set(false);
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	//return false;
	return _kill.IsOn();
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();

	_hold.Check();
	_resume.Check();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

	_controllerfan.SetLevel(128);
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

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
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	for (axis_t i = 0; i < EEPROM_NUM_AXIS; i++)
	{
		axis_t axis = CConfigEeprom::GetSlotU8(EConfigSlot::RefMove, i);
		if (axis < EEPROM_NUM_AXIS)
		{
			EnumAsByte(EReverenceType) referenceType = (EReverenceType)CConfigEeprom::GetSlotU8(EConfigSlot::ReferenceType, axis);
			if (referenceType != EReverenceType::NoReference)
				GoToReference(axis,	(steprate_t) CConfigEeprom::GetSlotU32(EConfigSlot::RefMoveStepRate),referenceType == EReverenceType::ReferenceToMin);
		}
	}
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t steprate, bool toMinRef)
{
	return super::GoToReference(axis, steprate, toMinRef);
	//	return CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMinRef), toMinRef, STEPRATERATE_REFMOVE, 0, MOVEAWAYFROMREF_STEPS);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParserBase gcode(reader, output);
	return ParseAndPrintResult(&gcode, output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
	switch (eventtype)
	{
		case OnStartEvent:
			_controllerfan.On();
			break;
		case OnIdleEvent:
			if (IsControllerFanTimeout())
			{
				_controllerfan.Off();
			}
			break;
	}

	return super::OnEvent(eventtype, addinfo);
}
