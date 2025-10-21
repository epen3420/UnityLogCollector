using Prefs = UnityEditor.EditorPrefs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class LogSender
{
    /// <summary>
    /// ジェネリック型 (クラスや構造体) をログとして送信します。
    /// </summary>
    public static async Task SendLog<T>(T data, string sheetName = "null")
    {
        if (data == null)
        {
            Debug.LogError("Can't send log. Because data is null");
            return;
        }

        var dataType = typeof(T);
        var dataProperties = dataType.GetFields();

        if (dataProperties.Length == 0)
        {
            return;
        }

        WWWForm form = new WWWForm();
        InitForm(form, sheetName);

        // キー (プロパティ名) のリストを作成
        var keys = dataProperties.Select(prop => prop.Name).ToArray();
        form.AddField("keys", string.Join(',', keys));

        string logStr = "";
        // 各プロパティの値を追加
        foreach (var prop in dataProperties)
        {
            var propValue = prop.GetValue(data);
            form.AddField(prop.Name, propValue?.ToString() ?? "null");
            logStr += $"[{prop.Name}] {propValue}, ";
        }
        logStr = logStr.Remove(logStr.Length - 2, 2);
        Debug.Log(logStr);

        try
        {
            await PostGas(form);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Log send failed with exception: {e.Message}");
        }
    }

    /// <summary>
    /// Dictionary<string, object> のデータをログとして送信します。
    /// </summary>
    public static async Task SendLog(Dictionary<string, object> data, string sheetName = "null")
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogError("Can't send log. Because data dictionary is null or empty");
            return;
        }

        WWWForm form = new WWWForm();
        InitForm(form, sheetName);

        var keys = data.Keys.ToArray();
        form.AddField("keys", string.Join(',', keys));

        string logStr = "";
        foreach (var pair in data)
        {
            form.AddField(pair.Key, pair.Value?.ToString() ?? "null");
            logStr += $"[{pair.Key}] {pair.Value}, ";
        }
        logStr = logStr.Remove(logStr.Length - 2, 2);
        Debug.Log(logStr);

        try
        {
            await PostGas(form);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Log send failed with exception: {e.Message}");
        }
    }


    private static void InitForm(WWWForm form, string sheetName)
    {
        string sheetURL = Prefs.GetString("SHEET_URL");

        form.AddField("sheetURL", sheetURL);
        form.AddField("sheetName", sheetName);
    }

    private static async Task PostGas(WWWForm form)
    {
        string gasURL = Prefs.GetString("GAS_URL");

        using UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            // 失敗時は例外を投げて呼び出し元に知らせる
            throw new System.Exception($"Failed to send log: {www.error} (Response: {www.downloadHandler.text})");
        }
        else
        {
            Debug.Log($"Success to send log: {www.downloadHandler.text}");
        }
    }
}
