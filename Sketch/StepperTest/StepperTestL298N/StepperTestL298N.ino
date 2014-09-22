#include <StepperLib.h>

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:__AVR_ATmega328P__
#endif

CStepperL298N Stepper;

//////////////////////////////////////////////////////////////////////

void setup()
{
  StepperSerial.begin(115200);
  StepperSerial.println(F("StepperTestL298N is starting ... ("__DATE__", "__TIME__")"));

 // Stepper.SetEnablePin(X_AXIS,10,11);
 // Stepper.SetEnablePin(Y_AXIS,12,13);
  Stepper.Init();
  pinMode(13, OUTPUT);

//  Stepper.SetStepMode(0,CStepper::FullStep);
  Stepper.SetStepMode(0,CStepper::HalfStep);
  Stepper.SetStepMode(1,CStepper::HalfStep);

  Stepper.SetEnableTimeout(0, 1);
  Stepper.SetEnableTimeout(1, 1);

  Stepper.SetUsual(410);

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
  
  sdist_t count[NUM_AXIS] = { 0 };

  int divspeed=12;
  int divdist=6;
  int dist=0;

  Stepper.CStepper::MoveRel(0, dist=300/divdist, 1000/divspeed); count[0] -= dist;
  Stepper.CStepper::MoveRel(0, dist=800/divdist, 2000/divspeed); count[0] -= dist;
  Stepper.CStepper::MoveRel(0, dist=1500/divdist, 3000/divspeed); count[0] -= dist;
  Stepper.CStepper::MoveRel(0, dist=3500/divdist, 5000/divspeed); count[0] -= dist;
  Stepper.CStepper::MoveRel(0, dist=300/divdist, 500/divspeed); count[0] -= dist;
  Stepper.CStepper::MoveRel(0, dist=550/divdist, 2000/divspeed); count[0] -= dist;

  WaitBusy();

  divspeed=12;
  divdist=6;
  Stepper.CStepper::MoveRel(1, dist=300/divdist, 1000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=800/divdist, 2000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=1500/divdist, 3000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=3500/divdist, 5000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=300/divdist, 500/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=550/divdist, 2000/divspeed); count[1] -= dist;
  WaitBusy();

  Stepper.MoveRel(count);
  WaitBusy();
}

//////////////////////////////////////////////////////////////////////

static void Test2()
{
  Serial.println(F("Test 2"));

  sdist_t count[NUM_AXIS] = { 500,500 };
  udist_t c0[NUM_AXIS] = { 0 };
  Stepper.MoveRel(count);
  Stepper.MoveAbs(c0);
}
//////////////////////////////////////////////////////////////////////

void loop()
{
  Test2();
  Test1();
 }
