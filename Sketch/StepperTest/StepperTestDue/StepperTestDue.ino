#include <StepperSystem.h>

#if !defined(__SAM3X8E__)
#error Only Works with Arduino:due
#endif

//////////////////////////////////////////////////////////////////////////

CStepperRampsFD Stepper;

//////////////////////////////////////////////////////////////////////////

static void Test1()
{
	for (register unsigned char i = 0;i< NUM_AXIS;i++)
	{
		long count = 0;
		Stepper.CStepper::MoveRel(i, 3000, 5000); count += 3000;
		Stepper.CStepper::MoveRel(i, 8000, 10000); count += 8000;
		Stepper.CStepper::MoveRel(i, 15000, 15000); count += 15000;
		Stepper.CStepper::MoveRel(i, 35000, 25000); count += 35000;
		Stepper.CStepper::MoveRel(i, 3000, 2500); count += 3000;
		Stepper.CStepper::MoveRel(i, 5500, 10000); count += 5500;
		Stepper.WaitBusy();  

		Stepper.CStepper::MoveAbs(i, 0, 25000);
		Stepper.WaitBusy();  
	}
}

//////////////////////////////////////////////////////////////////////////

void setup()
{
	Serial.begin(115200);
	StepperSerial.println(F("StepperTestRamps14 is starting ... ("__DATE__", "__TIME__")"));

	Stepper.Init();
	pinMode(13, OUTPUT);

	Stepper.SetDefaultMaxSpeed(15000, 500 , 600);
	Stepper.SetLimitMax(0, 70000);
	Stepper.SetLimitMax(1, 70000);
	Stepper.SetLimitMax(2, 70000);
	Stepper.SetLimitMax(3, 70000);
	Stepper.SetLimitMax(4, 70000);

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
