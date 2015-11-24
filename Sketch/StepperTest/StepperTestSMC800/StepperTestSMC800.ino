#include <StepperLib.h>
#include <Steppers/StepperSMC800.h>
#include "StepperTest.h"

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:Duemilanove
#endif

#undef REFMOVE

#define PENUPPOS 30
#define PENDOWNPOS 0

#define DEFSPEED steprate_t(3000)  // tested by try and errror

#define TESTAXIS 3


CStepperSMC800 Stepper;
//CStepperL298N Stepper;

//////////////////////////////////////////////////////////////////////

void setup()
{
  StepperSerial.begin(115200);
  StepperSerial.println(F("StepperTestSMC800 is starting ... (" __DATE__ ", " __TIME__ ")"));

  Stepper.Init();
  CHAL::pinMode(13, OUTPUT);

  CHAL::pinMode(A0,INPUT_PULLUP);
  CHAL::pinMode(A1,INPUT_PULLUP);
  CHAL::pinMode(A2,INPUT_PULLUP);
  CHAL::pinMode(A3,INPUT_PULLUP);
  CHAL::pinMode(A4,INPUT_PULLUP);
  CHAL::pinMode(A5,INPUT_PULLUP);

  CHAL::pinMode(12,INPUT_PULLUP);
//  pinMode(13,INPUT_PULLUP);

  Stepper.SetUsual(3000);
  
  Stepper.SetLimitMax(0, 100000);
  Stepper.SetLimitMax(1, 100000);
  Stepper.SetLimitMax(2, 100000);

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

void WaitBusy()
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

void MoveRel3(sdist_t dX, sdist_t dY, sdist_t dZ, steprate_t vMax = 0)
{
  Stepper.MoveRelEx(vMax, X_AXIS, dX, Y_AXIS, dY, Z_AXIS, dZ, -1);
}

void MoveAbs3(udist_t X, udist_t Y, udist_t Z, steprate_t vMax = 0)
{
  Stepper.MoveAbsEx(vMax, X_AXIS, X, Y_AXIS, Y, Z_AXIS, Z, -1);
}

//////////////////////////////////////////////////////////////////////

void Test1()
{
  Serial.println(F("Test 1"));
  
  static CStepperTest::SMove mv[] = 
  {
    {  3000,   10000 },
    {  8000,   20000 },
    { 15000,   30000 },
    { 25000,   50000 },
    {  3000,    5000 },
    {  5500,   20000 },
    {  0,      100 },
    {  5500,   20000 },
    {  0,      1 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  0,      1 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  0,      1 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  0,      1 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  0,      1 },
    {  1500,   50000 },
    {  1500,   50000 },
    {  1500,   50000 },
    { 0, 0 }
  };

  if (false)
  {
    Stepper.SetUsual(6000);
    Stepper.SetDefaultMaxSpeed(15000);
    CStepperTest teststepper(mv,steprate_t(15000),60000);
    teststepper.TestAxis(Y_AXIS);
    WaitBusy();
    teststepper.Home();
  }

  if (true)
  {
    CStepperTest teststepper(mv,DEFSPEED,4000);
    for (axis_t axis = 0;axis<TESTAXIS;axis++)
    {
      teststepper.TestAxis(axis);
      WaitBusy();
    }
    teststepper.Home();
  }
}

//////////////////////////////////////////////////////////////////////

bool _isPenDown = true;
void PenUp()
{
  if (_isPenDown)
  {
    Stepper.WaitBusy();
    Stepper.SetDefaultMaxSpeed(1000); Stepper.SetAcc(Z_AXIS, 100); Stepper.SetDec(Z_AXIS, 150);

    _isPenDown = false;
    Stepper.MoveAbs(Z_AXIS, PENDOWNPOS);
    Stepper.WaitBusy();

    Stepper.SetDefaultMaxSpeed(5000);
    Stepper.SetAcc(X_AXIS, 200); Stepper.SetDec(X_AXIS, 250);
    Stepper.SetAcc(Y_AXIS, 200); Stepper.SetDec(Y_AXIS, 250);
  }
}

void PenDown()
{
  if (!_isPenDown)
  {
    Stepper.WaitBusy();
    Stepper.SetDefaultMaxSpeed(250); Stepper.SetAcc(Z_AXIS, 65); Stepper.SetDec(Z_AXIS, 65);

    _isPenDown = true;
    Stepper.MoveAbs(Z_AXIS, PENUPPOS);
    Stepper.WaitBusy();

    Stepper.SetDefaultMaxSpeed(1000);
    Stepper.SetAcc(X_AXIS, 200); Stepper.SetDec(X_AXIS, 250);
    Stepper.SetAcc(Y_AXIS, 200); Stepper.SetDec(Y_AXIS, 250);
  }
}

//////////////////////////////////////////////////////////////////////
// Test HPGL Move

void Test3()
{
  Serial.println(F("Test 3"));

  /*
  PU 2683 5738;
  PD 3047 5831;
  PU 3410 5508;
  PD 3047 5831;
  PU 3762 5046;
  PD 3410 5508;
  PU 4067 4800;
  PD 3762 5046;
  */

#define ToHPLG(a) (a/3)

  if (true)
  {
    PenUp();
    MoveAbs3(ToHPLG(2683), ToHPLG(5738), Stepper.GetPosition(Z_AXIS));
    PenDown();
    MoveAbs3(ToHPLG(3047), ToHPLG(5831), Stepper.GetPosition(Z_AXIS));
    PenUp();
    MoveAbs3(ToHPLG(3410), ToHPLG(5508), Stepper.GetPosition(Z_AXIS));
  }
  else
  {
    PenUp();
    MoveAbs3(ToHPLG(3047), ToHPLG(5831), Stepper.GetPosition(Z_AXIS));
  }

  PenDown();
  MoveAbs3(ToHPLG(3047), ToHPLG(5831), Stepper.GetPosition(Z_AXIS));
  PenUp();
  MoveAbs3(ToHPLG(3762), ToHPLG(5046), Stepper.GetPosition(Z_AXIS));

  if (true)
  {
    StepperSerial.println();
    while (Stepper.IsBusy())
    {
      Stepper.Dump(CStepper::DumpPos | CStepper::DumpMovements);
    }
  }

  PenDown();
  //  Stepper.MoveAbs3(ToHPLG(3410),ToHPLG(5508),Stepper.GetPosition(Z_AXIS));
  // PenUp();
  // Stepper.MoveAbs3(ToHPLG(4067),ToHPLG(4800),Stepper.GetPosition(Z_AXIS));
  // PenDown();
  // Stepper.MoveAbs3(ToHPLG(3762),ToHPLG(5046),Stepper.GetPosition(Z_AXIS));
}

//////////////////////////////////////////////////////////////////////

void Test4()
{
  Serial.println(F("Test 4"));
  
  while(1)
  {
    Serial.print(digitalRead(A0) ? F("1") : F("0"));
    Serial.print(digitalRead(A1) ? F("1") : F("0"));
    Serial.print(digitalRead(A2) ? F("1") : F("0"));
    Serial.print(digitalRead(A3) ? F("1") : F("0"));
    Serial.print(digitalRead(A4) ? F("1") : F("0"));
    Serial.print(digitalRead(A5) ? F("1") : F("0"));
    Serial.print(digitalRead(12) ? F("1") : F("0"));
    Serial.print(digitalRead(13) ? F("1") : F("0"));
    Serial.println();
    delay(100);
   }
}

//////////////////////////////////////////////////////////////////////

void loop()
{
  PenUp();
  MoveAbs3(0, 0, 0);
  Test1();
 }
