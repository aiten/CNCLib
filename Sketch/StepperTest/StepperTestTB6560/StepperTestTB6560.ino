#include <StepperLib.h>
#include <StepperTB6560.h>

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:Duemilanove
#endif

CStepperTB6560 Stepper;

#define DEFSPEED steprate_t(25500)  // tested by try and errror

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

  Stepper.SetUsual(DEFSPEED);

  Stepper.SetLimitMax(0, 100000);
  Stepper.SetLimitMax(1, 100000);
  Stepper.SetLimitMax(2, 100000);
  
  Stepper.SetEnableTimeout(0, 1);
  Stepper.SetEnableTimeout(1, 1);
  Stepper.SetEnableTimeout(2, 1);

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

  delay(1200);
}

//////////////////////////////////////////////////////////////////////

static void Test1()
{
  Serial.println(F("Test 1"));
  
  sdist_t count[NUM_AXIS] = { 0,0,0 };

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

  for (axis_t axis = 0;axis<3;axis++)
  {
    for (register unsigned char i=0;mv[i].dist != 0; i++)
    {
      sdist_t    dist = mv[i].dist;
      steprate_t rate = RoundMulDivUInt(mv[i].rate,DEFSPEED,50000);
      Stepper.CStepper::MoveRel(axis, dist, rate); count[axis] -= dist;
    }
  
    WaitBusy();
  }

  Stepper.MoveRel(count,DEFSPEED);

  Serial.println(Stepper.GetPosition(X_AXIS));
  Serial.println(Stepper.GetPosition(Y_AXIS));
  Serial.println(Stepper.GetPosition(Z_AXIS));

  WaitBusy();
}

//////////////////////////////////////////////////////////////////////

static void Test2()
{
  Serial.println(F("Test 2"));

  sdist_t count[NUM_AXIS] = { 50000 };
  udist_t c0[NUM_AXIS] = { 0 };
 
  Stepper.MoveRel(count,DEFSPEED);
  Stepper.MoveAbs(c0,DEFSPEED);
}
//////////////////////////////////////////////////////////////////////

void loop()
{
  Test1();
//  Test2();
 } 
 
