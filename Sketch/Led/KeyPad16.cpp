#include <Arduino.h>
#include "KeyPad16.h"


void CKeyPad16::Init()
{
  pinMode(PAD_IN + 0, INPUT_PULLUP);
  pinMode(PAD_IN + 1, INPUT_PULLUP);
  pinMode(PAD_IN + 2, INPUT_PULLUP);
  pinMode(PAD_IN + 3, INPUT_PULLUP);

  pinMode(PAD_OUT + 0, INPUT);
  pinMode(PAD_OUT + 1, INPUT);
  pinMode(PAD_OUT + 2, INPUT);
  pinMode(PAD_OUT + 3, INPUT);
}

bool CKeyPad16::GetKey(unsigned char outpin, unsigned char inpin)
{
  digitalWrite(outpin, LOW);
  pinMode(outpin, OUTPUT);
  unsigned char ret = (digitalRead(inpin) == LOW) ? true : false;
  digitalWrite(outpin, HIGH);
  pinMode(outpin, INPUT);
  return ret;
}

bool CKeyPad16::GetKey(unsigned char key)
{
  unsigned char row = key / 4;
  unsigned char col = 3 - (key % 4);

  return GetKey(PAD_OUT + row, PAD_IN + col);
}

unsigned int CKeyPad16::GetAllKeys()
{
  unsigned int keys = 0;
  for (unsigned char key = 0 ; key < 16; key++)
  {
    if (GetKey(key))
    {
      keys += (1 << key);
    }
  }

  return keys;
}

