using UnityEngine;
using UnityEngine.UI;
using Unity.Core;
using Unity.Widget;

public static class WebViewHelper
{
    private const string c_UniWebView_Name = "UniWebView_Name";

    /// <summary>
    /// 打开内嵌全屏网页，比如用于客服
    /// </summary>
    /// <param name="url"></param>
    public static void OpenView(string url)
    {
        var obj = CanvasHelper.CreateEmptyCanvas();
        obj.AddTo(WDirector.GetRootLayer().gameObject);
        obj.name = c_UniWebView_Name;

        var wview = obj.AddComponent<UniWebView>();
        wview.ReferenceRectTransform = obj.transform as RectTransform;
        wview.OnShouldClose += OnShouldClose;

        wview.SetShowSpinnerWhileLoading(true);
        wview.SetSpinnerText("加载中...");

        wview.Load(url);
        wview.Show();
    }

    private static bool OnShouldClose(UniWebView view)
    {
        var obj = GameObject.Find(c_UniWebView_Name);
        if (obj != null)
        {
            GameObject.Destroy(obj);
        }
        return true;
    }
}
