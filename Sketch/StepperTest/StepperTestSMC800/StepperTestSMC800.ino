#include <StepperLib.h>

#if !defined(__AVR_ATmega328P__)
//#error Only Works with Arduino:Duemilanove
#endif

#undef REFMOVE

#define PENUPPOS 30
#define PENDOWNPOS 0

CStepperSMC800 Stepper;

//////////////////////////////////////////////////////////////////////

void setup()
{
  StepperSerial.begin(115200);
  StepperSerial.println(F("StepperTestSMC800 is starting ... ("__DATE__", "__TIME__")"));

  Stepper.Init();
  pinMode(13, OUTPUT);

  pinMode(A0,INPUT_PULLUP);
  pinMode(A1,INPUT_PULLUP);
  pinMode(A2,INPUT_PULLUP);
  pinMode(A3,INPUT_PULLUP);
  pinMode(A4,INPUT_PULLUP);
  pinMode(A5,INPUT_PULLUP);

  pinMode(12,INPUT_PULLUP);
//  pinMode(13,INPUT_PULLUP);

  Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
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
  while (true && Stepper.IsBusy())
  {
    StepperSerial.print(Stepper.GetCurrentPosition(0)); StepperSerial.print(F(":"));
    StepperSerial.print(Stepper.GetCurrentPosition(1)); StepperSerial.print(F(":"));
    StepperSerial.print(Stepper.GetCurrentPosition(2)); StepperSerial.println();
  }
  Stepper.WaitBusy();

  delay(1000);
}

static void MoveRel3(sdist_t dX, sdist_t dY, sdist_t dZ, steprate_t vMax = 0)
{
  Stepper.MoveRelEx(vMax, X_AXIS, dX, Y_AXIS, dY, Z_AXIS, dZ, -1);
}

static void MoveAbs3(udist_t X, udist_t Y, udist_t Z, steprate_t vMax = 0)
{
  Stepper.MoveAbsEx(vMax, X_AXIS, X, Y_AXIS, Y, Z_AXIS, Z, -1);
}

//////////////////////////////////////////////////////////////////////

static void Test1()
{
  int count = 0;
  Stepper.CStepper::MoveRel(0, 300, 1000); count += 300;
  Stepper.CStepper::MoveRel(0, 800, 2000); count += 800;
  Stepper.CStepper::MoveRel(0, 1500, 3000); count += 1500;
  Stepper.CStepper::MoveRel(0, 3500, 5000); count += 3500;
  Stepper.CStepper::MoveRel(0, 300, 500); count += 300;
  Stepper.CStepper::MoveRel(0, 550, 2000); count += 550;
  WaitBusy();

  int count1 = 0;
  Stepper.CStepper::MoveRel(1, 300, 1000); count1 += 300;
  Stepper.CStepper::MoveRel(1, 800, 2000); count1 += 800;
  Stepper.CStepper::MoveRel(1, 800, 3000); count1 += 800;
  Stepper.CStepper::MoveRel(1, 1400, 5000); count1 += 1400;
  Stepper.CStepper::MoveRel(1, 300, 500); count1 += 300;
  Stepper.CStepper::MoveRel(1, 400, 2000); count1 += 400;
  WaitBusy();

  int count2 = 0;
  Stepper.CStepper::MoveRel(2, 300, 1000); count2 += 300;
  Stepper.CStepper::MoveRel(2, 800, 2000); count2 += 800;
  Stepper.CStepper::MoveRel(2, 800, 3000); count2 += 800;
  Stepper.CStepper::MoveRel(2, 1400, 5000); count2 += 1400;
  Stepper.CStepper::MoveRel(2, 300, 500); count2 += 300;
  Stepper.CStepper::MoveRel(2, 400, 2000); count2 += 400;
  WaitBusy();

  Stepper.CStepper::MoveRel(0, -count, 5000);
  Stepper.CStepper::MoveRel(1, -count1, 5000);
  Stepper.CStepper::MoveRel(2, -count2, 5000);
  WaitBusy();
}

//////////////////////////////////////////////////////////////////////

static bool _isPenDown = true;
static void PenUp()
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

static void PenDown()
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

static void Test3()
{

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

static void Test4()
{
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
