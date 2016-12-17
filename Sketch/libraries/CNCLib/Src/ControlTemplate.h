////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

enum EReverenceType
{
	NoReference,
	ReferenceToMin,
	ReferenceToMax
};

class CControlTemplate
{
public:

	//////////////////////////////////////////////////////////////
	// inline template

	static inline void InitReference(EnumAsByte(EReverenceType) refx, EnumAsByte(EReverenceType) refy, EnumAsByte(EReverenceType) refz, EnumAsByte(EReverenceType) refa)
	{
		if (refx == ReferenceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
		else if (refx == ReferenceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, false), true);

		if (refy == ReferenceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
		else if (refy == ReferenceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, false), true);

		if (refz == ReferenceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true), true);
		else if (refz == ReferenceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);

		if (refa == ReferenceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, true), true);
		else if (refa == ReferenceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, false), true);
	}

	static inline void SetLimitMinMax(axis_t numxis, udist_t maxx, udist_t maxy, udist_t maxz, udist_t maxa, udist_t maxb, udist_t maxc)
	{
		if (numxis >= 1)
			CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, maxx));
		if (numxis >= 2)
			CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, maxy));
		if (numxis >= 3)
			CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, maxz));
		if (numxis >= 4)
			CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS, maxa));
		if (numxis >= 5)
			CStepper::GetInstance()->SetLimitMax(B_AXIS, CMotionControlBase::GetInstance()->ToMachine(B_AXIS, maxb));
		if (numxis >= 6)
			CStepper::GetInstance()->SetLimitMax(C_AXIS, CMotionControlBase::GetInstance()->ToMachine(C_AXIS, maxc));
	}
};

////////////////////////////////////////////////////////
