using UnityEngine;

public class TestLogCollector : MonoBehaviour
{
    [SerializeField]
    private string apiURL;
    [SerializeField]
    private string sheetURL;


    public async void Send()
    {
        var logClass = new TestLogClass()
        {
            name = "epen",
            hitPoint = 0
        };

        await LogSender.SendLog(apiURL, sheetURL, logClass);
    }
}

public class TestLogClass
{
    public string name;
    public int hitPoint;
}
