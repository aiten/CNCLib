////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

class CMessage
{
public: 

	CMessage(unsigned int messageNo, const __FlashStringHelper * message)
	{
		_message = message;
		_messageNo = messageNo;
	};

	bool IsMessage()											{ return _messageNo != 0; };
	const __FlashStringHelper * GetMessage()					{ return _message; }
	void ClearMessage()											{ _messageNo = 0;  _message = NULL; }

private:

	const __FlashStringHelper * _message;
	unsigned int				_messageNo;
};

