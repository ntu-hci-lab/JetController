const int ValvePin[5] = { 2, 4, 7, 8, 9 };
const int AdjForcePin[4] = { 3, 5, 11, 6 };

void setup() {
  //Modify timer2 to let pin 3 & pin 11 be 980Hz
  //Reference:https://www.arduino.cn/thread-12906-1-1.html
  TCCR2B = TCCR2B & 0xF8 | 3; 

  //Set Serial Baud Rate
  Serial.begin(500000);

  //Enable PWM pins
  for (int i = 0; i < 4; ++i)
    pinMode(AdjForcePin[i], OUTPUT);

  //Set All Valve Pins as Output
  for (int i = 0; i < 5; ++i)
    pinMode(ValvePin[i], OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  
  // SW:Switch. 1:ON 0:OFF
  // ID:
  // Left  Right Up  Down  Front Rear
  // 00    01    02  03    04    05
  // Packet Open/Close Valve: 05 00 ID SW 00
  // Packet Adjust Force: 04 ID FR 00
  
  while (Serial.available())
  {
    int PacketLength = Serial.peek();
    while (Serial.available() < PacketLength)
      delay(0);
    switch (Serial.read())
    {
      case 4:
        {
          int ID = Serial.read();
          int PWM = Serial.read();
          int Check = Serial.read();
          if (Check == 0 && ID <= 3 && ID >= 0)
            analogWrite(AdjForcePin[ID], PWM);
        }
        break;
        
      case 5:
        {
          int Check0 = Serial.read();
          int ID = Serial.read();
          int Switch = Serial.read();
          int Check1 = Serial.read();
          if (Check0 == 0 && Check1 == 0 && ID < 5 && (Switch == 0 || Switch || 1))
            digitalWrite(ValvePin[ID], Switch == 0 ? LOW : HIGH);
        }
        break;
      default:
        while (Serial.available())
          Serial.read();
        break;
    }
  }
}
