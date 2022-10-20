// CameraUtil.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/11.24

using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class CameraUtil
{
    private static Camera mainCamera;
    private static Camera uiCamera;
    private static Camera renderCamera;

    public static Rect WorldRect;
    public static void InitWorldRect()
    {
        Vector3 size = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        WorldRect = new Rect(-size.x, -size.z, 2 * size.x, 2 * size.z);
    }

    public static Camera GetMainCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        return mainCamera;
    }

    /// <summary>
    /// 透视相机固定高度适配
    /// </summary>
    /// <param name="camera">相机</param>
    /// <param name="designAspect">设计分辨率的宽高比例</param>
    /// <param name="designFov">设计视野范围Filed of View</param>
    public static void SetupPerspectiveCameraByFixedHeight(Camera camera, float designAspect, float designFov)
    {
        if (camera == null || camera.orthographic)
        {
            return;
        }

        float width = Screen.width > Screen.height ? Screen.width : Screen.height;
        float height = Screen.width > Screen.height ? Screen.height : Screen.width;

        float aspect = width / height;
        float s = aspect / designAspect;

        camera.fieldOfView = designFov * s;
    }

    public static void SetPostProcessingEnable(bool enable)
    {
        var data = Camera.main.GetUniversalAdditionalCameraData();
        data.renderPostProcessing = enable;
    }

    public static Camera GetUICamera()
    {
        if (uiCamera != null)
        {
            return uiCamera;
        }
        uiCamera = GetCamera("UICamera");
        return uiCamera;
    }

    public static Camera GetCamera(string name)
    {
        var obj = GameObject.Find(name);
        if (obj != null)
        {
            return obj.GetComponent<Camera>();
        }
        return null;
    }

    public static Vector2 WorldToRenderUIPoint(Vector3 vector3)
    {
        var camera = CameraUtil.GetMainCamera();
        if (camera == null)
        {
            return Vector2.zero;
        }
        var ret = camera.WorldToScreenPoint(vector3);
        ret.x /= display.srceenScaleFactor;
        ret.y /= display.srceenScaleFactor;
        return new Vector2(ret.x, ret.y);
    }

    public static Vector3 RenderUIToWorldPoint(Vector2 vector2)
    {
        var camera = CameraUtil.GetMainCamera();
        if (camera == null)
        {
            return Vector2.zero;
        }

        vector2.x *= display.srceenScaleFactor;
        vector2.y *= display.srceenScaleFactor;
        var ret = camera.ScreenToWorldPoint(vector2);

        return new Vector3(ret.x, 0, ret.z);
    }

    public static Vector2 WorldToUIPoint(Vector3 vector3)
    {
        var camera = CameraUtil.GetUICamera();
        if (camera == null)
        {
            return Vector2.zero;
        }
        var ret = camera.WorldToScreenPoint(vector3);
        ret.x /= display.srceenScaleFactor;
        ret.y /= display.srceenScaleFactor;
        return new Vector2(ret.x, ret.y);
    }

    public static Vector3 UIToWorldPoint(Vector2 vector2)
    {
        var camera = CameraUtil.GetUICamera();
        if (camera == null)
        {
            return Vector2.zero;
        }

        vector2.x *= display.srceenScaleFactor;
        vector2.y *= display.srceenScaleFactor;
        var ret = camera.ScreenToWorldPoint(vector2);

        return new Vector3(ret.x, ret.y, 0);
    }

    public static Vector3 ScreenToWorldPoint(Vector2 pos)
    {
        pos *= display.srceenScaleFactor;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    public static Vector3 ScreenToWorldPoint(Vector3 pos)
    {
        pos.z = 0;
        pos *= display.srceenScaleFactor;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    public static Vector3 WorldToScreenPoint(Vector3 pos)
    {
        var ret = Camera.main.WorldToScreenPoint(pos);
        ret.z = 0;
        ret.x /= display.srceenScaleFactor;
        ret.y /= display.srceenScaleFactor;
        return ret;
    }
}
