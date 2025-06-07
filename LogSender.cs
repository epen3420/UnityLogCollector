using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LogSender
{
    private string gasURL;
    private WWWForm form = new WWWForm();
    private List<string> keysOrder = new List<string>();


    public LogSender(string gasURL, string sheetURL)
    {
        this.gasURL = gasURL;
        form.AddField("sheetURL", sheetURL);
    }

    public void SendLog<T>(T data, string sheetName = "null")
    {
        if (data == null)
        {
            Debug.LogError($"Can't send log. Because dataClass is null");
            return;
        }

        var dataType = typeof(T);
        var dataFields = dataType.GetFields();

        WWWForm form = new WWWForm();
        form.AddField("sheetName", sheetName);

        if (dataFields.Length == 0)
        {
            // フィールドがない → int や string など
            Debug.Log($"{dataType.Name} is invalid data. Please pass class or struct as an parameter ");
            return;
        }

        var keys = dataFields.Select(f => f.Name).ToArray();
        form.AddField("keys", string.Join(',', keys));

        foreach (var field in dataFields)
        {
            var fieldValue = field.GetValue(data);
            form.AddField(field.Name, fieldValue?.ToString() ?? "null");
            Debug.Log($"[{field.Name}] {fieldValue}");
        }

        PostGas(form);
    }

    public void AddForm<T>(string key, T value)
    {
        keysOrder.Add(key);
        form.AddField(key, value.ToString());
    }

    public void SendLog(string sheetName = "null")
    {
        if (form.data.Length <= 0)
        {
            Debug.LogError($"Can't send log. Because form is null");
            return;
        }

        form.AddField("sheetName", sheetName);

        // 配列を一つのStringとしてformに設定
        form.AddField("keys", string.Join(',', keysOrder));

        PostGas(form);
    }

    private async void PostGas(WWWForm form)
    {
        using UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Failed to send log: " + www.error);
        else
            Debug.Log($"Success to send log: " + www.downloadHandler.text);
    }
}
