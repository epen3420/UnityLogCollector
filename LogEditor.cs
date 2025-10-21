using UnityEditor;
using UnityEngine;

public class LogEditor : EditorWindow
{
    [MenuItem("Tools/LogSenderSetting")]
    private static void OpenWindow()
    {
        GetWindow<LogEditor>();
    }

    private string gasURL = "";
    private string sheetURL = "";
    private const string GAS_URL_KEY = "GAS_URL";
    private const string SHEET_URL_KEY = "SHEET_URL";

    private void OnEnable()
    {
        gasURL = EditorPrefs.GetString(GAS_URL_KEY);
        sheetURL = EditorPrefs.GetString(SHEET_URL_KEY);
    }

    private void OnGUI()
    {
        GUILayout.Label("LogSender Settings");

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("GAS URL");
        gasURL = GUILayout.TextField(gasURL);

        GUILayout.Label("Sheet URL");
        sheetURL = GUILayout.TextField(sheetURL);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(GAS_URL_KEY, gasURL);
            EditorPrefs.SetString(SHEET_URL_KEY, sheetURL);
        }
    }
}
