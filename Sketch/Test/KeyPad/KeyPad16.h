#pragma once

#define PAD_START 22
#define PAD_OUT  PAD_START+4
#define PAD_IN   PAD_START

class CKeyPad16
{
private:

  bool GetKey(unsigned char outpin, unsigned char inpin);

public:

  // CKeyPad16();

  void Init();

  bool GetKey(unsigned char key);
  unsigned int GetAllKeys();

};

