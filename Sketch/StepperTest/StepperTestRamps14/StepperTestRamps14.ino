#include <StepperLib.h>
#include <Steppers/StepperRamps14_pins.h>
#include <Steppers/StepperRamps14.h>

#if !defined(__AVR_ATmega2560__)
#error Only Works with Arduino:mega2560
#endif

//////////////////////////////////////////////////////////////////////////

CStepperRamps14 Stepper;

//////////////////////////////////////////////////////////////////////////

void DrawAll() {};

//////////////////////////////////////////////////////////////////////////

void WaitBusy()
{
	while (true && Stepper.IsBusy())
	{
		DrawAll();
	}
	Stepper.WaitBusy();
	DrawAll();

	delay(1000);
}

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
		WaitBusy();  

		Stepper.CStepper::MoveAbs(i, 0, 25000);
		WaitBusy();  
	}
}

//////////////////////////////////////////////////////////////////////////

void setup()
{
	StepperSerial.begin(115200);
	StepperSerial.println(F("StepperTestRamps14 is starting ... (" __DATE__ ", " __TIME__ ")"));

	Stepper.Init();
	CHAL::pinMode(13, OUTPUT);

	Stepper.SetDefaultMaxSpeed(15000, 500 , 600);
	
        for (register unsigned char i=0;i<NUM_AXIS;i++)
        {
    	    Stepper.SetLimitMax(i, 70000);
        }

	for (register unsigned char i=0;i<NUM_AXIS*2;i++)
	{
		Stepper.UseReference(i,false);  
	}

	Stepper.SetWaitFinishMove(false);

	Stepper.SetJerkSpeed(0, 400);
	Stepper.SetJerkSpeed(1, 400);
	Stepper.SetJerkSpeed(2, 400);
}

//////////////////////////////////////////////////////////////////////////

void loop()
{
	Test1();
}
