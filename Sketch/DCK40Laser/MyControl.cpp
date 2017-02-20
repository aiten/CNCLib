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
#include <GCodeParser.h>

#include "MyControl.h"
#include "MyLCD.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControl MotionControl;
CConfigEeprom Eprom;
HardwareSerial& StepperSerial = Serial;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

const CConfigEeprom::SCNCEeprom CMyControl::_eepromFlash PROGMEM =
{
	EPROM_SIGNATURE,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	GetInfo1a()|CConfigEeprom::IS_LASER|CConfigEeprom::HAVE_SPINDLE|CConfigEeprom::HAVE_SPINDLE_ANALOG,0,
	0,
	STEPPERDIRECTION,0,0,SPINDEL_FADETIMEDELAY,
	SPINDLE_MAXSPEED,
	CNC_JERKSPEED,
	CNC_MAXSPEED,
	CNC_ACC,
	CNC_DEC,
	STEPRATERATE_REFMOVE,
	MOVEAWAYFROMREF_MM1000,
	X_STEPSPERMM/1000.0,
	{
		{ X_MAXSIZE,     X_USEREFERENCE, REFMOVE_1_AXIS,  X_REFERENCEHITVALUE_MIN, X_REFERENCEHITVALUE_MAX },
		{ Y_MAXSIZE,     Y_USEREFERENCE, REFMOVE_2_AXIS,  Y_REFERENCEHITVALUE_MIN, Y_REFERENCEHITVALUE_MAX },
		{ Z_MAXSIZE,     Z_USEREFERENCE, REFMOVE_3_AXIS,  Z_REFERENCEHITVALUE_MIN, Z_REFERENCEHITVALUE_MAX },
#if NUM_AXIS > 3
		{ A_MAXSIZE,     A_USEREFERENCE, REFMOVE_4_AXIS,  A_REFERENCEHITVALUE_MIN, A_REFERENCEHITVALUE_MAX },
#endif
#if NUM_AXIS > 4
		{ B_MAXSIZE,     B_USEREFERENCE, REFMOVE_5_AXIS,  B_REFERENCEHITVALUE_MIN, B_REFERENCEHITVALUE_MAX },
#endif
#if NUM_AXIS > 5
		{ C_MAXSIZE,     C_USEREFERENCE, REFMOVE_6_AXIS,  C_REFERENCEHITVALUE_MIN, C_REFERENCEHITVALUE_MAX },
#endif
	}
};

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(CConfigEeprom::SCNCEeprom), &_eepromFlash, EPROM_SIGNATURE);

#ifdef DISABLELEDBLINK
	DisableBlinkLed();
#endif

	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	super::Init();

	InitFromEeprom();

	_data.Init();

	_laserPWM.Init();
	_laserOnOff.Init();

	_laserWater.Init();
	_laserVacuum.Init();

	CGCodeParserDefault::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), G1_DEFAULT_FEEDPRATE, STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

#ifdef MYUSE_LCD
	InitSD(SD_ENABLE_PIN);
#endif

}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
		case SpindleCCW:
		case SpindleCW:

			if (level != 0)
			{
				_laserPWM.On((uint8_t)level);
				_laserOnOff.On();
			}
			else
			{
				_laserOnOff.Off();
			}
			return;

		case Vacuum:  _laserVacuum.Set(level > 0); return;
	//  case Coolant: _laserWater.Set(level > 0); return; do not allow water turn off

	}

	if (!_data.IOControl(tool, level))
	{
		super::IOControl(tool, level);
	}
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case SpindleCW:
		case SpindleCCW:	{ return _laserPWM.IsOn(); }
		case Probe:			{ return _data._probe.IsOn(); }
		case Coolant: 		{ return _laserWater.IsOn(); }
		case Vacuum: 		{ return _laserVacuum.IsOn(); }
		case ControllerFan: { return _data._controllerfan.GetLevel(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
	_data.Kill();
	_laserOnOff.Off();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	if (false && _data.IsKill())
	{
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("E-Stop"));
#endif
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
	_data.TimerInterrupt();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
	_data.Initialized();
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_data._resume.IsOn() || _data._holdresume.IsOn())
		{
			Resume();
#ifdef MYUSE_LCD
			Lcd.ClearDiagnostic();
#endif
		}
	}
	else if (_data._hold.IsOn() || _data._holdresume.IsOn())
	{
		Hold();
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("LCD Hold"));
#endif
	}
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
	_data.OnEvent(eventtype, addinfo);

	switch (eventtype)
	{
		case OnStartCut:
		{
			if (CGCodeParserBase::IsSpindleOn())
			{
				bool newIsCutMove = addinfo != 0;
				if (CGCodeParserBase::IsCutMove() != newIsCutMove)
				{
					CStepper::GetInstance()->IoControl(CControl::SpindleCW, newIsCutMove ? CGCodeParserBase::GetSpindleSpeed() : 0);
				}
			}
			break;
		}
		case OnStartEvent:
			_laserWater.On();
			_laserVacuum.On();
			break;
		case OnIdleEvent:
			if (_laserOnOff.IsOn() == false)
			{
				if (millis() - CStepper::GetInstance()->IdleTime() > LASERWATER_ONTIME)
				{
					_laserWater.Off();
				}
				if (millis() - CStepper::GetInstance()->IdleTime() > LASERVACUUM__ONTIME)
				{
					_laserVacuum.Off();
				}
			}
			break;
	}

	return super::OnEvent(eventtype, addinfo);
}
