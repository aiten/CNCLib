////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "Beep.h"

////////////////////////////////////////////////////////

#ifndef _MSC_VER
#pragma GCC diagnostic ignored "-Wmissing-field-initializers"
#endif

const SPlayTone SPlayTone::PlayOK[] PROGMEM =
{
	{ ToneA6, MilliSecToDuration(100) },
	{ ToneEnd }
};

const SPlayTone SPlayTone::PlayError[] PROGMEM
{
	{ ToneA6, MilliSecToDuration(200) },
	{ ToneA7, MilliSecToDuration(200) },
	{ ToneA6, MilliSecToDuration(200) },
	{ ToneA7, MilliSecToDuration(200) },
	{ ToneEnd }
};

/*
const SPlayTone SPlayTone::PlayInfo[] PROGMEM
{
	{ ToneA5, MilliSecToDuration(200) },
	{ ToneA6, MilliSecToDuration(200) },
	{ ToneA7, MilliSecToDuration(200) },
	{ ToneEnd }
};
*/
// Fuer Elise: Beethoven

#define Duration0	4000
#define Duration1	MilliSecToDuration(Duration0/1)
#define Duration2	MilliSecToDuration(Duration0/2)
#define Duration4	MilliSecToDuration(Duration0/4)
#define Duration8	MilliSecToDuration(Duration0/8)
#define Duration16	MilliSecToDuration(Duration0/16)

const SPlayTone SPlayTone::PlayInfo[] PROGMEM
{
	{ ToneE5,	Duration16	},
	{ ToneDx5 , Duration16	},

	{ ToneE5 ,	Duration16	},
	{ ToneDx5 , Duration16	},
	{ ToneE5 ,	Duration16	},
	{ ToneB4 ,	Duration16	},
	{ ToneD5 ,	Duration16	},
	{ ToneC5 ,	Duration16	},
	
	{ ToneA4,	Duration8	},
	{ ToneNo,	Duration16	},
	{ ToneC4,	Duration16	},
	{ ToneE4,	Duration16	},
	{ ToneA4,	Duration16	},

	{ ToneB4,	Duration8	},
	{ ToneNo,	Duration16	},
	{ ToneE4,	Duration16	},
	{ ToneGx4,	Duration16	},
	{ ToneB4,	Duration16	},

	{ ToneC5,	Duration8	},
	{ ToneNo,	Duration16	},
	{ ToneE4,	Duration16	},
	{ ToneE5,	Duration16	},
	{ ToneDx5 , Duration16	},

	{ ToneE5 ,	Duration16	},
	{ ToneDx5 , Duration16	},
	{ ToneE5 ,	Duration16	},
	{ ToneB4 ,	Duration16	},
	{ ToneD5 ,	Duration16	},
	{ ToneC5 ,	Duration16	},

	{ ToneA4,	Duration8	},
	{ ToneNo,	Duration16	},
	{ ToneC4,	Duration16	},
	{ ToneE4,	Duration16	},
	{ ToneA4,	Duration16	},

	{ ToneB4,	Duration8	},
	{ ToneNo,	Duration16	},
	{ ToneE4,	Duration16	},
	{ ToneC5,	Duration16	},
	{ ToneB4,	Duration16	},

	{ ToneA4,	Duration4	},

	{ ToneEnd }
};
