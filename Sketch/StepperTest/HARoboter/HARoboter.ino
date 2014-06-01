#include "Settings.h"
#include <HAStepper.h>

#include <HAControl.h>
#include <U8glib.h>

#include "Global.h"
  
////////////////////////////////////////////////////////////
 
  
  CMyCommand Command;
  CMyStepper Stepper;
  CControl Control;
  SSettings Settings;
  
  void setup() 
  {     
    StepperSerial.begin(115200);        
    StepperSerial.println(F("MyRobot(HA) is starting ... ("__DATE__", "__TIME__")"));
  
    // only drive stepper  
    Stepper.Init();
    pinMode(13, OUTPUT);     
  
    Stepper.SetWaitFinishMove(false);
    Stepper.SetDefaultMaxSpeed(10000, 400 , 400);
    Stepper.SetLimitMax(0,1500000);
    Stepper.SetLimitMax(1,1000000);
    Stepper.SetLimitMax(2,100000);
    Stepper.SetLimitMax(3,1000000);
    Stepper.SetLimitMax(4,1000000);
  
     Stepper.SetJerkSpeed(0,500);
     Stepper.SetJerkSpeed(1,500);
     Stepper.SetJerkSpeed(2,500);
     Stepper.SetJerkSpeed(3,500);
     Stepper.SetJerkSpeed(4,500);

     Stepper.SetMaxSpeed(2,5000);
     
     for (register unsigned char i=0;i<NUM_AXIS*2;i++)
     {
       Stepper.UseReference(i,false);  
     }
  }
  
  void loop() 
  {
    Control.Run();
  }

