typedef unsigned char axis_t;  // type for "axis"
typedef long mm1000_t;
typedef long sdist_t;

typedef mm1000_t(*ToMm1000_t) (axis_t axis, sdist_t val);
typedef sdist_t(*ToMachine_t) (axis_t axis, mm1000_t val);

void setup() 
{
  Serial.begin(115200);

  Serial.println("Hallo Arduino/Uno");
}

inline unsigned long MulDivU32(unsigned long v, unsigned long m, unsigned long d)
{
  return (v * m) / d;
}

inline long MulDivI32(long v, long m, long d)
{
  return (v * m) / d;
}

inline unsigned long RoundMulDivU32(unsigned long v, unsigned long m, unsigned long d)
{
  return (v * m + d / 2) / d;
}

inline long RoundMulDivI32(long v, long m, long d)
{
  return (v * m + d / 2) / d;
}

inline long ToMm1000_float(axis_t /* axis */, long val)               { return  (long) (val * (80.0/256.0)); }
inline long  ToMachine_float(axis_t /* axis */, long val)             { return  (long) (val * (256.0/80.0)); }

inline long ToMm1000_float_round(axis_t /* axis */, long val)         { return  (long) ((val * (80.0/256.0))+0.5); }
inline long  ToMachine_float_round(axis_t /* axis */, long val)       { return  (long) ((val * (256.0/80.0))+0.5); }

inline mm1000_t ToMm1000_1_3200(axis_t /* axis */, sdist_t val)        { return  MulDivU32(val, 80, 256); }
inline sdist_t  ToMachine_1_3200(axis_t /* axis */, mm1000_t val)      { return  MulDivU32(val, 256, 80); }
inline mm1000_t ToMm1000_1_3200R(axis_t /* axis */, sdist_t val)       { return  RoundMulDivU32(val, 80, 256); }
  

inline mm1000_t ToMm1000Inch_1_3200(axis_t /* axis */, sdist_t val)    { return  MulDivU32(val, 25, 2032); }
inline sdist_t  ToMachineInch_1_3200(axis_t /* axis */, mm1000_t val)  { return  MulDivU32(val, 2032, 25); }


void TestFnc(ToMm1000_t fnc, char* text) 
{
  unsigned long timeto = millis()+10000;
  unsigned long i=0;

  while (millis()< timeto)
  {
    fnc(0, 1414);
    fnc(0, 2414);
    fnc(0, 3414);
    fnc(0, 4414);
    fnc(0, 5414);
    fnc(0, 6414);
    fnc(0, 7414);
    fnc(0, 8414);
    fnc(0, 9414);
    fnc(0,10414);
    i+=10;
  }

  Serial.print(text);
  Serial.print(i);
  Serial.println();
}

void loop() 
{
  TestFnc(ToMm1000_float, "float: ");
  TestFnc(ToMm1000_float_round, "floatR:");
  TestFnc(ToMm1000_1_3200,"int1:  ");
  TestFnc(ToMm1000_1_3200R,"int1r: ");
  TestFnc(ToMm1000Inch_1_3200,"int2:  ");
}

