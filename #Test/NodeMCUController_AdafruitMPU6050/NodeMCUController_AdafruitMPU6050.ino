#include <Adafruit_Sensor.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_ADS1X15.h>
#include <Wire.h>
#include <ESP8266WiFi.h> 
#include <PubSubClient.h>

Adafruit_MPU6050 mpu;

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
Adafruit_ADS1115 ads;

TwoWire mpu_i2cWire = TwoWire();
//TwoWire ads_i2cWire = TwoWire();

void setup() {
    Serial.begin(115200);

    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);
    pinMode(JOYSTICK_SW, INPUT_PULLUP);  // Joystick button as digital input

    // Connect to Wi-Fi
    setup_wifi();
    
    // Initialize MPU-6050
    mpu_i2cWire.begin(4, 5);
    if (!mpu.begin(MPU6050_I2CADDR_DEFAULT, &mpu_i2cWire)) {
        Serial.println("MPU6050 initialization failed!");
        while (1) delay(10);
    } else {
        Serial.println("MPU6050 connected");
    }

    mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
    mpu.setGyroRange(MPU6050_RANGE_500_DEG);
    mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);

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
    sensors_event_t a, g, temp;
    mpu.getEvent(&a, &g, &temp);

    // Prepare the payload as a JSON string or simple text
    String payload = "Hall_Sensor:" + String(hallValue) + ","
                     + "Button_1:" + String(button1State) + ","
                     + "Button_2:" + String(button2State) + ","
                     + "Joystick:(" + String(joyX) + " " + String(joyY) + " " + String(joySW) + "),"
                     + "Accel:(" + String(a.acceleration.x) + " " + String(a.acceleration.y) + " " + String(a.acceleration.z) + "),"
                     + "Gyro:(" + String(g.gyro.x) + " " + String(g.gyro.y) + " " + String(g.gyro.z) + ")";

    // Publish the payload to the MQTT topic
    if (client.publish(mqtt_topic, payload.c_str())) {
        Serial.println("Data sent to MQTT broker");
    } else {
        Serial.println("Failed to send data to MQTT broker");
    }

    delay(10); // Wait for a while before sending the next set of data
}
