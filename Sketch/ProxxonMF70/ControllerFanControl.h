#pragma once

////////////////////////////////////////////////////////

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

class CControllerFanControl
{
public:

	unsigned char Level;    // use like a property

	void Init()
	{
		On(255);
		Level=255;
	}

	void On()
	{
		On(Level);
	}

	void Off()
	{
		On(0);
	}

private:

	void On(unsigned char level)
	{
		analogWrite(CONTROLLERFAN_FAN_PIN, level);
	}

};

////////////////////////////////////////////////////////
