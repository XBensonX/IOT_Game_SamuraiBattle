#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>
#include <ESP8266WiFi.h>

// const char* ssid = "HW_ROUTER";
// const char* password = "t12313261";
// WiFiServer server(8000);
String inputString = "";
Adafruit_MPU6050 mpu;

#define HALL_SENSOR_PIN 14  // D5
#define BUTTON1_PIN 12      // D6
#define BUTTON2_PIN 13      // D7
#define TOUCH_SENSOR_PIN A0  //A0


void setup() {
    Serial.begin(115200);
  //   WiFi.begin(ssid, password);
  //   Serial.println("正在連接 WiFi...");
  // while (WiFi.status() != WL_CONNECTED) {
  //   delay(500);
  //   Serial.print(".");
  // }

  // Serial.println("WiFi 已連接！");
  // Serial.print("IP 地址: ");
  // Serial.println(WiFi.localIP());

  // server.begin();
    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);
    pinMode(TOUCH_SENSOR_PIN, INPUT);

    // 初始化 MPU-6050（Adafruit 的寫法）
    if (!mpu.begin()) {
        Serial.println("MPU6050 初始化失敗！");
        while (1) delay(10);
    } else {
        Serial.println("MPU6050 已連接");
    }

    mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
    mpu.setGyroRange(MPU6050_RANGE_500_DEG);
    mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);
}

void loop() {
  //   WiFiClient client = server.available();
  //   if (client) {
  //   Serial.println("有客戶端連進來");

  //   while (client.connected()) {
  //     while (Serial.available()) {
  //       char c = Serial.read();
  //       inputString += c;
  //       if (c == '\n') {
  //         client.print(inputString);  // 將字串傳給 Unity
  //         inputString = "";
  //       }
  //     }
  //   }

  //   client.stop();
  //   Serial.println("客戶端離線");
  // }
  if (Serial.available()) {
    inputString = Serial.readStringUntil('\n');
    Serial.print("收到來自 Nano: "); Serial.println(inputString);
  }
    int hallValue = digitalRead(HALL_SENSOR_PIN);
    int button1State = digitalRead(BUTTON1_PIN);
    int button2State = digitalRead(BUTTON2_PIN);

    int touchValue = analogRead(TOUCH_SENSOR_PIN);

    // Serial.print("Hall Sensor: "); Serial.println(hallValue);
    // Serial.print("Button 1: "); Serial.print(button1State);
    // Serial.print("  |  Button 2: "); Serial.println(button2State);

    // Serial.print("Touch Sensor: "); Serial.println(touchValue);

    sensors_event_t a, g, temp;
    mpu.getEvent(&a, &g, &temp);

    // Serial.print("Accel: ");
    // Serial.print(a.acceleration.x); Serial.print(", ");
    // Serial.print(a.acceleration.y); Serial.print(", ");
    // Serial.println(a.acceleration.z);

    // Serial.print("Gyro: ");
    // Serial.print(g.gyro.x); Serial.print(", ");
    // Serial.print(g.gyro.y); Serial.print(", ");
    // Serial.println(g.gyro.z);

    delay(500);
}