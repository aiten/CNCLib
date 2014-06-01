#pragma once

////////////////////////////////////////////////////////

class CCommandBase
{
public: 

  CCommandBase();
  virtual bool Command(char* xbuffer);

protected:

  void SkipSpaces();
  bool SkipSeperator();

  bool Error();
  bool ErrorExpectedEnd();
  bool ErrorMissingOnInvalidParameter();
  bool IsDelOrEnd();
  bool IsDelimiter(); 
  bool GetInt(long* value);
  bool IsCommand(char* b, bool resumePen);
  bool IsEndCommand();
  bool IsSingleLineCommand(); 
  bool GetIntWithSeperator(long* value);
  void ScanOldParam();

  bool _isInitialized;

  unsigned char* _cnt;
  char* _buffer;
  long _x,_y,_z,_t,_u;
  long _d[NUM_AXIS];
  unsigned long _ud[NUM_AXIS];
};

////////////////////////////////////////////////////////


