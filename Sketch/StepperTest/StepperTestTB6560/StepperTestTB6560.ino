#include <StepperLib.h>
#include "StepperTB6560.h"

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:Duemilanove
#endif

CStepperTB6560 Stepper;

int defspeed=25500;  // tested by try and errror

//////////////////////////////////////////////////////////////////////

void setup()
{
  StepperSerial.begin(115200);
  StepperSerial.println(F("StepperTestL298N is starting ... ("__DATE__", "__TIME__")"));

  Stepper.Init();
  pinMode(13, OUTPUT);

//  Stepper.SetStepMode(0,CStepper::FullStep);
  Stepper.SetStepMode(0,CStepper::HalfStep);
  Stepper.SetStepMode(1,CStepper::HalfStep);

  Stepper.SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, 350, 350);

  Stepper.SetLimitMax(0, 695000);
  Stepper.SetLimitMax(1, 40000);
  Stepper.SetLimitMax(2, 40000);

  int dist2 = Stepper.GetLimitMax(2) - Stepper.GetLimitMin(2);
  int dist0 = Stepper.GetLimitMax(0) - Stepper.GetLimitMin(0);
  int dist1 = Stepper.GetLimitMax(1) - Stepper.GetLimitMin(1);

#ifdef REFMOVE

  Stepper.MoveReference(2, -min(dist2, 10000), 10, 100, Stepper.ToReferenceId(2, true), Stepper.GetDefaultVmax() / 4);
  Stepper.MoveReference(0, -min(dist0, 10000), 12, 100, Stepper.ToReferenceId(0, true), Stepper.GetDefaultVmax() / 4);
  Stepper.MoveReference(1, -min(dist1, 10000), 10, 100, Stepper.ToReferenceId(1, true), Stepper.GetDefaultVmax() / 4);

#endif

  Stepper.SetJerkSpeed(X_AXIS, 1000);
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

  delay(1200);
}

//////////////////////////////////////////////////////////////////////

static void Test1()
{
  Serial.println(F("Test 1"));
  
  sdist_t count[NUM_AXIS] = { 0 };

  struct MV
  {
    sdist_t dist;
    steprate_t rate;
  }
  mv[] = 
  {
    {  3000,   10000 },
    {  8000,   20000 },
    { 15000,   30000 },
    { 25000,   50000 },
    {  3000,    5000 },
    {  5500,   20000 },
    { 0 }
  };

  for (register unsigned char i=0;mv[i].dist != 0; i++)
  {
    sdist_t    dist = mv[i].dist;
    steprate_t rate = RoundMulDivUInt(mv[i].rate,defspeed,50000);
    Stepper.CStepper::MoveRel(X_AXIS, dist, rate); count[X_AXIS] -= dist;
  }

  WaitBusy();

  Stepper.MoveRel(count,defspeed);

  Serial.println(Stepper.GetPosition(X_AXIS));

  WaitBusy();
}

//////////////////////////////////////////////////////////////////////

static void Test2()
{
  Serial.println(F("Test 2"));

  sdist_t count[NUM_AXIS] = { 50000 };
  udist_t c0[NUM_AXIS] = { 0 };
 
  Stepper.MoveRel(count,defspeed);
  Stepper.MoveAbs(c0,defspeed);
}
//////////////////////////////////////////////////////////////////////

void loop()
{
  Test1();
//  Test2();
 } 
 
