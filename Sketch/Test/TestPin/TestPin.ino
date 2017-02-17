void setup() {
  // put your setup code here, to run once:

pinMode(9,INPUT);
pinMode(10,INPUT);
pinMode(11,INPUT);
pinMode(12,INPUT);
pinMode(13,INPUT);
pinMode(14,INPUT);
pinMode(15,INPUT);
pinMode(16,INPUT);
pinMode(17,INPUT);
pinMode(18,INPUT);
pinMode(A5,INPUT);
pinMode(A6,INPUT);
pinMode(A7,INPUT);
digitalWrite(9,HIGH);
digitalWrite(10,HIGH);
digitalWrite(11,HIGH);
digitalWrite(12,HIGH);
digitalWrite(13,HIGH);
digitalWrite(14,HIGH);
digitalWrite(15,HIGH);
digitalWrite(16,HIGH);
digitalWrite(17,HIGH);
digitalWrite(18,HIGH);
digitalWrite(A5,HIGH);
digitalWrite(A6,HIGH);
digitalWrite(A7,HIGH);
Serial.begin(115200);

pinMode(12,OUTPUT);
pinMode(13,OUTPUT);

Serial.begin(250000);
}

int pin9=false;
int pin10=false;
int pin11=false;
int pin12=false;
int pin13=false;

int pin14=false;
int pin15=false;
int pin16=false;
int pin17=false;
int pin18=false;
int pinA5=false;
int pinA6=false;
int pinA7=false;

void TestPin(int pin, int&pinval)
{
  if (digitalRead(pin)!=pinval)
  {
    pinval = digitalRead(pin);
    Serial.print(pin);Serial.print(":");Serial.println(pinval);
  }
}

int i=0;

void loop() 
{
//i++;
//analogWrite(13,i);
//analogWrite(12,i);
//return;
  TestPin(9,pin9);
  TestPin(10,pin10);
  TestPin(11,pin11);
  TestPin(12,pin12);
  TestPin(13,pin13);
  TestPin(14,pin14);
  TestPin(15,pin15);
  TestPin(16,pin16);
  TestPin(17,pin17);
  TestPin(18,pin18);
  TestPin(A5,pinA5);
  TestPin(A6,pinA6);
  TestPin(A7,pinA7);

}
