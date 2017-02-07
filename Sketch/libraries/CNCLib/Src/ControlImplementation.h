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
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#include <Control.h>
#include <ConfigEeprom.h>

#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <Analog8IOControlSmooth.h>
#include <Analog9IOControlSmooth.h>
#include <Analog8IOControlDirSmooth.h>
#include <ReadPinIOControl.h>
#include <PushButtonLow.h>
#include <DummyIOControl.h>

////////////////////////////////////////////////////////

struct ControlData
{
#ifdef SPINDLE_ENABLE_PIN
#ifdef SPINDLE_ANALOGSPEED
#if SPINDLE_MAXSPEED == 255
	CAnalog8IOControl<SPINDLE_ENABLE_PIN> _spindle;
	inline uint8_t ConvertSpindleSpeedToIO(unsigned short level) { return (uint8_t)level; }
#else	
//	CAnalog8IOControlSmooth<SPINDLE_ENABLE_PIN, 2> _spindle;
//	inline uint8_t ConvertSpindleSpeedToIO(unsigned short level) { return CControl::ConvertSpindleSpeedToIO8(CConfigEeprom::GetConfigU16(offsetof(CConfigEeprom::SCNCEeprom, maxspindlespeed)), level); }
	CAnalog8IOControlDirSmooth<SPINDLE_ENABLE_PIN, SPINDLE_DIR_PIN, 2> _spindle;
	inline uint8_t ConvertSpindleSpeedToIO(unsigned short level) { return CControl::ConvertSpindleSpeedToIO8(CConfigEeprom::GetConfigU16(offsetof(CConfigEeprom::SCNCEeprom, maxspindlespeed)), level); }
#undef SPINDLE_DIR_PIN
#endif
#else
	COnOffIOControl<SPINDLE_ENABLE_PIN, SPINDLE_DIGITAL_ON, SPINDLE_DIGITAL_OFF> _spindle;
	inline uint8_t ConvertSpindleSpeedToIO(unsigned short level) { return (uint8_t)level; }
#endif
#ifdef SPINDLE_DIR_PIN
	COnOffIOControl<SPINDLE_DIR_PIN, SPINDLE_DIR_CLW, SPINDLE_DIR_CCLW> _spindleDir;
#else
	CDummyIOControl _spindleDir;
#endif
#else
	CDummyIOControl _spindle;
	CDummyIOControl _spindleDir;
	inline uint8_t ConvertSpindleSpeedToIO(unsigned short level) { return (uint8_t)level; }
#endif  

#ifdef COOLANT_PIN
	COnOffIOControl<COOLANT_PIN, COOLANT_ON, COOLANT_OFF> _coolant;
#else
	CDummyIOControl _coolant;
#endif
#ifdef PROBE_PIN
	CReadPinIOControl<PROBE_PIN, PROBE_ON> _probe;
#else
	CDummyIOControl _probe;
#endif

#ifdef KILL_PIN
	CReadPinIOControl<KILL_PIN, KILL_PIN_ON> _kill;
#else
	CDummyIOControl _kill;
#endif

#if defined(HOLD_PIN) && defined(RESUME_PIN)
	CPushButtonLow<HOLD_PIN, LOW> _hold;
	CPushButtonLow<RESUME_PIN, LOW> _resume;
#else
	CDummyIOControl _hold;
	CDummyIOControl _resume;
#endif

#if defined(HOLDRESUME_PIN)
	CPushButtonLow<HOLDRESUME_PIN, LOW> _holdresume;
#else
	CDummyIOControl _holdresume;
#endif

#ifdef CONTROLLERFAN_FAN_PIN
#ifdef CONTROLLERFAN_ANALOGSPEED
#if defined(USE_RAMPSFD)
	CAnalog8InvertIOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
#else
	CAnalog8IOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
#endif
#else
	COnOffIOControl<CONTROLLERFAN_FAN_PIN, CONTROLLERFAN_DIGITAL_ON, CONTROLLERFAN_DIGITAL_OFF> _controllerfan;
#endif
	inline bool IsControllerFanTimeout() { return millis() - CStepper::GetInstance()->IdleTime() > CONTROLLERFAN_ONTIME; }
#else
	CDummyIOControl _controllerfan;
	inline bool IsControllerFanTimeout() { return false; }
#endif

	inline void Init()
	{
		_controllerfan.Init(255);

		_spindle.Init();
		_probe.Init();
		_kill.Init();
		_coolant.Init();

		_hold.Init();
		_resume.Init();
		_holdresume.Init();
	}

	inline bool IOControl(uint8_t tool, unsigned short level)
	{
		switch (tool)
		{
			case CControl::SpindleCW:		_spindle.On(ConvertSpindleSpeedToIO(level),true);return true;
			case CControl::SpindleCCW:		_spindle.On(ConvertSpindleSpeedToIO(level),false); return true;
				/*
			case CControl::SpindleCW:
			case CControl::SpindleCCW:		_spindle.On(ConvertSpindleSpeedToIO(level)); _spindleDir.Set(tool == CControl::SpindleCCW);	return true;
*/
			case CControl::Coolant:			_coolant.Set(level > 0); return true;
			case CControl::ControllerFan:	_controllerfan.SetLevel((uint8_t)level); return true;
		}
		return false;
	}

	inline void Kill()
	{
		_spindle.Off();
		_coolant.Set(false);
	}

	inline bool IsKill()
	{
		if (_kill.IsOn())
		{
			return true;
		}
		return false;
	}

	inline void TimerInterrupt()
	{
		_hold.Check();
		_resume.Check();
		_holdresume.Check();
		_spindle.Poll();
	}

	inline void Initialized()
	{
		_controllerfan.SetLevel(128);
	}

	inline void OnEvent(EnumAsByte(CControl::EStepperControlEvent) eventtype, uintptr_t /* addinfo */)
	{
		switch (eventtype)
		{
			case CControl::OnStartEvent:
				_controllerfan.On();
				break;
			case CControl::OnIdleEvent:
				if (IsControllerFanTimeout())
				{
					_controllerfan.Off();
				}
				break;
		}
	}
};

////////////////////////////////////////////////////////

constexpr uint16_t GetInfo1a()
{
	return
#ifdef SPINDLE_ENABLE_PIN
		CConfigEeprom::HAVE_SPINDLE |
#ifdef SPINDLE_ANALOGSPEED
		CConfigEeprom::HAVE_SPINDLE_ANALOG |
#endif
#ifdef SPINDLE_DIR_PIN
		CConfigEeprom::HAVE_SPINDLE_DIR |
#endif
#endif
#ifdef COOLANT_PIN
		CConfigEeprom::HAVE_COOLANT |
#endif
#ifdef PROBE_PIN
		CConfigEeprom::HAVE_PROBE |
#endif
#ifdef KILL_PIN
		CConfigEeprom::HAVE_KILL |
#endif
#ifdef HOLD_PIN
		CConfigEeprom::HAVE_HOLD |
#endif
#ifdef RESUME_PIN
		CConfigEeprom::HAVE_RESUME |
#endif
#ifdef HOLDRESUME_PIN
		CConfigEeprom::HAVE_HOLDRESUME |
#endif
#ifndef REDUCED_SIZE
		CConfigEeprom::CAN_ROTATE |
#endif
#if defined(__AVR_ARCH__)
		CConfigEeprom::HAVE_EEPROM |
#endif
#ifdef MYUSE_LCD
		CConfigEeprom::HAVE_SD |
#endif
		CConfigEeprom::NONE;
}
