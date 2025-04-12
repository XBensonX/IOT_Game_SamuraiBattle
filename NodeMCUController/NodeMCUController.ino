#include "I2Cdev.h"
#include "MPU6050.h"
#include <Wire.h>
#include <ESP8266WiFi.h> 
#include <PubSubClient.h>

// Wi-Fi settings
const char* ssid  = "HW_ROUTER";
const char* password= "t12312361";

// MQTT settings
const char* mqtt_server = "192.168.1.109";
const int mqtt_port =1883;
const char* mqtt_user = "mqtt";
const char* mqtt_password = "passwd";
const char* mqtt_topic = "sensor/data";

// Pin definitions
#define HALL_SENSOR_PIN 14  // D5
#define BUTTON1_PIN 12      // D6
#define BUTTON2_PIN 13      // D7

#define JOYSTICK_X A0
#define JOYSTICK_SW 15

#define OUTPUT_READABLE_ACCELGYRO

WiFiClient espClient;
PubSubClient client(espClient);

/* MPU6050 default I2C address is 0x68*/
MPU6050 mpu;

void setup() {
    /*--Start I2C interface--*/
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
      Wire.begin(); 
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
      Fastwire::setup(400, true);
    #endif
  
    Serial.begin(115200);

    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);
    pinMode(JOYSTICK_SW, INPUT_PULLUP);  // Joystick button as digital input

    // Connect to Wi-Fi
    setup_wifi();

    // Initialize MPU-6050
    Serial.println("Initializing MPU...");
    mpu.initialize();
    Serial.println("Testing MPU6050 connection...");
    if(mpu.testConnection() ==  false){
      Serial.println("MPU6050 connection failed");
      while(true);
    }
    else{
      Serial.println("MPU6050 connection successful");
    }
    offsetMPU6050();

    client.setServer(mqtt_server, mqtt_port);
    Serial.print(mqtt_server);
}

void setup_wifi() {
    delay(10);
    Serial.println();
    Serial.print("Connecting to WiFi...");

    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) {
        delay(250);
        Serial.print(".");
    }

    Serial.println("Connected to WiFi");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
}

void reconnect() {
    while (!client.connected()) {
        Serial.print("Attempting MQTT connection...");
        if (client.connect("ESP8266", mqtt_user, mqtt_password)) {
            Serial.println("Connected to MQTT broker");
        } else {
            Serial.print("Failed to connect, rc=");
            Serial.print(client.state());
            delay(2000);
        }
    }
}

void offsetMPU6050() {
    /* Use the code below to change accel/gyro offset values. Use MPU6050_Zero to obtain the recommended offsets */ 
    Serial.println("Updating internal sensor offsets...\n");
    mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
    mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
    mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
    mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
    mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
    mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
    /*Print the defined offsets*/
    Serial.print("\t");
    Serial.print(mpu.getXAccelOffset());
    Serial.print("\t");
    Serial.print(mpu.getYAccelOffset()); 
    Serial.print("\t");
    Serial.print(mpu.getZAccelOffset());
    Serial.print("\t");
    Serial.print(mpu.getXGyroOffset()); 
    Serial.print("\t");
    Serial.print(mpu.getYGyroOffset());
    Serial.print("\t");
    Serial.print(mpu.getZGyroOffset());
    Serial.print("\n");
}

void loop() {
    // Ensure MQTT connection
    if (!client.connected()) {
        reconnect();
    }
    client.loop();

    int hallValue = digitalRead(HALL_SENSOR_PIN);
    int button1State = digitalRead(BUTTON1_PIN);
    int button2State = digitalRead(BUTTON2_PIN);

    // Read joystick
    int joyX = analogRead(JOYSTICK_X);
    int joySW = digitalRead(JOYSTICK_SW);
    joySW=!joySW;
    // Read MPU6050
    int16_t ax, ay, az;
    int16_t gx, gy, gz;
    mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
    //mpu.getAcceleration(&ax, &ay, &az);
    //mpu.getRotation(&gx, &gy, &gz);

    // Prepare the payload as a JSON string or simple text
    String payload = "Hall_Sensor:" + String(hallValue) + ","
                     + "Button_1:" + String(button1State) + ","
                     + "Button_2:" + String(button2State) + ","
                     + "Joystick_X:" + String(joyX) + ","
                     + "Joystick_SW:" + String(joySW) + ","
                     + "Accel:(" + String(ax) + " " + String(ay) + " " + String(az) + "),"
                     + "Gyro:(" + String(gx) + " " + String(gy) + " " + String(gz) + ")";

    // Publish the payload to the MQTT topic
    if (client.publish(mqtt_topic, payload.c_str())) {
        Serial.println("Data sent to MQTT broker");
    } else {
        Serial.println("Failed to send data to MQTT broker");
    }

    delay(10); // Wait for a while before sending the next set of data
}
