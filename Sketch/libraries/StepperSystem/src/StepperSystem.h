#include "StepperSMC800.h"
#include "StepperRamps14.h"
#include "StepperRampsFD.h"
#include "Utilities.h"
#include "MotionControl.h"

#include "Parser.h"
#include "Control.h"


#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

#define CStepperRamps CStepperRamps14

#elif defined(__SAM3X8E__)

#define CStepperRamps CStepperRampsFD

#endif
