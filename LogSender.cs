using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LogSender
{
    private static readonly HttpClient httpClient = new HttpClient();


    public static async Task SendLog<T>(string apiURL, string sheetURL, T dataClass) where T : class
    {
        // var sendDataClass = new SendLogClass<T>(sheetURL, dataClass);
        string json = JsonUtility.ToJson(dataClass);

        Debug.Log(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(apiURL, content);
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("ログ送信成功");
            }
            else
            {
                Debug.LogError("ログ送信失敗: " + response.StatusCode);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("通信エラー: " + e.Message);
        }
    }
}

public class SendLogClass<T> where T : class
{
    public SendLogClass(string sheetURL, T a)
    {
        this.sheetURL = sheetURL;
        json = JsonUtility.ToJson(a);
    }
    public string sheetURL;

    public string json;
}
