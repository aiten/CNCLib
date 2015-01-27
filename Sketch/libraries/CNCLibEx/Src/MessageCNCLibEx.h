////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define MESSAGE_PARSER3D_BEGIN_FILE_LIST F("Begin file list")
#define MESSAGE_PARSER3D_END_FILE_LIST F("End file list")
#define MESSAGE_PARSER3D_INITIALIZATION_FAILED F("initialization failed!")
#define MESSAGE_PARSER3D_INITIALIZATION_DONE F("initialization done.")
#define MESSAGE_PARSER3D_ERROR_READING_FILE F("error reading file")
#define MESSAGE_PARSER3D_FILE_OPENED F("File opened: ")
#define MESSAGE_PARSER3D_SIZE F(" Size: ")
#define MESSAGE_PARSER3D_FILE_SELECTED F("File selected")
#define MESSAGE_PARSER3D_NO_FILE_SELECTED F("No file selected for execution or running")
#define MESSAGE_PARSER3D_LINE_SEEK_ERROR F("cannot seek to line")
#define MESSAGE_PARSER3D_SD_PRINTING_BYTE F("SD printing byte ")
#define MESSAGE_PARSER3D_SLASH PSTR("/")
#define MESSAGE_PARSER3D_NOT_SD_PRINTING F("Not SD printing")
#define MESSAGE_PARSER3D_ERROR_CREATING_FILE F("error creating/writing file")
#define MESSAGE_PARSER3D_WRITING_TO_FILE F("Writing to file: ")
#define MESSAGE_PARSER3D_DONE_SAVE_FILE F("Done saving file.")
#define MESSAGE_PARSER3D_FILE_DELETED F("File deleted: ")
#define MESSAGE_PARSER3D_COLON F(":")
#define MESSAGE_PARSER3D_VERSION F("PROTOCOL_VERSION:1.0 FIRMWARE_URL:http//xx.com FIRMWARE_NAME:ProxxonMF70 MACHINE_TYPE:ProxxonMF70 EXTRUDER_COUNT:0")
#define MESSAGE_PARSER3D_FILE_OCCUPIED F("File occupied")
#define MESSAGE_PARSER3D_DIRECOTRY_SPECIFIED F("directory specified")
#define MESSAGE_PARSER3D_CANNOT_DELETE_FILE F("cannot delete file")
#define MESSAGE_PARSER3D_FILE_NOT_EXIST F("file not exists")
#define MESSAGE_PARSER3D_ILLEGAL_FILENAME F("Illegal Filename")

////////////////////////////////////////////////////////

#define MESSAGE_CONTROL3D_InitializingSDCard		F("Initializing SD card...")
#define MESSAGE_CONTROL3D_initializationFailed		MESSAGE_PARSER3D_INITIALIZATION_FAILED
#define MESSAGE_CONTROL3D_initializationDone		MESSAGE_PARSER3D_INITIALIZATION_DONE
#define MESSAGE_CONTROL3D_ExecutingStartupNc		F("Executing startup.nc")
#define MESSAGE_CONTROL3D_NoStartupNcFoundOnSD		F("no startup.nc found on SD")
#define MESSAGE_CONTROL3D_ExecutingStartupNcDone	F("Executing startup.nc done")
