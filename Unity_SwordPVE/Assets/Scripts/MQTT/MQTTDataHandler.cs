using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MQTTDataHandler : MonoBehaviour
{
    public static MQTTDataHandler instance;

    [HideInInspector] public string data;

    // Each sensor's data
    [Header("Sensor Data")]
    public bool isHallTrigger = false;
    public bool isAttackBtnPressed = false;
    public bool isResetBtnPressed = false;
    public float joystickVal = 530f;
    public bool isJoystickPressed = false; // Defense
    public Vector3 acceleration_MPU6050 = Vector3.zero;
    public Vector3 gyro_MPU6050 = Vector3.zero;

    [Header("")]
    private float _holdSecs = 0;
    [SerializeField] private float _holdToTriggerSecs = 5f;

    void Start()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        // hold 2 buttons to trigger
        if (isResetBtnPressed) _holdSecs += Time.deltaTime;
        else _holdSecs = 0;
        //Debug.Log(_holdSecs);
        if (_holdSecs >= _holdToTriggerSecs)
        {
            Offset();
            _holdSecs = 0;
        }
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

            switch (sensorColumns[0])
            {
                case "Hall_Sensor": // Hall Sensor
                    isHallTrigger = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "Button_1": // Attack Button
                    isAttackBtnPressed = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "Button_2": // Defense Button
                    isResetBtnPressed = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "Joystick_X": // Joystick movement
                    joystickVal = float.Parse(sensor.Split(":")[1]);
                    break;
                case "Joystick_SW": // Joystick Button
                    isJoystickPressed = sensor.Split(":")[1] == "1" ? true : false;
                    break;
                case "Accel": // acceleration in MPU6050
                    string accXYZ_Str = sensor.Split(":")[1].Replace("(", "").Replace(")", "");
                    string[] accXYZ = accXYZ_Str.Split(" "); // Final Split: Spliting to X Y Z for Vector3.
                    acceleration_MPU6050 = new Vector3(float.Parse(accXYZ[0]), float.Parse(accXYZ[1]), float.Parse(accXYZ[2]));
                    break;
                case "Gyro": // gyro in MPU6050
                    string gyroXYZ_Str = sensor.Split(":")[1].Replace("(", "").Replace(")", "");
                    string[] gyroXYZ = gyroXYZ_Str.Split(" "); // Final Split: Spliting to X Y Z for Vector3.
                    gyro_MPU6050 = new Vector3(float.Parse(gyroXYZ[0]), float.Parse(gyroXYZ[1]), float.Parse(gyroXYZ[2]));
                    break;
                default:
                    break;
            }
        }
    }

    private void Offset()
    {
        PlayerController.instance.originJoystick = joystickVal;
        PlayerController.instance.ResetKatanaPosAndRot();
    }
}
