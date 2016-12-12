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
	ReferaeceToMin,
	ReferaeceToMax
};

template<
	EReverenceType refx,
	EReverenceType refy,
	EReverenceType refz,
	EReverenceType refa
> class CControlTemplate
{
public:

	static void InitReference()
	{
		if (refx == ReferaeceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
		else if (refx == ReferaeceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, false), true);

		if (refy == ReferaeceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
		else if (refy == ReferaeceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, false), true);

		if (refz == ReferaeceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true), true);
		else if (refz == ReferaeceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);

		if (refa == ReferaeceToMin)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, true), true);
		else if (refa == ReferaeceToMax)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, false), true);
	}
};

////////////////////////////////////////////////////////
