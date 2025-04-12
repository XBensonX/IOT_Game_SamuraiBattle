using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MQTTDataHandler : MonoBehaviour
{
    public static MQTTDataHandler instance;

    [HideInInspector] public string data;

    // TODO: each sensor use respective variable
    public bool isHallTrigger = false;
    public bool isAttackBtnPressed = false;
    public bool isDefenseBtnPressed = false;
    public float joystickVal = 0.0f;
    public Vector3 acceleration_MPU6050 = Vector3.zero;
    public Vector3 gyro_MPU6050 = Vector3.zero;

    void Start()
    {
        if (instance == null) instance = this;
    }

    public void SplitData()
    {
        if (data == string.Empty) return;

        string[] sensorRows = data.Split(','); // First Split: Spliting to each sensor
        foreach (string sensor in sensorRows)
        {
            // Second Split: Spliting 2 column.
            // Format is "SENSOR:DATA", it just need to save DATA
            string[] sensorColumns = sensor.Split(":");

            // Saving data directly if DATA is integer.
            // Otherwise, handle it.

            // TODO: Modify SENSOR's name
            switch (sensorColumns[0])
            {
                case "HallSensor": // Hall Sensor
                    isHallTrigger = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "AttackBtn": // Attack Button
                    isAttackBtnPressed = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "DefenseBtn": // Defense Button
                    isDefenseBtnPressed = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "Joystick": // Joystick
                    joystickVal = float.Parse(sensor.Split(":")[1]);
                    break;
                case "Acceleration_MPU6050": // acceleration in MPU6050
                    string accXYZ_Str = sensor.Split(":")[1].Replace("(", "").Replace(")", "");
                    string[] accXYZ = accXYZ_Str.Split(" "); // Final Split: Spliting to X Y Z for Vector3.
                    acceleration_MPU6050 = new Vector3(float.Parse(accXYZ[0]), float.Parse(accXYZ[1]), float.Parse(accXYZ[2]));
                    break;
                case "Gyro_MPU6050": // gyro in MPU6050
                    string gyroXYZ_Str = sensor.Split(":")[1].Replace("(", "").Replace(")", "");
                    string[] gyroXYZ = gyroXYZ_Str.Split(" "); // Final Split: Spliting to X Y Z for Vector3.
                    gyro_MPU6050 = new Vector3(float.Parse(gyroXYZ[0]), float.Parse(gyroXYZ[1]), float.Parse(gyroXYZ[2]));
                    break;
                default:
                    break;
            }
        }
    }
}
