#include "KeyPad16.h"

CKeyPad16 keypad;

void setup()
{
  Serial.begin(115200);

  keypad.Init();
}

void loop()
{
  unsigned int keys = keypad.GetAllKeys();
  static unsigned int oldkeys = 0;

  if (keys != oldkeys)
  {
    oldkeys = keys;

    bool have = false;
    for (unsigned char key = 0 ; key < 16; key++)
    {
      if ((keys % 2) == 1)
      {
        if (have)
        {
          Serial.print(F("-"));
        }
        else
        {
          have = true;
        }
        Serial.print((unsigned char )key + 1);
      }
      keys /= 2;
    }
    if (have)
      Serial.println();
    else
      Serial.println(F("no"));
  }
}
