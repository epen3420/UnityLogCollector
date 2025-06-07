using UnityEngine;

public class TestLogCollector : MonoBehaviour
{
    [SerializeField]
    private string apiURL;
    [SerializeField]
    private string sheetURL;

    private struct testStruct
    {
        public string character;
        public int hp;
    }
    public void Send()
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

        new LogSender(apiURL, sheetURL);
    }
}

[System.Serializable]
public class TestLogClass
{
    public string name;
    public int hitPoint;
    public float clearTime;
}
