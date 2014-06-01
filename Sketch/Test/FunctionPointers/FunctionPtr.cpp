
#include <Arduino.h>
#include "functionptr.h"


static void Test1()
{
  Serial.println("hallo1");
}

static void Test2()
{
  Serial.println("hallo2");
}


PROGMEM CFunctionPtr::STest CFunctionPtr::testarr[2] =
{
  { Test1, 1 },  
  { Test2, 2 }  
};


static unsigned char i=0;

void CFunctionPtr::Test() 
{
  i=(i+1)%2;
  Serial.print("pre");
  
  TestFunction fnc = (TestFunction) pgm_read_word_near(&testarr[i].fnc);
  fnc();

 // testarr[i].fnc();
  Serial.print("post");
}
