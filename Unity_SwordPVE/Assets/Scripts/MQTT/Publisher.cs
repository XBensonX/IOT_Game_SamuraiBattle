using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;

public class Publisher : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public string account = "mqtt";
    public string passwd = "passwd";
    private MqttClient client;
    public int count = 0;

    private void Start()
    {
        client = new MqttClient(ip);//MQTT Broker的IP

        /*
         * 開始連線，第一個傳入的參數是Client ID，目的是用來在Broker內判斷你是誰
         * 如果MQTT Broker需要帳號密碼才能連線，則可在Connect方法內額外傳入
         */
        client.Connect("Publisher", account, passwd);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Publish message"))
        {
            client.Publish("Record", Encoding.UTF8.GetBytes("Hello, This is the " + count + " message."));
            count++;
        }
    }
}
