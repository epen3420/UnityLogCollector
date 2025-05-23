using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LogSender
{
    private static readonly HttpClient httpClient = new HttpClient();
    /// <summary>
    /// スプシのURLを含めたラッパークラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class SendLogClass<T>
    {
        public SendLogClass(string sheetURL, T datas)
        {
            this.sheetURL = sheetURL;
            this.datas = datas;
        }

        public string sheetURL;
        public T datas;
    }

    public static async Task SendLog<T>(string gasURL,
                                        string sheetURL,
                                        T dataClass)
    {
        var sendDataClass = new SendLogClass<T>(sheetURL, dataClass);
        string json = JsonUtility.ToJson(sendDataClass);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(gasURL, content);
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
