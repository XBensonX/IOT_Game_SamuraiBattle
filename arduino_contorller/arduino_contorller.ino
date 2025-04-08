void setup() {
  Serial.begin(115200);  // 與 NodeMCU 溝通
}

void loop() {
  int joyX = analogRead(A0); // Joystick X 軸
  int joyY = analogRead(A1); // Joystick Y 軸

  // 格式為 X,Y\n，方便 NodeMCU 使用 readStringUntil('\n') 解析
  Serial.print(joyX);
  Serial.print(",");
  Serial.println(joyY);
  Serial.print("\n"); 

  delay(50);  // 控制頻率
}