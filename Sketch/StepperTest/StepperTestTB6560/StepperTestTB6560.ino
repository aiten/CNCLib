#include <StepperLib.h>
#include "StepperTB6560.h"

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:Duemilanove
#endif

CStepperTB6560 Stepper;

int defspeed=20000;

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
  
  sdist_t count[NUM_AXIS] = { 0 };
  count[X_AXIS] = 0;

  long divspeed=20;
  long divdist=10;
  sdist_t dist=0;

  Stepper.CStepper::MoveRel(X_AXIS, dist= (300000l/divdist), 100000l/divspeed); count[X_AXIS] -= dist;
Stepper.CStepper::MoveRel(X_AXIS, -dist, 100000l/divspeed); count[X_AXIS] += dist;


//  Stepper.CStepper::MoveRel(X_AXIS, dist= (30000l/divdist), 100000l/divspeed); count[X_AXIS] -= dist;
//  Stepper.CStepper::MoveRel(X_AXIS, dist= (80000l/divdist), 200000l/divspeed); count[X_AXIS] -= dist;
//  Stepper.CStepper::MoveRel(X_AXIS, dist=(150000l/divdist), 300000l/divspeed); count[X_AXIS] -= dist;
//  Stepper.CStepper::MoveRel(X_AXIS, dist=(350000l/divdist), 500000l/divspeed); count[X_AXIS] -= dist;
//  Stepper.CStepper::MoveRel(X_AXIS, dist= (30000l/divdist),  50000l/divspeed); count[X_AXIS] -= dist;
//  Stepper.CStepper::MoveRel(X_AXIS, dist= (55000l/divdist), 200000l/divspeed); count[X_AXIS] -= dist;

  WaitBusy();
/*
  divspeed=12;
  divdist=6;
  Stepper.CStepper::MoveRel(1, dist=300/divdist, 1000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=800/divdist, 2000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=1500/divdist, 3000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=3500/divdist, 5000/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=300/divdist, 500/divspeed); count[1] -= dist;
  Stepper.CStepper::MoveRel(1, dist=550/divdist, 2000/divspeed); count[1] -= dist;
  WaitBusy();
*/
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
 
