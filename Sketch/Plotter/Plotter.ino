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
CMyLcd Lcd;

void setup()
{  
  StepperSerial.begin(115200);
  StepperSerial.println(F("Plotter(HA) is starting ... ("__DATE__", "__TIME__")"));

  Lcd.Init();
  Stepper.Init();

  Stepper.SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max, CHPGLParser::_state.penUp.acc , CHPGLParser::_state.penUp.dec);

  Stepper.SetLimitMax(0, 55600);  // 6950*8
  Stepper.SetLimitMax(1, 32000);  // 4000*8
  Stepper.SetLimitMax(2, 8000);   // 100*8

  Stepper.SetJerkSpeed(0, 4000);  // 500 * 8?
  Stepper.SetJerkSpeed(1, 4000);
  Stepper.SetJerkSpeed(2, 4000);
}

void loop()
{
  Control.Run();
}
/*
void GoToReference(axis_t axis)
{
        sdist_t dist = Stepper.GetLimitMax(axis) - Stepper.GetLimitMin(axis);
        // goto min
          Stepper.MoveReference(axis, -dist, 2000, 10000, Stepper.ToReferenceId(axis, true), Stepper.GetDefaultVmax() / 4);
}

////////////////////////////////////////////////////////////

void GoToReference()
{
	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}
*/
