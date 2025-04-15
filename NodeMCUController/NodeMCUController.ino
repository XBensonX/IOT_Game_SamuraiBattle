#include <Wire.h>
#include <MPU6050_light.h>
#include <ESP8266WiFi.h> 
#include <PubSubClient.h>
#include <Adafruit_ADS1X15.h>

// Wi-Fi settings
const char* ssid  = "LAPTOP-SUO53CI2 7053";
const char* password= "467hH=01";

// MQTT settings
const char* mqtt_server = "192.168.137.1";
const int mqtt_port =1883;
const char* mqtt_user = "mqtt";
const char* mqtt_password = "passwd";
const char* mqtt_topic = "sensor/data";
const char* mqtt_topic_unity = "unity/data";

// Pin definitions
#define HALL_SENSOR_PIN 14  // D5
#define BUTTON1_PIN 12      // D6
#define BUTTON2_PIN 13      // D7

//#define JOYSTICK_X 0
//#define JOYSTICK_Y 1
#define JOYSTICK_Y A0
#define JOYSTICK_SW 15

// Constant
#define RAD_TO_DEG (PI/180)

WiFiClient espClient;
PubSubClient client(espClient);
MPU6050 mpu(Wire);
Adafruit_ADS1115 ads;

unsigned long timer = 0;

TwoWire ads_i2cWire = TwoWire();

void setup() {
    Serial.begin(115200);

    pinMode(HALL_SENSOR_PIN, INPUT);
    pinMode(BUTTON1_PIN, INPUT_PULLUP);
    pinMode(BUTTON2_PIN, INPUT_PULLUP);
    pinMode(JOYSTICK_SW, INPUT_PULLUP);  // Joystick button as digital input

    // Connect to Wi-Fi
    setup_wifi(); 

    /***************** MPU6050 *****************/
    Wire.begin();

    byte state = mpu.begin();
    Serial.print(F("MPU6050 status: "));
    Serial.println(state == 0 ? "Success" : "Fail");
    while(state!=0){ } // stop everything if could not connect to MPU6050
    
    Serial.println(F("Calculating offsets, do not move MPU6050"));
    delay(1000);
    mpu.calcOffsets(true,true); // gyro and accelero
    Serial.println("Done!\n");

    /***************** ADS1115: JoySitck I2C *****************/
//    // Initialize ADS1115
//    ads_i2cWire.begin(0, 2); // using D3 (GPIO0) as SDA, D4 (GPIO2) as SCL (I2C)
//    if (!ads.begin(ADS1X15_ADDRESS, &ads_i2cWire)) {
//      Serial.println("ADS1115 not found.");
//      while (true);
//    }
//    ads.setGain(GAIN_ONE); // GAIN_ONE = ±4.096V (can use to 3.3V joystick)

    client.setServer(mqtt_server, mqtt_port);
    client.setCallback(mqttCallback);
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
            client.subscribe(mqtt_topic_unity);
        } else {
            Serial.print("Failed to connect, rc=");
            Serial.print(client.state());
            delay(2000);
        }
    }
}

void mqttCallback(char *topic, byte *payload, unsigned int len) {
    String message = "";
    Serial.print("Message received on topic: ");
    Serial.println(topic);
    Serial.print("Message:");
    for (unsigned int i = 0; i < len; i++) {
        Serial.print((char) payload[i]);
        message += (char) payload[i];
    }
    Serial.println();
    Serial.println("-----------------------");

//    if (message.equals("Reset Position")){
//        Serial.println(F("Calculating offsets, do not move MPU6050"));
//        delay(1000);
//        mpu.calcOffsets(true,true); // gyro and accelero
//        Serial.println("Done!\n");
//    }
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

    /***************** Read joystick *****************/
    float joyX;
    float joyY = analogRead(JOYSTICK_Y);
    //int16_t joyX = ads.readADC_SingleEnded(JOYSTICK_X);
    //int16_t joyY = ads.readADC_SingleEnded(JOYSTICK_Y);
    int joySW = !digitalRead(JOYSTICK_SW);

    // transfer to 0.125 mV/bit（GAIN_ONE）
    //float xVoltage = joyX * 0.125 / 1000.0;
    //float yVoltage = joyY * 0.125 / 1000.0;
    
    /***************** Read MPU6050 *****************/
    mpu.update();
    
    float ax = mpu.getAccX();
    float ay = mpu.getAccY();
    float az = mpu.getAccZ();
    
    float gx = mpu.getGyroX();
    float gy = mpu.getGyroY();
    float gz = mpu.getGyroZ();
    
    float accAngleRoll = mpu.getAccAngleX();
    float accAnglePitch = mpu.getAccAngleY();
    
    float angleRoll = mpu.getAngleX();
    float anglePitch = mpu.getAngleY();
    float angleYaw = mpu.getAngleZ();

//    Serial.print("Roll: ");
//    Serial.print(angleRoll);
//    Serial.print(",Pitch: ");
//    Serial.print(anglePitch);
//    Serial.print(",Yaw: ");
//    Serial.println(angleYaw);

    /***************************************************/

    // Prepare the payload as a JSON string or simple text
    String payload = "Hall_Sensor:" + String(hallValue) + ","
                     + "Button_1:" + String(button1State) + ","
                     + "Button_2:" + String(button2State) + ","
                     + "Joystick:(" + String(joyX) + " " + String(joyY) + " " + String(joySW) + "),"
                     + "Accel:(" + String(ax) + " " + String(ay) + " " + String(az) + "),"
                     + "Gyro:(" + String(gx) + " " + String(gy) + " " + String(gz) + "),"
                     + "Angle:(" + String(angleRoll) + " " + String(anglePitch) + " " + String(angleYaw) + ")";

    // Publish the payload to the MQTT topic
    if (client.publish(mqtt_topic, payload.c_str())) {
        Serial.println(payload);
        Serial.println("Data sent to MQTT broker");
    } else {
        Serial.println("Failed to send data to MQTT broker");
    }

    delay(10); // Wait for a while before sending the next set of data
}
