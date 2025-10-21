using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection; // リフレクションのために追加

/// <summary>
/// ログをGoogle Apps Scriptに非同期で送信するシングルトンクラス。
/// </summary>
public class LogSender : MonoBehaviour
{
    // シングルトンインスタンス (getのみpublicにし、カプセル化を改善)
    public static LogSender Instance { get; private set; }

    // [SerializeField] を使い、InspectorからURLを設定できるように変更
    [Header("Google Apps Script Settings")]
    [SerializeField]
    private string gasURL = "";

    [SerializeField]
    private string sheetURL = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ロガーはシーン遷移後も維持されるべき
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合は重複させない
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // null または 空文字(Empty) の場合をチェック
        if (string.IsNullOrEmpty(gasURL) || string.IsNullOrEmpty(sheetURL))
        {
            // LogErrorで問題を明確に通知
            Debug.LogError("LogSender: 'gasURL' または 'sheetURL' がInspectorで設定されていません。LogSenderを破棄します。");
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ジェネリック型 (クラスや構造体) の public プロパティ をログとして送信します。
    /// </summary>
    public async Task SendLog<T>(T data, string sheetName = "null")
    {
        if (data == null)
        {
            Debug.LogError("Can't send log. Because data is null");
            return;
        }

        // GetFields (publicフィールド) から GetProperties (publicプロパティ) に変更
        // BindingFlagsを指定してインスタンスのpublicプロパティのみを取得
        var dataProperties = typeof(T).GetFields();

        if (dataProperties.Length == 0)
        {
            Debug.LogWarning($"Can't send log. No public properties found on type {typeof(T).Name}");
            return;
        }

        // T型をDictionaryに変換
        var dataDict = new Dictionary<string, object>();
        foreach (var prop in dataProperties)
        {
            dataDict[prop.Name] = prop.GetValue(data);
        }

        // 共通のPostDataメソッドに処理を委譲
        await PostData(dataDict, sheetName);
    }

    /// <summary>
    /// Dictionary<string, object> のデータをログとして送信します。
    /// </summary>
    public async Task SendLog(Dictionary<string, object> data, string sheetName = "null")
    {
        if (data == null || data.Count == 0)
        {
            Debug.LogError("Can't send log. Because data dictionary is null or empty");
            return;
        }

        // 共通のPostDataメソッドに処理を委譲
        await PostData(data, sheetName);
    }

    /// <summary>
    /// [共通化] 辞書データをWWWFormに変換し、GASにPOSTする内部メソッド
    /// </summary>
    private async Task PostData(Dictionary<string, object> data, string sheetName)
    {
        WWWForm form = new WWWForm();
        InitForm(form, sheetName);

        var keys = data.Keys.ToArray();
        form.AddField("keys", string.Join(',', keys));

        string logStr = "";
        foreach (var pair in data)
        {
            // nullの場合も "null" という文字列を安全に送る
            string valueStr = pair.Value?.ToString() ?? "null";
            form.AddField(pair.Key, valueStr);
            logStr += $"[{pair.Key}] {valueStr}, ";
        }

        // ログ文字列の末尾の ", " を削除
        if (logStr.Length > 2)
        {
            logStr = logStr.Remove(logStr.Length - 2, 2);
            Debug.Log($"Sending Log: {logStr}");
        }

        try
        {
            // POST処理と例外処理
            await PostGas(form);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Log send failed with exception: {e.Message}");
        }
    }

    /// <summary>
    /// フォームの共通初期化
    /// </summary>
    private void InitForm(WWWForm form, string sheetName)
    {
        form.AddField("sheetURL", sheetURL);
        form.AddField("sheetName", sheetName);
    }

    /// <summary>
    /// GASへのPOSTリクエストを非同期で実行
    /// </summary>
    private async Task PostGas(WWWForm form)
    {
        using UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield(); // 1フレーム待機
        }

        // Unity 2020.1以降の標準的な結果判定
        if (www.result != UnityWebRequest.Result.Success)
        {
            // 失敗時は例外を投げて呼び出し元(PostData)に知らせる
            throw new System.Exception($"Failed to send log: {www.error} (Response: {www.downloadHandler.text})");
        }
        else
        {
            Debug.Log($"Success to send log: {www.downloadHandler.text}");
        }
    }
}
