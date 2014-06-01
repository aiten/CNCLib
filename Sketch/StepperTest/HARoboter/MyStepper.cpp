#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Settings.h"
#include <HAStepper.h>

#include "Global.h"
#include "MyCommand.h"


void CMyStepper::OnWait()
{
  super::OnWait();
  drawloop();
}
