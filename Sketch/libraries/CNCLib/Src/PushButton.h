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

#pragma once

////////////////////////////////////////////////////////

class CPushButton
{
public:

	CPushButton()
	{
	}

	CPushButton(pin_t pin, uint8_t onValue)
	{
		SetPin(pin, onValue);
	}

	void SetPin(pin_t pin, uint8_t onValue)		
	{ 
		_pin=pin; 
		_onvalue = onValue; 
		CHAL::pinModeInputPullUp(_pin);
	}

	void Check()
	{
		bool isOn = CHAL::digitalRead(_pin) == _onvalue;
		switch (_state)
		{
			case ButtonOff:
				if (isOn)
				{
					_state = ButtonPressed;
				}
				break;

			case ButtonPressed:
				if (!isOn)
				{
					_state = ButtonPressedOff;
				}
				break;

			case ButtonPressedOff:
				break;

			case ExpectButtonOff:
				if (!isOn)
				{
					_state = ButtonOff;
				}
				break;
		}
	}

	bool IsPressed()
	{
		// current state!!!!!
		// use IsOn() 
		return CHAL::digitalRead(_pin) == _onvalue;
	}

	bool IsOn()
	{
		// check and set state 
		// button must be released and pressed to get "true" again.

		Check();

		if (_state == ButtonPressed)
		{
			_state = ExpectButtonOff;
			return true;
		}
		else if (_state == ButtonPressedOff)
		{
			_state = ButtonOff;
			return true;
		}
		return false;
	}

protected:

	pin_t	_pin = 0;
	uint8_t	_onvalue = 0;

	enum EButtonState
	{
		ButtonOff = 0,			// button not pressed, not waiting
		ButtonPressed,			// report pressed if IsOn is called, Button still pressed, wait for Button Off
		ButtonPressedOff,		// Pressed an released but not reported
		ExpectButtonOff			// reported on IsOn, wait for "Off"
	};

	EnumAsByte(EButtonState) _state = ButtonOff;

};

////////////////////////////////////////////////////////
