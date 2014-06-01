#include <StepperSystem.h>

#if !defined(__AVR_ATmega2560__)
#error Only Works with Arduino:mega2560
#endif

//////////////////////////////////////////////////////////////////////////

CStepperRamps14 Stepper;

//////////////////////////////////////////////////////////////////////////

#include "U8glib.h"

#define LCD_PINS_RS 16 
#define LCD_PINS_ENABLE 17
#define LCD_PINS_D4 23
#define LCD_PINS_D5 25 
#define LCD_PINS_D6 27
#define LCD_PINS_D7 29

#define ST7920_CLK_PIN  LCD_PINS_D4
#define ST7920_DAT_PIN  LCD_PINS_ENABLE
#define ST7920_CS_PIN   LCD_PINS_RS

U8GLIB_ST7920_128X64_1X u8g(ST7920_CLK_PIN, ST7920_DAT_PIN, ST7920_CS_PIN);	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17

//////////////////////////////////////////////////////////////////////////

static void DrawLine(unsigned char line, unsigned long pos) 
{
	// graphic commands to redraw the complete screen should be placed here  
	u8g.setFont(u8g_font_unifont);
	//u8g.setFont(u8g_font_osb21);
	char tmp[16];
	ltoa(pos,tmp,10);

	u8g.drawStr( 0, (line+1)*(64/5), tmp);
}

static void DrawAll()
{
	u8g.firstPage();  
	do 
	{
		DrawLine(0,Stepper.GetCurrentPosition(0));
		DrawLine(1,Stepper.GetCurrentPosition(1)); 
		DrawLine(2,Stepper.GetCurrentPosition(2)); 
		DrawLine(3,Stepper.GetCurrentPosition(3)); 
		DrawLine(4,Stepper.GetCurrentPosition(4)); 
	} 
	while( u8g.nextPage() );
}

//////////////////////////////////////////////////////////////////////////

static void WaitBusy()
{
	while (true && Stepper.IsBusy())
	{
		DrawAll();
	}
	Stepper.WaitBusy();
	DrawAll();

	delay(1000);
}

//////////////////////////////////////////////////////////////////////////

static void Test1()
{
	for (register unsigned char i = 0;i< NUM_AXIS;i++)
	{
		long count = 0;
		Stepper.CStepper::MoveRel(i, 3000, 5000); count += 3000;
		Stepper.CStepper::MoveRel(i, 8000, 10000); count += 8000;
		Stepper.CStepper::MoveRel(i, 15000, 15000); count += 15000;
		Stepper.CStepper::MoveRel(i, 35000, 25000); count += 35000;
		Stepper.CStepper::MoveRel(i, 3000, 2500); count += 3000;
		Stepper.CStepper::MoveRel(i, 5500, 10000); count += 5500;
		WaitBusy();  

		Stepper.CStepper::MoveAbs(i, 0, 25000);
		WaitBusy();  
	}
}

//////////////////////////////////////////////////////////////////////////

void setup()
{
	StepperSerial.begin(115200);
	StepperSerial.println(F("StepperTestRamps14 is starting ... ("__DATE__", "__TIME__")"));

	Stepper.Init();
	pinMode(13, OUTPUT);

	Stepper.SetDefaultMaxSpeed(15000, 500 , 600);
	Stepper.SetLimitMax(0, 70000);
	Stepper.SetLimitMax(1, 70000);
	Stepper.SetLimitMax(2, 70000);
	Stepper.SetLimitMax(3, 70000);
	Stepper.SetLimitMax(4, 70000);

	for (register unsigned char i=0;i<NUM_AXIS*2;i++)
	{
		Stepper.UseReference(i,false);  
	}


	// flip screen, if required
	// u8g.setRot180();

	// set SPI backup if required
	//u8g.setHardwareBackup(u8g_backup_avr_spi);

	// assign default color value
	if ( u8g.getMode() == U8G_MODE_R3G3B2 ) 
	{
		u8g.setColorIndex(255);     // white
	}
	else if ( u8g.getMode() == U8G_MODE_GRAY2BIT ) 
	{
		u8g.setColorIndex(3);         // max intensity
	}
	else if ( u8g.getMode() == U8G_MODE_BW ) 
	{
		u8g.setColorIndex(1);         // pixel on
	}
	else if ( u8g.getMode() == U8G_MODE_HICOLOR ) 
	{
		u8g.setHiColorByRGB(255,255,255);
	}

	Stepper.SetWaitFinishMove(false);

	Stepper.SetJerkSpeed(0, 400);
	Stepper.SetJerkSpeed(1, 400);
	Stepper.SetJerkSpeed(2, 400);
}

//////////////////////////////////////////////////////////////////////////

void loop()
{
	Test1();
}
