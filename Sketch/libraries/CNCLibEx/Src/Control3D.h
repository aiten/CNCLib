
#pragma once

////////////////////////////////////////////////////////

#include <Control.h>

////////////////////////////////////////////////////////

class CControl3D : public CControl
{
private:

	typedef CControl super;

public:

	CControl3D()				 { }

protected:

	virtual void Init();
	virtual void Initialized();
	virtual bool Parse(CStreamReader* reader, Stream* output);

	void InitSD(unsigned char sdEnablePin);

	virtual void ReadAndExecuteCommand();

private:

};

////////////////////////////////////////////////////////
