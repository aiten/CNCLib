#include <StepperLib.h>
#include <Steppers/StepperRampsFD_pins.h>
#include <Steppers/StepperRampsFD.h>

#if !defined(__SAM3X8E__)
#error Only Works with Arduino:due
#endif

//////////////////////////////////////////////////////////////////////////

CStepperRampsFD Stepper;

//////////////////////////////////////////////////////////////////////////

void Test1()
{
	for (register unsigned char i = 0;i< NUM_AXIS;i++)
	{
		Stepper.CStepper::MoveRel(i, 3000, 5000);
		Stepper.CStepper::MoveRel(i, 8000, 10000);
		Stepper.CStepper::MoveRel(i, 15000, 15000);
		Stepper.CStepper::MoveRel(i, 35000, 25000);
		Stepper.CStepper::MoveRel(i, 3000, 2500);
		Stepper.CStepper::MoveRel(i, 5500, 10000);
		Stepper.WaitBusy();  

		Stepper.CStepper::MoveAbs(i, 0, 25000);
		Stepper.WaitBusy();  
	}
}

//////////////////////////////////////////////////////////////////////////

void setup()
{
	Serial.begin(115200);
	StepperSerial.println(F("StepperTestRamps14 is starting ... (" __DATE__ ", " __TIME__ ")"));

	Stepper.Init();
	pinMode(13, OUTPUT);

	Stepper.SetDefaultMaxSpeed(15000, 500 , 600);

        for (register unsigned char i=0;i<NUM_AXIS;i++)
        {
    	    Stepper.SetLimitMax(i, 70000);
        }

	for (register unsigned char i=0;i<NUM_AXIS*2;i++)
	{
		Stepper.UseReference(i,false);  
	}

	Stepper.SetJerkSpeed(0, 400);
	Stepper.SetJerkSpeed(1, 400);
	Stepper.SetJerkSpeed(2, 400);
}

//////////////////////////////////////////////////////////////////////////

void loop()
{
	Test1();
}
