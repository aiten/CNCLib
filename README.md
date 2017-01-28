CNCLib
======

CNCLib is a project to build your own DIY CNC machine.

The first part of the project implements a library to customize CNC machines on
different Arduino hardware.

The second part is designed for Windows to communicate with the CNC (Arduino)
machine.

Arduino
-------

see folder: *Sketch*

### Hardware

-   8 bit Arduino: Uno, Nano, Duemilanove, Mega 2560, Pro

-   32 bit Arduino: Due, zero, M0

-   Arduino Shields: CNCShield, Ramps 1.4, Ramps FD

-   Drivers: A4988, DRV8825, SMC 800, L298 and TB6560

-   LCD 12864 (using u8glib) and SD

-   can be used with servos

### Software

-   Stepper motor library with acceleration support

-   GCode Interpreter basic and extended (Arduino dependent)

-   Axis supported: 4 or 6, depending on the hardware

-   LCD Support

-   SD Support

-   HPGL Interpreter sample

Windows
-------

see folder: *VC/CNCLib*

-   Sending (GCode) to Arduino

-   Configure CNC machine by writing to the Eeprom

-   Support different machines (axis, range, ...)

-   Support CNC-Joystick(Arduino based)

-   Preview

-   Import HPGL with "Resize" and "Move"

-   Import Image (laser)

-   Sample for WPF, WebApi, Enterprise architecture, ...
