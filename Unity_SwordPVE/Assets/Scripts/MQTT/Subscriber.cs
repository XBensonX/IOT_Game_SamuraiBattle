using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class Subscriber : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public string account = "mqtt";
    public string passwd = "passwd";
    public string topic = "senser/data";
    private MqttClient client;

    private void Start()
    {
        client = new MqttClient(ip); // MQTT Broker��IP
        client.MqttMsgPublishReceived += Receive;   // �������Ʈɭn���檺��k

        // �s�u�]Client ID �i�۩w�^
        client.Connect("Subscriber", account, passwd);

        // �q�\�A���w�� topic
        client.Subscribe(new string[] { topic },
                         new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
    }

    private void Receive(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log("Topic: " + e.Topic + ", Message: " + Encoding.UTF8.GetString(e.Message));
    }

    private void OnDisable()
    {
        // �{�������ɨ����q�\�]�O�o�άۦP�� topic �W�١^
        client.Unsubscribe(new string[] { topic });
    }
}
