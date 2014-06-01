
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Settings.h"
#include <HAStepper.h>

#include "Global.h"

////////////////////////////////////////////////////////////

#if !defined(_MSC_VER)

#include <U8glib.h>

#define LCD_PINS_RS 16 
#define LCD_PINS_ENABLE 17
#define LCD_PINS_D4 23
#define LCD_PINS_D5 25 
#define LCD_PINS_D6 27
#define LCD_PINS_D7 29

#define ST7920_CLK_PIN  LCD_PINS_D4
#define ST7920_DAT_PIN  LCD_PINS_ENABLE
#define ST7920_CS_PIN   LCD_PINS_RS
;
U8GLIB_ST7920_128X64_1X u8g(ST7920_CLK_PIN, ST7920_DAT_PIN, ST7920_CS_PIN);	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17

void drawpos(unsigned char line, unsigned long pos) 
{
  // graphic commands to redraw the complete screen should be placed here  
  u8g.setFont(u8g_font_unifont);
  //u8g.setFont(u8g_font_osb21);
  char tmp[16];
  ltoa(pos,tmp,10);

  u8g.drawStr( 0, (line+1)*(64/5), tmp);
}
void drawES() 
{
  unsigned line=0;
  u8g.setFont(u8g_font_unifont);
  //u8g.setFont(u8g_font_osb21);
  char tmp[16];
  for (register unsigned char i=0;i<RAMPS14_ENDSTOPCOUNT;i++) 
    tmp[i] = Stepper.IsReference(i) ? '1' : '0';
  tmp[RAMPS14_ENDSTOPCOUNT] = 0;
  u8g.drawStr( 64+16, (line+1)*(64/5), tmp);
}

void drawloop()
{
  u8g.firstPage();  
  do {
    drawpos(0,Stepper.GetCurrentPosition(0));
    drawpos(1,Stepper.GetCurrentPosition(1)); 
    drawpos(2,Stepper.GetCurrentPosition(2)); 
    drawpos(3,Stepper.GetCurrentPosition(3)); 
    drawpos(4,Stepper.GetCurrentPosition(4)); 
    drawES();
  } while( u8g.nextPage() );
}

#endif

////////////////////////////////////////////////////////////

CControl::CControl()
{
  _bufferidx = 0;
  _brightness = 0;
}

////////////////////////////////////////////////////////////

void CControl::Init(bool refmove)
{
  _bufferidx = 0;
  _brightness = 0;

  Stepper.SetWaitFinishMove(false);

  if (refmove)
  {
    int dist2 = Stepper.GetLimitMax(2)-Stepper.GetLimitMin(2);
    int dist0 = Stepper.GetLimitMax(0)-Stepper.GetLimitMin(0);
    int dist1 = Stepper.GetLimitMax(1)-Stepper.GetLimitMin(1);
//    Stepper.MoveReference(2,-min(dist2,10000),10);
//xStepper.MoveReference(0,-min(dist0,10000),10);
//    Stepper.MoveReference(1,-min(dist1,10000),10);
  }
  else
  {
    Stepper.CStepper::MoveAbs(0,0,0);
  }    
}

////////////////////////////////////////////////////////////

void CControl::Idle(unsigned int idletime)
{
//  Plotter.Idle(idletime);
  if (_lasttimeBlink+1000 < _time)
  {
    digitalWrite(13,digitalRead(13)==HIGH ? LOW : HIGH);
    _lasttimeBlink = _time;
  }
}

////////////////////////////////////////////////////////////

bool CControl::Command(char* buffer)
{
  return _myCommand.Command(buffer); // || _hpgl.Command(buffer);
}

////////////////////////////////////////////////////////////

void CControl::Run()
{
  _time=_lasttime=_lasttimeBlink=0;
  Init(true);
  StepperSerial.println(F("done"));

  while (1)
  {
    if (StepperSerial.available() > 0) 
    {
      int idx=_bufferidx++;
      _buffer[idx] = StepperSerial.read();  

      if (_buffer[idx]=='\n' || _buffer[idx]=='\r' || _buffer[idx]==';')
      {
        _buffer[idx]=0;
        if (Command(_buffer))
        {
        }
        else
        {
          StepperSerial.print(F("Command: "));
          StepperSerial.println(_buffer);
        }
        StepperSerial.println(F("ok"));
        
        _bufferidx=0;
      }
      else if (_bufferidx>sizeof(_buffer))
      {
        StepperSerial.println(F("Flush Buffer"));
        _bufferidx=0;
      }
      _lasttime = millis();
    }

    _time = millis();

    if (_lasttime + 100 < _time)
    {
drawloop();
      Idle(_time - _lasttime);
    }

    _brightness++;
    //    analogWrite(13,brightness>>8);
  }
}

