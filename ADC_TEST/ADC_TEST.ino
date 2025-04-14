#include <Wire.h>
#include <Adafruit_ADS1X15.h>

Adafruit_ADS1115 ads;  // 建立 ADS1115 物件

void setup() {
  Serial.begin(115200);

  // 🟡 使用 D3 (GPIO0) 當 SDA，D4 (GPIO2) 當 SCL（軟體 I2C）
  Wire.begin(0, 2);

  // 初始化 ADS1115（預設位址 0x48）
  if (!ads.begin()) {
    Serial.println("找不到 ADS1115，請檢查接線！");
    while (1);
  }

  // 設定增益範圍（GAIN_ONE = ±4.096V，適合 3.3V 搖桿）
  ads.setGain(GAIN_ONE);

  Serial.println("初始化完成，開始讀取搖桿資料...");
}

void loop() {
  // 讀取 A0（VRx）與 A1（VRy）通道的原始 ADC 值
  int16_t x = ads.readADC_SingleEnded(0);
  int16_t y = ads.readADC_SingleEnded(1);

  // 換算為電壓：0.125 mV/bit（GAIN_ONE）
  float x_voltage = x * 0.125 / 1000.0;
  float y_voltage = y * 0.125 / 1000.0;

  // 顯示結果
  Serial.print("X 軸電壓: "); Serial.print(x_voltage); Serial.print(" V\t");
  Serial.print("Y 軸電壓: "); Serial.print(y_voltage); Serial.println(" V");

  delay(300);  // 可依需求調整速度
}
