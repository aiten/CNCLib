#include "functionptr.h"

void setup() 
{
  Serial.begin(115200);
}

void loop() 
{
  CFunctionPtr::Test();
  delay(500);
}


