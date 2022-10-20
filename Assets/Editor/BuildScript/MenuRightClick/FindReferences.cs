

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindReferences
{
    [MenuItem("Assets/自定义/查找引用该资源的对象")]
    static private void Find()
    {
        // 清除控制台信息
        WLDebug.ClearLogConsole();

#if UNITY_EDITOR_OSX
        FindProjectReferencesForMac();
#else
        FindProjectReferences();
#endif
    }

    static private void FindProjectReferences()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
        var gameAssetPath = Application.dataPath + "/GameAssets";
        string[] files = Directory.GetFiles(gameAssetPath, "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;
        string guid = AssetDatabase.AssetPathToGUID(path);
        int resCount = 0;
        EditorApplication.update = delegate ()
        {
            string file = files[startIndex];
            if (Regex.IsMatch(File.ReadAllText(file), guid))
            {
                Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                resCount++;
            }

            startIndex++;
            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                Debug.Log(resCount + " references found for object " + Selection.activeObject.name);
                startIndex = 0;
                resCount = 0;
            }
        };
    }

    [MenuItem("Assets/自定义/查找引用该资源的对象", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }


	// 查找性能非常快，但是只限mac平台
	private static void FindProjectReferencesForMac()
	{
        string selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string guid = AssetDatabase.AssetPathToGUID(selectedAssetPath);
		var psi = new System.Diagnostics.ProcessStartInfo();
		psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
		psi.FileName = "/usr/bin/mdfind";
		psi.Arguments = "-onlyin " + Application.dataPath + " " + guid;
		psi.UseShellExecute = false;
		psi.RedirectStandardOutput = true;
		psi.RedirectStandardError = true;

		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo = psi;

        string appDataPath = Application.dataPath;
        List<string> references = new List<string>();
        process.OutputDataReceived += (sender, e) => {
			if (string.IsNullOrEmpty(e.Data))
				return;

			string relativePath = "Assets" + e.Data.Replace(appDataPath, "");

			// skip the meta file of whatever we have selected
			if (relativePath == selectedAssetPath + ".meta")
				return;

			references.Add(relativePath);

		};

        string output = "";
        process.ErrorDataReceived += (sender, e) => {
			if (string.IsNullOrEmpty(e.Data))
				return;

			output += "Error: " + e.Data + "\n";
		};
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		process.WaitForExit(2000);

		foreach (var file in references)
		{
			output += file + "\n";
			Debug.Log(file, AssetDatabase.LoadMainAssetAtPath(file));
		}
        
        Debug.Log(references.Count + " references found for object " + Selection.activeObject.name);
	}
}
