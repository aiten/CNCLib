////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
#include <ConfigEeprom.h>

#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;

#ifdef REDUCED_SIZE
CMotionControlBase MotionControl;
#define CMyParser CGCodeParserBase
#define InitParser()
#else
#include <GCodeParser.h>
CMotionControl MotionControl;
#define CMyParser CGCodeParser
#define InitParser() CGCodeParser::Init()
#endif

CConfigEeprom Eprom;
HardwareSerial& StepperSerial = Serial;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

float scaleToMm;
float scaleToMachine;

mm1000_t MyConvertToMm1000(axis_t axis, sdist_t val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (mm1000_t)(val * scaleToMm);
		case Y_AXIS: return  (mm1000_t)(val * scaleToMm);
		case Z_AXIS: return  (mm1000_t)(val * scaleToMm);
		case A_AXIS: return  (mm1000_t)(val * scaleToMm);
		case B_AXIS: return  (mm1000_t)(val * scaleToMm);
		case C_AXIS: return  (mm1000_t)(val * scaleToMm);
	}
}

sdist_t MyConvertToMachine(axis_t axis, mm1000_t  val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (sdist_t)(val * scaleToMachine);
		case Y_AXIS: return  (sdist_t)(val * scaleToMachine);
		case Z_AXIS: return  (sdist_t)(val * scaleToMachine);
		case A_AXIS: return  (sdist_t)(val * scaleToMachine);
		case B_AXIS: return  (sdist_t)(val * scaleToMachine);
		case C_AXIS: return  (sdist_t)(val * scaleToMachine);
	}
}

////////////////////////////////////////////////////////////

static const CConfigEeprom::SCNCEeprom eepromFlash PROGMEM =
{
	0x21436587,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	0,
	CNC_MAXSPEED,
	CNC_ACC,
	CNC_DEC,
	STEPRATERATE_REFMOVE,
	(1000.0 / X_STEPSPERMM),
	{
		{ X_MAXSIZE,     X_USEREFERENCE, REFMOVE_1_AXIS },
		{ Y_MAXSIZE,     Y_USEREFERENCE, REFMOVE_2_AXIS },
		{ Z_MAXSIZE,     Z_USEREFERENCE, REFMOVE_3_AXIS },
#if NUM_AXIS > 3
		{ A_MAXSIZE,     A_USEREFERENCE, REFMOVE_4_AXIS },
#endif
#if NUM_AXIS > 4
		{ B_MAXSIZE,     B_USEREFERENCE, REFMOVE_5_AXIS },
#endif
#if NUM_AXIS > 5
		{ C_MAXSIZE,     C_USEREFERENCE, REFMOVE_5_AXIS },
#endif
	}
};

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(CConfigEeprom::SCNCEeprom), &eepromFlash, 0x21436587);

	scaleToMm = CConfigEeprom::GetConfigFloat(offsetof(CConfigEeprom::SCNCEeprom, ScaleMm1000ToMachine));
	scaleToMachine = 1.0 / scaleToMm;

#ifdef DISABLELEDBLINK
	DisableBlinkLed();
#endif

	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

#ifdef SETDIRECTION
	CStepper::GetInstance()->SetDirection(SETDIRECTION);
#endif

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	for (uint8_t axis = 0; axis < NUM_AXIS; axis++)
	{
		EnumAsByte(EReverenceType) ref = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
		if (ref != NoReference)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(axis, ref == EReverenceType::ReferenceToMin), true);

		CStepper::GetInstance()->SetLimitMax(axis, CMotionControlBase::GetInstance()->ToMachine(axis, CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, axis[0].size) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis)));
	}

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

  InitParser();
	CGCodeParserBase::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));
	CStepper::GetInstance()->SetDefaultMaxSpeed(
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, acc))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, dec))));
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
		case Spindel:		{ return _spindel.IsOn(); }
		case Probe:			{ return _probe.IsOn(); }
		case Coolant:		{ return _coolant.IsOn(); }
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
	for (axis_t i = 0; i < NUM_AXIS; i++)
	{
		axis_t axis = CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].refmoveSequence)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*i);
		if (axis < NUM_AXIS)
		{
			EnumAsByte(EReverenceType) referenceType = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
			if (referenceType != EReverenceType::NoReference)
				GoToReference(axis,	(steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, refmovesteprate)),referenceType == EReverenceType::ReferenceToMin);
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
	CMyParser gcode(reader, output);
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
