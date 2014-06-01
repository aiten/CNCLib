#pragma once

////////////////////////////////////////////////////////

#include <u8glib.h>
#include "Settings.h"
#include "Control.h"
#include "CommandBase.h"
#include "MyCommand.h"
#include "MyStepper.h"

////////////////////////////////////////////////////////

#ifdef _MSC_VER
extern class CStepper& Stepper;
#else
extern class CMyStepper Stepper;
#endif

extern struct SSettings Settings;
extern class CControl Control;
extern class CMyCommand Command;

extern void drawloop();

////////////////////////////////////////////////////////


