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

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////

#include <stdio.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <SPI.h>
#include <SD.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include <GCodeParserBase.h>
#include <GCodeParser.h>
#include "StepperServo.h"
#include "MyControl.h"
#include "MyMotionControl.h"
#include "MyParser.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

CMyMotionControl MotionControl;
CConfigEeprom Eprom;

HardwareSerial& StepperSerial = Serial;
const CConfigEeprom::SCNCEeprom CMyControl::_eepromFlash PROGMEM =
{
	EPROM_SIGNATURE,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	GetInfo1a(),GetInfo1b(),
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

	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	CStepper::GetInstance()->SetLimitMin(X_AXIS, MIN_LIMIT);  // ms
	CStepper::GetInstance()->SetLimitMin(Y_AXIS, MIN_LIMIT);
	CStepper::GetInstance()->SetLimitMin(Z_AXIS, MIN_LIMIT);
	CStepper::GetInstance()->SetLimitMin(A_AXIS, MIN_LIMIT);

	CStepper::GetInstance()->SetLimitMax(X_AXIS, MAX_LIMIT);  // ms
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, MAX_LIMIT);
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, MAX_LIMIT);
	CStepper::GetInstance()->SetLimitMax(A_AXIS, MAX_LIMIT);

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED, CNC_ACC, CNC_DEC);
	CGCodeParserDefault::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), G1_DEFAULT_FEEDPRATE, STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

#ifdef MYUSE_LCD
	InitSD(SD_ENABLE_PIN);
#endif

}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	if (_data.IsKill())
	{
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("LCD E-Stop"));
#endif
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
  CStepper::GetInstance()->SetPosition(X_AXIS, INIT_PULS1);
  CStepper::GetInstance()->SetPosition(Y_AXIS, INIT_PULS2);
  CStepper::GetInstance()->SetPosition(Z_AXIS, INIT_PULS3);
  CStepper::GetInstance()->SetPosition(A_AXIS, CENTER_LIMIT);

  ((CStepperServo*)CStepper::GetInstance())->SetServo();
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CMyParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////
