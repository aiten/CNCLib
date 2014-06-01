#pragma once

////////////////////////////////////////////////////////////

#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2
//#define E1_AXIS 3
//#define E2_AXIS 4

#define SERIALBUFFERSIZE 135

////////////////////////////////////////////////////////

struct SSettings
{
public: 

  SSettings();

  struct SSPEED
  {
	unsigned int  max;
	unsigned int  acc;
	unsigned int  dec;
  };
};

extern SSettings Settings;

////////////////////////////////////////////////////////


