// @Author: tanjinhua
// @Date: 2021/10/27  17:47


using UnityEditor;

public class FntImporter : AssetPostprocessor
{
    /// <summary>
    /// 将fnt格式转为fontsetting格式
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            string path = str.Replace("\\", "/");
            if (path.EndsWith(".fnt"))
            {
                Fnt2FontSetting.Generate(path);
            }
        }
    }
}
