#pragma once

////////////////////////////////////////////////////////

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

class CSpindelControl
{
public:

	void Init()
	{
		On(0);
		pinMode(SPINDEL_PIN, OUTPUT);
	}

	void On(unsigned short level)
	{
		if (level)
			digitalWrite(SPINDEL_PIN, SPINDEL_ON);
		else
			digitalWrite(SPINDEL_PIN, SPINDEL_OFF);
	}
};

////////////////////////////////////////////////////////
