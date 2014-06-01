#ifndef Control_h
#define Control_h

////////////////////////////////////////////////////////

#include <Stepper.h>
#include "MyCommand.h"

class CControl
{
public: 

  CControl();
  void Run();

  void Init(bool refmove);

protected:

  bool Command(char* xbuffer);
  void Idle(unsigned int idletime);

  CMyCommand  _myCommand; 

  char _buffer[SERIALBUFFERSIZE];
  int _bufferidx;

  unsigned long _time;
  unsigned long _lasttime;
  unsigned long _lasttimeBlink;
  unsigned long _brightness;
};

////////////////////////////////////////////////////////

#endif

