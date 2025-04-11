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
        client = new MqttClient(ip);//MQTT Broker��IP

        /*
         * �}�l�s�u�A�Ĥ@�ӶǤJ���ѼƬOClient ID�A�ت��O�ΨӦbBroker���P�_�A�O��
         * �p�GMQTT Broker�ݭn�b���K�X�~��s�u�A�h�i�bConnect��k���B�~�ǤJ
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
