#pragma once

////////////////////////////////////////////////////////

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

class CCoolantControl
{
public:

	void Init()
	{
		On(0);
		pinMode(COOLANT_PIN, OUTPUT);
	}

	void On(unsigned short level)
	{
		if (level)
			digitalWrite(COOLANT_PIN, COOLANT_ON);
		else
			digitalWrite(COOLANT_PIN, COOLANT_OFF);
	}

	bool IsOn()
	{
		return digitalRead(COOLANT_PIN)==COOLANT_ON;
	}

};

////////////////////////////////////////////////////////
