/* 
	Editor: http://www.visualmicro.com
	        arduino debugger, visual micro +, free forum and wiki
	
	Hardware: Arduino Due (Native USB Port), Platform=sam, Package=arduino
*/

#define __SAM3X8E__
#define USB_VID 0x2341
#define USB_PID 0x003e
#define USBCON
#define USB_MANUFACTURER "\"Unknown\""
#define USB_PRODUCT "\"Arduino Due\""
#define ARDUINO 101
#define ARDUINO_MAIN
#define F_CPU 84000000L
#define printf iprintf
#define __SAM__
#define __cplusplus
extern "C" void __cxa_pure_virtual() {;}

static void DrawLine(unsigned char line, unsigned long pos);
static void DrawAll();
static void WaitBusy();
static void Test1();
//
//

#include "D:\user\Herbert\Arduino\arduino-1.5.5\hardware\arduino\sam\variants\arduino_due_x\pins_arduino.h" 
#include "D:\user\Herbert\Arduino\arduino-1.5.5\hardware\arduino\sam\variants\arduino_due_x\variant.h" 
#include "D:\user\Herbert\Arduino\arduino-1.5.5\hardware\arduino\sam\cores\arduino\arduino.h"
#include "D:\user\Herbert\Arduino\src\Sketch\StepperTestRamps14\StepperTestRamps14.ino"
