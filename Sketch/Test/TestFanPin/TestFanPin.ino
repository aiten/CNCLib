const int analogOutPin = 9; // Analog output pin that the LED is attached to

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);

  pinMode(40,OUTPUT);
  pinMode(42,OUTPUT);
}

unsigned char outputValue=128;

void loop() {
  // put your main code here, to run repeatedly:

  
  outputValue++;

  analogWrite(analogOutPin, outputValue);

  digitalWrite(40, (outputValue&128)==0);
  digitalWrite(42, (outputValue&64)==0);

  delay(75);

}
