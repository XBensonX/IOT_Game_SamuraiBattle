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
        client = new MqttClient(ip); // MQTT Broker的IP
        client.MqttMsgPublishReceived += Receive;   // 接收到資料時要執行的方法

        // 連線（Client ID 可自定）
        client.Connect("Subscriber", account, passwd);

        // 訂閱你指定的 topic
        client.Subscribe(new string[] { topic },
                         new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
    }

    private void Receive(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log("Topic: " + e.Topic + ", Message: " + Encoding.UTF8.GetString(e.Message));
    }

    private void OnDisable()
    {
        // 程式關閉時取消訂閱（記得用相同的 topic 名稱）
        client.Unsubscribe(new string[] { topic });
    }
}
