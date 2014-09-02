#include <StepperLib.h>

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:__AVR_ATmega328P__
#endif

#undef REFMOVE

#define PENUPPOS 30
#define PENDOWNPOS 0

CStepperL298N Stepper;

//////////////////////////////////////////////////////////////////////

void setup()
{
  StepperSerial.begin(115200);
  StepperSerial.println(F("StepperTestL298N is starting ... ("__DATE__", "__TIME__")"));

  Stepper.Init();
  pinMode(13, OUTPUT);

  Stepper.SetStepMode(0,CStepper::FullStep);
//  Stepper.SetStepMode(0,CStepper::HalfStep);
  Stepper.SetStepMode(1,CStepper::HalfStep);

  Stepper.SetDefaultMaxSpeed(1000, 100 , 150);
  Stepper.SetLimitMax(0, 6950);
  Stepper.SetLimitMax(1, 4000);
  Stepper.SetLimitMax(2, 4000);

  int dist2 = Stepper.GetLimitMax(2) - Stepper.GetLimitMin(2);
  int dist0 = Stepper.GetLimitMax(0) - Stepper.GetLimitMin(0);
  int dist1 = Stepper.GetLimitMax(1) - Stepper.GetLimitMin(1);

#ifdef REFMOVE

  Stepper.MoveReference(2, -min(dist2, 10000), 10, 100, Stepper.ToReferenceId(2, true), Stepper.GetDefaultVmax() / 4);
  Stepper.MoveReference(0, -min(dist0, 10000), 12, 100, Stepper.ToReferenceId(0, true), Stepper.GetDefaultVmax() / 4);
  Stepper.MoveReference(1, -min(dist1, 10000), 10, 100, Stepper.ToReferenceId(1, true), Stepper.GetDefaultVmax() / 4);

#endif

  Stepper.SetJerkSpeed(0, 400);
  Stepper.SetJerkSpeed(1, 400);
  Stepper.SetJerkSpeed(2, 400);
}

//////////////////////////////////////////////////////////////////////

static void WaitBusy()
{
  while (false && Stepper.IsBusy())
  {
    StepperSerial.print(Stepper.GetCurrentPosition(0)); StepperSerial.print(F(":"));
    StepperSerial.print(Stepper.GetCurrentPosition(1)); StepperSerial.print(F(":"));
    StepperSerial.print(Stepper.GetCurrentPosition(2)); StepperSerial.println();
  }
  Stepper.WaitBusy();

  delay(1000);
}

//////////////////////////////////////////////////////////////////////

static void Test1()
{
  Serial.println(F("Test 1"));
  
  int count = 0;
  int count1 = 0;
  int count2 = 0;

  int mydelay=100;

for(register char j=0;j<100;j++)
{
  Stepper.MoveRelEx(1000,X_AXIS,sdist_t(1),Y_AXIS,sdist_t(1),-1);
  delay(mydelay);
}
/*
  Stepper.CStepper::MoveRel(0, 300, 100); count += 300;
  Stepper.CStepper::MoveRel(0, 300, 50); count += 300;
*/  
/*
  Stepper.CStepper::MoveRel(0, 300, 1000); count += 300;
  Stepper.CStepper::MoveRel(0, 800, 2000); count += 800;
  Stepper.CStepper::MoveRel(0, 1500, 3000); count += 1500;
  Stepper.CStepper::MoveRel(0, 3500, 5000); count += 3500;
  Stepper.CStepper::MoveRel(0, 300, 500); count += 300;
  Stepper.CStepper::MoveRel(0, 550, 2000); count += 550;

  WaitBusy();
*/  

/*
  Stepper.CStepper::MoveRel(1, 300, 1000); count1 += 300;
  Stepper.CStepper::MoveRel(1, 800, 2000); count1 += 800;
  Stepper.CStepper::MoveRel(1, 800, 3000); count1 += 800;
  Stepper.CStepper::MoveRel(1, 1400, 5000); count1 += 1400;
  Stepper.CStepper::MoveRel(1, 300, 500); count1 += 300;
  Stepper.CStepper::MoveRel(1, 400, 2000); count1 += 400;
  WaitBusy();

  Stepper.CStepper::MoveRel(2, 300, 1000); count2 += 300;
  Stepper.CStepper::MoveRel(2, 800, 2000); count2 += 800;
  Stepper.CStepper::MoveRel(2, 800, 3000); count2 += 800;
  Stepper.CStepper::MoveRel(2, 1400, 5000); count2 += 1400;
  Stepper.CStepper::MoveRel(2, 300, 500); count2 += 300;
  Stepper.CStepper::MoveRel(2, 400, 2000); count2 += 400;
  WaitBusy();
*/
/*
  Stepper.CStepper::MoveRel(0, -count, 5000);
  Stepper.CStepper::MoveRel(1, -count1, 5000);
  Stepper.CStepper::MoveRel(2, -count2, 5000);
  WaitBusy();
*/
}
//////////////////////////////////////////////////////////////////////

void loop()
{
  Test1();
  delay(500);
 }
