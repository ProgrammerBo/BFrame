using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorExportVersion
{

	public static void MenuExportVersion()
    {
        System.DateTime date = System.DateTime.UtcNow.AddHours(8);
        string buildNumber = string.Format(
            "{0}{1}{2}{3}",
            date.Year.ToString().Substring(2),
            date.Month.ToString("00"),
            date.Day.ToString("00"),
            ((date.Hour * 60 + date.Minute) / 15).ToString("00"));
        UnityEditor.PlayerSettings.iOS.buildNumber = buildNumber;
    }
}
