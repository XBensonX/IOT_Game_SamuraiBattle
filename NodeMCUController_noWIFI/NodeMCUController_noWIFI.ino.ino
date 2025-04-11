你說：
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>

Adafruit_MPU6050 mpu;

#define HALL_SENSOR_PIN 14  // D5
#define BUTTON1_PIN 12      // D6
#define BUTTON2_PIN 13      // D7

#define JOYSTICK_X A0
#define JOYSTICK_SW 15

void setup() {
    Serial.begin(115200);

    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);

    pinMode(JOYSTICK_SW, INPUT_PULLUP);  // 搖桿按鈕為數位輸入

    // 初始化 MPU-6050
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
    int hallValue = digitalRead(HALL_SENSOR_PIN);
    int button1State = digitalRead(BUTTON1_PIN);
    int button2State = digitalRead(BUTTON2_PIN);

    // 讀取搖桿
    int joyX = analogRead(JOYSTICK_X);
    int joySW = digitalRead(JOYSTICK_SW);

    Serial.print("Hall Sensor: "); Serial.println(hallValue);
    Serial.print("Button 1: "); Serial.print(button1State);
    Serial.print("  |  Button 2: "); Serial.println(button2State);

    Serial.print("Joystick X: "); Serial.print(joyX);
    Serial.print(" | SW: "); Serial.println(joySW);

    // 讀取 MPU6050
    sensors_event_t a, g, temp;
    mpu.getEvent(&a, &g, &temp);

    Serial.print("Accel: ");
    Serial.print(a.acceleration.x); Serial.print(", ");
    Serial.print(a.acceleration.y); Serial.print(", ");
    Serial.println(a.acceleration.z);

    Serial.print("Gyro: ");
    Serial.print(g.gyro.x); Serial.print(", ");
    Serial.print(g.gyro.y); Serial.print(", ");
    Serial.println(g.gyro.z);

    Serial.println("-------------------------");

    delay(500);
}