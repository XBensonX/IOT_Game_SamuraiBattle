using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using TMPro;

public class ConnectToBroker : MonoBehaviour
{
    public static ConnectToBroker instance;

    [SerializeField] private string brokerAddr = "127.0.0.1";
    [SerializeField] private string username = "mqtt";
    [SerializeField] private string passwd = "passwd";

    [SerializeField] private string[] subscribeTopics = { "sensor/data" };
    [SerializeField] private string[] publishTopics = { "unity/data" };
    [SerializeField] byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };

    private MqttClient client;

    void Start()
    {
        if (instance == null) instance = this;

        client = new MqttClient(brokerAddr);
        client.MqttMsgPublishReceived += Recieve;

        client.Subscribe(subscribeTopics, qosLevels);

        client.Connect("UnityClient", username, passwd);
    }

    void Recieve(object sender, MqttMsgPublishEventArgs e)
    {
        MQTTDataHandler.instance.data = Encoding.Default.GetString(e.Message);
        //Debug.Log(MQTTDataHandler.instance.data);
        MQTTDataHandler.instance.SplitData();
    }

    public void Publish(string message)
    {
        client.Publish(publishTopics[0], Encoding.UTF8.GetBytes(message));
    }

    private void OnDisable()
    {
        client.Unsubscribe(subscribeTopics);
    }
}
