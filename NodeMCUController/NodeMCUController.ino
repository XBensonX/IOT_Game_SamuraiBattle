#include "I2Cdev.h"
#include "MPU6050.h"
#include <Wire.h>
#include <ESP8266WiFi.h> 
#include <PubSubClient.h>
#include <Adafruit_ADS1X15.h>

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

//#define JOYSTICK_X 0
//#define JOYSTICK_Y 1
#define JOYSTICK_Y A0
#define JOYSTICK_SW 15

WiFiClient espClient;
PubSubClient client(espClient);
MPU6050 mpu;
Adafruit_ADS1115 ads;

TwoWire ads_i2cWire = TwoWire();

void setup() {
    Serial.begin(115200);

    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);
    pinMode(JOYSTICK_SW, INPUT_PULLUP);  // Joystick button as digital input

    // Connect to Wi-Fi
    setup_wifi(); 

    /*--Start I2C interface--*/
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
      Wire.begin(); 
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
      Fastwire::setup(400, true);
    #endif
    
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

//    // Initialize ADS1115
//    ads_i2cWire.begin(0, 2); // using D3 (GPIO0) as SDA, D4 (GPIO2) as SCL (I2C)
//    if (!ads.begin(ADS1X15_ADDRESS, &ads_i2cWire)) {
//      Serial.println("ADS1115 not found.");
//      while (true);
//    }
//    ads.setGain(GAIN_ONE); // GAIN_ONE = ±4.096V (can use to 3.3V joystick)

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
    float joyX;
    float joyY = analogRead(JOYSTICK_Y);
    //int16_t joyX = ads.readADC_SingleEnded(JOYSTICK_X);
    //int16_t joyY = ads.readADC_SingleEnded(JOYSTICK_Y);
    int joySW = !digitalRead(JOYSTICK_SW);

    // transfer to 0.125 mV/bit（GAIN_ONE）
    //float xVoltage = joyX * 0.125 / 1000.0;
    //float yVoltage = joyY * 0.125 / 1000.0;
    
    // Read MPU6050
    int16_t axLSB, ayLSB, azLSB;
    int16_t gx, gy, gz;
    mpu.getMotion6(&axLSB, &ayLSB, &azLSB, &gx, &gy, &gz);
    //mpu.getAcceleration(&axLSB, &ayLSB, &azLSB);
    //mpu.getRotation(&gx, &gy, &gz);
    
    float rateRoll = (float)gx/65.5;
    float ratePitch = (float)gy/65.5;
    float rateYaw = (float)gz/65.5;

    float ax = (float)axLSB / 32768;
    float ay = (float)ayLSB / 32768;
    float az = (float)azLSB / 32768;

    // Prepare the payload as a JSON string or simple text
    String payload = "Hall_Sensor:" + String(hallValue) + ","
                     + "Button_1:" + String(button1State) + ","
                     + "Button_2:" + String(button2State) + ","
                     + "Joystick:(" + String(joyX) + " " + String(joyY) + " " + String(joySW) + "),"
                     + "Accel:(" + String(axLSB) + " " + String(ayLSB) + " " + String(azLSB) + "),"
                     + "Gyro:(" + String(gx) + " " + String(gy) + " " + String(gz) + ")";

    // Publish the payload to the MQTT topic
    if (client.publish(mqtt_topic, payload.c_str())) {
        Serial.println("Data sent to MQTT broker");
    } else {
        Serial.println("Failed to send data to MQTT broker");
    }

    delay(10); // Wait for a while before sending the next set of data
}
