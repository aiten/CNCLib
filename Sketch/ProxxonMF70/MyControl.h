#pragma once

////////////////////////////////////////////////////////

#include <Control.h>

#include "CoolantControl.h"
#include "SpindelControl.h"
#include "ControllerFanControl.h"
#include "ProbeControl.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	CMyControl()				 { }

	virtual void Kill();

	virtual void IOControl(unsigned char tool, unsigned short level);
	virtual unsigned short IOControl(unsigned char tool);

protected:

	virtual void Init();
	virtual void Initialized();
	virtual bool Parse();

	virtual void GoToReference();
	virtual void GoToReference(axis_t axis);

	virtual void ReadAndExecuteCommand();

	virtual bool OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, unsigned char addinfo);

private:

	CCoolantControl _coolant;
	CSpindelControl _spindel;
	CControllerFanControl _controllerfan;

};

////////////////////////////////////////////////////////

extern CMyControl Control;
