#pragma once

////////////////////////////////////////////////////////

#include <Control.h>

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	void MyInit() { return Init(); }

protected:

	virtual void Init();
	virtual void Initialized();
	virtual void Parse();
	virtual void Idle(unsigned int idletime);

	virtual void GoToReference();
	virtual void GoToReference(axis_t axis);

};

////////////////////////////////////////////////////////

extern CMyControl Control;
