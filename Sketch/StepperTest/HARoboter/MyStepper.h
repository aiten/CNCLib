#pragma once

////////////////////////////////////////////////////////

class CMyStepper : public CStepperRamps14
{
private:
        typedef CStepperRamps14 super;
  
  protected:

	virtual void OnWait();
};

