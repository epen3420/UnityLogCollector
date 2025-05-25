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

        await LogSender.SendLog2(apiURL, sheetURL, logClass);
    }
}

[System.Serializable]
public class TestLogClass
{
    public string name;
    public int hitPoint;
}
