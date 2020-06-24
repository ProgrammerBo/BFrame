using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExportClear :Editor
{
    
    [MenuItem("Export/Clear/Reporting Editor")]
    static void MenuReportingEditor()
    {
        //AssetDatabase.Refresh(ImportAssetOptions.DontDownloadFromCacheServer);
        //UnityEditor.EditorGUI.CanCacheInspectorGUI(UnityEditor.Selection.activeObject);
        //UnityEditor.CrashReporting.CrashReportingSettings.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
