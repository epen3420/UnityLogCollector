using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestLogCollector : MonoBehaviour
{
    private struct testStruct
    {
        public string character;
        public int hp;
    }

    private async void Start()
    {
        await Send();
    }

    public async Task Send()
    {
        var logClass = new TestLogClass()
        {
            name = "epen",
            hitPoint = 1,
        };
        var test = new testStruct
        {
            character = "ccc",
            hp = 5
        };

        var dict = new Dictionary<string, object>();
        dict.Add("name", "epen");
        dict.Add("age", 2);

        await LogSender.Instance.SendLog(dict); // Dictionary型の物を送る
        await LogSender.Instance.SendLog(test); // 適当なシートにデータを送る
        await LogSender.Instance.SendLog(logClass); // シート2にデータを送る（なければシートを作る）
    }
}

[System.Serializable]
public class TestLogClass
{
    public string name;
    public int hitPoint;
    public float clearTime;
}
