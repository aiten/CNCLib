#include <StepperSystem.h>

#include "MyControl.h"
#include "PlotterControl.h"
#include "MyLcd.h"
#include "HPGLParser.h"

#include <Wire.h>  // Comes with Arduino IDE
#include <LiquidCrystal_I2C.h>

////////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

CStepperRamps14 Stepper;

#elif defined(__SAM3X8E__)

CStepperRampsFD Stepper;

#elif defined(__AVR_ATmega328P__)

CStepperSMC800 Stepper;

#elif defined(_MSC_VER)

#error NOT in MVC

#endif

CMyControl Control;
CPlotter Plotter;

#ifdef __USE_LCD__
CMyLcd Lcd;
#endif

void setup()
{  
  StepperSerial.begin(115200);
}

void loop()
{
  Control.Run();
}

