  #pragma once

////////////////////////////////////////////////////////

#include "CommandBase.h"

////////////////////////////////////////////////////////

class CMyCommand : public CCommandBase
{
public: 

  CMyCommand();

  virtual bool Command(char* xbuffer);

protected:

  void GoToReference(axis_t axis, sdist_t maxdist, sdist_t distToRef, sdist_t distIfRefIsOn, unsigned char referenceid, udist_t setpos, steprate_t vmax);
  void GoToReference(axis_t axis);

  static unsigned long DefU32(unsigned long x, unsigned long def)  { return x==0 ? def : x; }
  static long Def32(long x, long def)  { return x == 0 ? def : x; }
  static unsigned short DefU16(long x, long def)  { return (unsigned short) (x == 0 ? def : x); }
  static short Def16(long x, long def)  { return short(x == 0 ? def : x); }

  void SetSpeed(SSettings::SSPEED& speed);
};

////////////////////////////////////////////////////////




