typedef unsigned char axis_t;  // type for "axis"
typedef long mm1000_t;
typedef long sdist_t;

//typedef mm1000_t(*ToMm1000_t) (axis_t axis, sdist_t val);
typedef sdist_t(*ToMachine_t) (axis_t axis, mm1000_t val);

#define Serial SerialUSB

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

void TestFnc(ToMachine_t fnc, const char* text) 
{
  unsigned long timetogo=5;
  unsigned long timeto = millis()+(timetogo*1000);
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
  Serial.print(F("\t= "));
  Serial.print(float(i)/timetogo);
  Serial.println();
}

void loop() 
{
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) (val * (256.0/80.0)); },          "float: ");
 // TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) ((val * (256.0/80.0))+0.5); },    "floatR:");
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivI32(val, 256, 80); },              "int(256,80): ");
// TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivI32(val, 16,  5); },              "int(16,5):   ");
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivU32(val, 32,  16); },              "int(32,16):  ");
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivU32(val, 17,  16); },              "int(17,16):  ");
    TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivU32(val, 80, 256); },              "int(80,256): ");
    TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivU32(val, 37, 17); },               "int(37,17): ");
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) RoundMulDivU32(val, 256, 80); },         "int1r: ");
//  TestFnc( [] (axis_t /* axis */, mm1000_t val) { return  (sdist_t) MulDivU32(val, 2032, 25); },             "int2:  ");
}

