using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using TMPro;

public class ConnectToBroker : MonoBehaviour
{
    [SerializeField] private string brokerAddr = "127.0.0.1";
    [SerializeField] private string username = "mqtt";
    [SerializeField] private string passwd = "passwd";

    [SerializeField] private string[] topics = { "sensor/data" };
    [SerializeField] byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };
            
    void Start()
    {
        MqttClient client = new MqttClient(brokerAddr);
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        client.Subscribe(topics, qosLevels);

        client.Connect("UnityClient", username, passwd);
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string mqttPayload = System.Text.Encoding.Default.GetString(e.Message);
        Debug.Log(mqttPayload);
    }
}
