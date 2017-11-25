CNCLib / CNCStepper
======

Build your own CNC machine/laser with *CNCLib* and *CNCStepper*.<br />
Both projects are on github.

https://github.com/aiten/CNCLib <br />
https://github.com/aiten/CNCStepper


CNCLib
======

CNCLib is a Windows application. 

Define and configure your own machine
------
- USB Port 
- Axis count and size
- Probe definition
- Initial commands 
- Custom commands  

Control your machine  
-------
- Send commands (gcode)
- Move axis
- Define zero-shift
- Read/Write SD
- Define rotations 
- Configure CNC machine by writing to the Eeprom
- Watch command history

Preview of CNC program 
-------
- Scroll
- Zoom in and out
- Rotate
- Define colors for machine, laser, mill, ...

<img src="Doc/Preview3D.jpg" alt="Drawing" style="width: 200px;"/>

Import and convert GCode/HPGL/Image
-----
GCode
- Import as it is
- Add linenumbers
- Convert e.g. g82 (drill command) because your machine does not support the command

HPGL
- Import HPGL with "Resize" and "Move"

Image - for grave
- Import Image (laser)

Image - for cut holes



