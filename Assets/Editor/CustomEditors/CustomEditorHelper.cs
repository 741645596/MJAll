// @Author: tanjinhua
// @Date: 2021/1/14  13:53


using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomEditorHelper
{
    // UnityEditor.UI.MenuOptions.GetStandardResources()
    private static MethodInfo m_getStandardResources;
    public static MethodInfo getStandardResources
    {
        get
        {
            if (m_getStandardResources == null)
            {
                Assembly assembly = GetAssembly("UnityEditor.UI");
                if (assembly != null)
                {
                    m_getStandardResources = assembly.GetType("UnityEditor.UI.MenuOptions")
                        .GetMethod("GetStandardResources", BindingFlags.NonPublic | BindingFlags.Static);
                }
            }
            return m_getStandardResources;
        }
    }

    // UnityEditor.UI.MenuOptions.PlaceUIElementRoot()
    private static MethodInfo m_placeUIElementRoot;
    public static MethodInfo placeUIElementRoot
    {
        get
        {
            if (m_placeUIElementRoot == null)
            {
                Assembly assembly = GetAssembly("UnityEditor.UI");
                if (assembly != null)
                {
                    m_placeUIElementRoot = assembly.GetType("UnityEditor.UI.MenuOptions")
                        .GetMethod("PlaceUIElementRoot", BindingFlags.NonPublic | BindingFlags.Static);
                }
            }
            return m_placeUIElementRoot;
        }
    }

    private static Assembly GetAssembly(string name)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if (assembly.GetName().Name == name)
            {
                return assembly;
            }
        }
        return null;
    }

    //3D
    [MenuItem("GameObject/3D Object/RT System", false)]
    public static void AddRenderTextureSystem(MenuCommand menuCommand)
    {
        GameObject obj = RTSystem.CreateTemplate();
    }


    [MenuItem("GameObject/UI/Weile/Skew Image", false)]
    public static void AddImage(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Skew Image", typeof(SKImage));

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Skew Raw Image", false)]
    public static void AddRawImage(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Skew Raw Image", typeof(SKRawImage));

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Remote Image", false)]
    public static void AddRemoteImage(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Remote Image", typeof(RemoteImage));

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/RTSystem Image", false)]
    public static void AddRTSystemImage(MenuCommand menuCommand)
    {
        var obj = new GameObject("RTSystem Image", typeof(RectTransform));

        var image = obj.AddComponent<SKRawImage>();

        var rtSys = RTSystem.CreateTemplate();

        rtSys.transform.SetParent(obj.transform);

        image.rtSystem = rtSys.GetComponent<RTSystem>();

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Skew Text", false)]
    public static void AddText(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Skew Text", typeof(SKText));

        SKText text = obj.GetComponent<SKText>();
        text.text = "New Skew text";

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Art Text", false)]
    public static void AddArtText(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Art Text", typeof(ArtText));
        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Long Press Button", false)]
    public static void AddLongPressButton(MenuCommand menuCommand)
    {
        Debug.Log("创建长按型按钮，使用方式同Button一样");
        GameObject obj = new GameObject("LongPressButton", typeof(Image));

        obj.AddComponent<LongPressButton>();
        
        InitializeUIElement(obj, menuCommand, true);
    }

    [MenuItem("GameObject/UI/Weile/Polygon Button", false)]
    public static void AddPolygonButton(MenuCommand menuCommand)
    {
        Debug.Log("创建多边形点击区域的按钮成功，编辑Polygon Collider 2D组件自定义响应区域");

        GameObject obj = new GameObject("PolygonButton", typeof(PolygonClickImage));

        var p2d = obj.GetComponent<PolygonCollider2D>();
        float w = 50;
        float h = 50;
        p2d.points = new Vector2[]
        {
            new Vector2(-w, -h),
            new Vector2(w, -h),
            new Vector2(w, h),
            new Vector2(-w, h)
        };

        obj.AddComponent<Button>();

        InitializeUIElement(obj, menuCommand, true);
    }

    [MenuItem("GameObject/UI/Weile/Progress Indicator", false)]
    public static void AddProgressIndicator(MenuCommand menuCommand)
    {
        GameObject obj = ProgressIndicator.CreateTemplate();

        InitializeUIElement(obj, menuCommand);
    }

    [MenuItem("GameObject/UI/Weile/Progress Radial", false)]
    public static void AddProgressRadial(MenuCommand menuCommand)
    {
        Debug.Log("创建雷达型倒计时，比如技能框倒计时");
        GameObject obj = ProgressRadial.CreateTemplate();
        obj.AddComponent<ProgressRadial>();
        InitializeUIElement(obj, menuCommand);
    }


    /// <summary>
    /// 序列帧动画
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("GameObject/UI/Weile/Sequence Sprite", false)]
    public static void AddAnimationSprite(MenuCommand menuCommand)
    {
        Debug.Log("创建序列帧动画");
        GameObject obj = new GameObject("Animation Sprite", typeof(Image));

        obj.AddComponent<SequenceSprite>();

        InitializeUIElement(obj, menuCommand);
    }

    /// <summary>
    /// 可以滚动的label，主要用于太长的公告
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("GameObject/UI/Weile/Label Scroll", false)]
    public static void AddLabelScroll(MenuCommand menuCommand)
    {
        Debug.Log("创建可滚动的长文本控件，比如公告");
        var obj = LabelScroll.CreateTemplate();
        InitializeUIElement(obj, menuCommand, true);
    }

    [MenuItem("GameObject/UI/Weile/Page View", false)]
    public static void AddPageView(MenuCommand menuCommand)
    {
        GameObject obj = PageRect.CreateTemplate();

        InitializeUIElement(obj, menuCommand, true);
    }

    [MenuItem("GameObject/UI/Weile/Table View", false)]
    public static void AddTableView(MenuCommand menuCommand)
    {
        Debug.Log("创建Table View，比如用于排行榜。你还需要创建相应的TableViewCase组件。\n" +
            "具体参考示例：TableViewCase");

        /// 创建默认的Scroll View
        GameObject go = DefaultControls.CreateScrollView((DefaultControls.Resources)getStandardResources.Invoke(null, null));
        
        /// 默认竖型
        var sr = go.GetComponent<ScrollRect>();
        sr.horizontal = false;

        // 修改默认参数
        var contentTran = go.FindReference("Content") as RectTransform;
        contentTran.anchorMin = new Vector2(0, 1);
        contentTran.anchorMax = new Vector2(0, 1);
        contentTran.offsetMin = Vector2.zero;
        contentTran.offsetMax = Vector2.zero;
        contentTran.pivot = new Vector2(0, 1);

        go.AddComponent<TableView>();

        placeUIElementRoot.Invoke(null, new object[] { go, menuCommand });
    }

    [MenuItem("GameObject/UI/Weile/Table View Cell", false)]
    public static void AddTableViewCell(MenuCommand menuCommand)
    {
        Debug.Log("创建Table View Cell成功，你需要在代码里继承TableViewCell然后初始化预设" +
            "具体参考示例：TableViewCase");

        /// 创建默认的Scroll View
        GameObject obj = new GameObject("TableViewCell", typeof(RectTransform));
        var transform = obj.transform as RectTransform;
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.zero;
        transform.offsetMin = Vector2.zero;
        transform.offsetMax = Vector2.zero;
        transform.pivot = new Vector2(0, 1);

        placeUIElementRoot.Invoke(null, new object[] { obj, menuCommand });
    }

    [MenuItem("GameObject/UI/Weile/Canvas", false)]
    public static void AddTCanvas(MenuCommand menuCommand)
    {
        Debug.Log("备注：UI/Canvas创建的Canvas实例化后Anchors被重置0，有可能是unity的bug，请统一使用该方法创建/管理UI层级");

        GameObject obj = CanvasHelper.CreateEmptyCanvas();
        obj.AddComponent<GraphicRaycaster>();
        placeUIElementRoot.Invoke(null, new object[] { obj, menuCommand });

        obj.GetComponent<Canvas>().overrideSorting = true;

        var rectTransform = obj.transform as RectTransform;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    private static void InitializeUIElement(GameObject uielement, MenuCommand menuCommand, bool raycastTarget = false)
    {
        placeUIElementRoot.Invoke(null, new object[] { uielement, menuCommand });

        RectTransform trs = uielement.transform as RectTransform;
        trs.anchoredPosition = Vector2.zero;

        if (uielement.TryGetComponent(out Graphic graphic))
        {
            graphic.raycastTarget = raycastTarget;
        }
    }
}
