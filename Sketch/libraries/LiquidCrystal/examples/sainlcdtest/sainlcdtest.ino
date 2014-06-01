/*
** Example Arduino sketch for SainSmart I2C LCD2004 adapter for HD44780 LCD screens
** Readily found on eBay or http://www.sainsmart.com/ 
** The LCD2004 module appears to be identical to one marketed by YwRobot
**
** Address pins 0,1 & 2 are all permenantly tied high so the address is fixed at 0x27
**
** Written for and tested with Arduino 1.0
** This example uses F Malpartida's NewLiquidCrystal library. Obtain from:
** https://bitbucket.org/fmalpartida/new-liquidcrystal 
**
** Edward Comer
** LICENSE: GNU General Public License, version 3 (GPL-3.0)
**
** NOTE: TEsted on Arduino NANO whose I2C pins are A4==SDA, A5==SCL
*/
#include <Wire.h>
#include <LCD.h>
#include <LiquidCrystal_I2C.h>

#define I2C_ADDR    0x27  // Define I2C Address where the PCF8574A is
#define BACKLIGHT_PIN     3
#define En_pin  2
#define Rw_pin  1
#define Rs_pin  0
#define D4_pin  4
#define D5_pin  5
#define D6_pin  6
#define D7_pin  7

int n = 1;

LiquidCrystal_I2C	lcd(I2C_ADDR,En_pin,Rw_pin,Rs_pin,D4_pin,D5_pin,D6_pin,D7_pin);

void setup()
{
  lcd.begin (20,4);
  
// Switch on the backlight
  lcd.setBacklightPin(BACKLIGHT_PIN,POSITIVE);
  lcd.setBacklight(HIGH);
  lcd.home ();                   // go home

  lcd.print("SainSmart I2C tester");  
  lcd.setCursor ( 0, 1 );        // go to the 2nd line
  lcd.print("F Malpartida library");
  lcd.setCursor ( 0, 2 );        // go to the third line
  lcd.print("Test By Edward Comer");
  lcd.setCursor ( 0, 3 );        // go to the fourth line
  lcd.print("Iteration No: ");
}

void loop()
{
  // Backlight on/off every 3 seconds
  lcd.setCursor (14,3);        // go col 14 of line 3
  lcd.print(n++,DEC);
  lcd.setBacklight(LOW);      // Backlight off
  delay(3000);
  lcd.setBacklight(HIGH);     // Backlight on
  delay(3000);
}


