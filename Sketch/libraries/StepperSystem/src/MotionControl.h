#pragma once

////////////////////////////////////////////////////////

#include "Stepper.h"

////////////////////////////////////////////////////////


// Convert between logical and physical "steps"

////////////////////////////////////////////////////////

#define VMAXTOFEEDRATE(a) (CMotionControl::ToMm1000(0,a*60l))

typedef mm1000_t(*ToMm1000_t) (axis_t axis, sdist_t val);
typedef sdist_t(*ToMachine_t) (axis_t axis, mm1000_t val);

class CMotionControl
{
private:

	static ToMm1000_t _ToMm1000;
	static ToMachine_t _ToMachine;

public:

	static void Arc(const mm1000_t to[NUM_AXIS], float offset0, float offset1, axis_t  axis_0, axis_t axis_1, bool isclockwise, feedrate_t feedrate);
	static void MoveAbs(const mm1000_t to[NUM_AXIS], feedrate_t feedrate);

	static void GetPositions(mm1000_t current[NUM_AXIS]);
	static mm1000_t GetPosition(axis_t axis)													{ return _ToMm1000(axis, CStepper::GetInstance()->GetPosition(axis)); }

	static steprate_t GetFeedRate(const mm1000_t to[NUM_AXIS], feedrate_t feedrate);

	static expr_t ToDouble(const mm1000_t mm1000)												{ return (expr_t)(mm1000 / 1000.0); }
	static mm1000_t FromDouble(expr_t dbl)														{ return (mm1000_t)(dbl * 1000); }

	static mm1000_t ToMm1000(axis_t axis, sdist_t val)											{ return _ToMm1000(axis,val);  }
	static sdist_t ToMachine(axis_t axis, mm1000_t val)											{ return _ToMachine(axis, val); }

	static void ToMachine(const mm1000_t mm1000[NUM_AXIS], udist_t machine[NUM_AXIS])			{ for (axis_t x = 0; x < NUM_AXIS; x++) { machine[x] = _ToMachine(x, mm1000[x]); } };
	static void ToMm1000(const udist_t machine[NUM_AXIS], mm1000_t mm1000[NUM_AXIS])			{ for (axis_t x = 0; x < NUM_AXIS; x++) { mm1000[x] = _ToMm1000(x, machine[x]); } };

	static void InitConversion(ToMm1000_t toMm1000, ToMachine_t toMachine)						{ _ToMm1000 = toMm1000; _ToMachine = toMachine; }

	/////////////////////////////////////////////////////////
	// Samples for converting functions
	//
	// Remark: use fix div and multiplier 2^x => compiler will generate faster inline function 
	//		   256 (as div and mul) => compiler will generate faster inline function 
	//
	// mm1000_t => 2^31 = 2147483648 = 2147483.648 mm = 2147.483648m 
	//
	//
	// functions: 1 rotation(3200Steps) = 1mm

	static mm1000_t ToMm1000_1_3200(axis_t /* axis */, sdist_t val)								{ return  RoundMulDivU32(val, 80, 256); }
	static sdist_t  ToMachine_1_3200(axis_t /* axis */, mm1000_t val)							{ return  MulDivU32(val, 256, 80); }
	
	static mm1000_t ToMm1000Inch_1_3200(axis_t /* axis */, sdist_t val)							{ return  MulDivU32(val, 25, 2032); }
	static sdist_t  ToMachineInch_1_3200(axis_t /* axis */, mm1000_t val)						{ return  MulDivU32(val, 2032, 25); }

	// functions: 1 rotation = 1.25mm

	static mm1000_t ToMm1000_1d25_3200(axis_t /* axis */, sdist_t val)							{ return  MulDivU32(val, 100, 256); }
	static sdist_t  ToMachine_1d25_3200(axis_t /* axis */, mm1000_t val)						{ return  MulDivU32(val, 256, 100); }

	// functions: 1 rotation = 1.5mm

	static mm1000_t ToMm1000_1d5_3200(axis_t /* axis */, sdist_t val)							{ return  MulDivU32(val, 120, 256); }
	static sdist_t  ToMachine_1d5_3200(axis_t /* axis */, mm1000_t val)							{ return  MulDivU32(val, 256, 120); }

	// functions: 1 rotation = 2mm

	static mm1000_t ToMm1000_2_3200(axis_t /* axis */, sdist_t val)								{ return  MulDivU32(val, 160, 256); }
	static sdist_t  ToMachine_2_3200(axis_t /* axis */, mm1000_t val)							{ return  MulDivU32(val, 256, 160); }

	// functions: 1 rotation = 3mm

	static mm1000_t ToMm1000_3_3200(axis_t /* axis */, sdist_t val)								{ return  MulDivU32(val, 240, 256); }
	static sdist_t  ToMachine_3_3200(axis_t /* axis */, mm1000_t val)							{ return  MulDivU32(val, 256, 240); }
};

////////////////////////////////////////////////////////




