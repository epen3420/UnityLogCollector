using UniGasClient.Data;
using UniGasClient.Editor;
using UnityEditor;
using UnityEngine;

namespace UnityLogSender.Editor
{
    public class GasSettingEditor : EditorWindow
    {
        [MenuItem("Tools/LogSenderSetting")]
        private static void OpenWindow()
        {
            GetWindow<GasSettingEditor>("LogSender Settings");
        }

        private GasSettings settings;


        private void OnEnable()
        {
            settings = GasSettingsService.LoadSettings();

            if (settings == null)
            {
                GasSettingsService.CreateSettings();
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("LogSender Settings", EditorStyles.boldLabel);

            if (settings == null)
            {
                GUILayout.Label("Settings asset could not be loaded.", EditorStyles.helpBox);
                if (GUILayout.Button("Retry Load/Resolve Paths"))
                {
                    OnEnable();
                }
                return;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.Label("GAS URL (Deploy ID)");
            string newDeployId = GUILayout.TextField(settings.DeployId);

            GUILayout.Label("Auth Token");
            string newAuthToken = GUILayout.TextField(settings.AuthToken);

            GUILayout.Label("Sheet ID");
            string newSheetId = GUILayout.TextField(settings.SheetId);


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(settings, "Change LogSender Settings");

                GasSettingsService.UpdateSettings(settings, newDeployId, newAuthToken, newSheetId);
            }

            if (GUILayout.Button("Save Settings to Disk"))
            {
                GasSettingsService.SaveSettingsToDisk();
            }
        }
    }
}
