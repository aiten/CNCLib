//unsigned char pins[] = { 16,17,23,25,27,29,31,33,35,37,39,41,43,45,47,32,49,0};
unsigned char pins[] = { 31,33,35,39,41,43,44,45,47,32,49,64,0};
unsigned char vals[128] = { 0 };

void setup() {
  // put your setup code here, to run once:

Serial.begin(115200);

  for (unsigned char i = 0;pins[i]; i++)
  {
   pinMode(pins[i],INPUT);
   digitalWrite(pins[i],1);
  }
}

void loop() 
{
  // put your main code here, to run repeatedly:


  for (unsigned char i = 0;pins[i]; i++)
  {
    unsigned char val = digitalRead(pins[i]);
    if (val!=vals[i])
    {
      Serial.print(pins[i]);      Serial.print("-");Serial.println(val);
      vals[i]=val;
    }
  }
  

}
