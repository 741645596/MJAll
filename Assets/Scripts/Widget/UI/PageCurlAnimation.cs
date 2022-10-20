using UnityEngine;

public class PageCurlAnimation : MonoBehaviour
{
    /// <summary>
    /// 正面
    /// </summary>
    public RectTransform front;
    /// <summary>
    /// 背面
    /// </summary>
    public RectTransform back;
    /// <summary>
    /// 阴影
    /// </summary>
    public RectTransform shadow;
    /// <summary>
    /// 裁切面
    /// </summary>
    public RectTransform clippingPlane;
    /// <summary>
    /// 时间参数
    /// </summary>
    public float durationTime = 0.5f;
    /// <summary>
    /// 高度参数
    /// </summary>
    public float kHeight = 0.5f;
    /// <summary>
    /// 缓存对角线长度
    /// </summary>
    private float hypotenous;
    /// <summary>
    /// 根节点的RectTransform
    /// </summary>
    private RectTransform root;
    /// <summary>
    /// 动画时间
    /// </summary>
    private float timer = 0;
    /// <summary>
    /// 动画起点
    /// </summary>
    /// <returns></returns>
    private Vector3 startPoint;
    /// <summary>
    /// 终点位置
    /// </summary>
    private Vector3 endPoint;
    /// <summary>
    /// 动画持续时间
    /// </summary>
    private float duration;


    void Start()
    {
        InitPageCurl();
    }

    void Update()
    {
        if (timer < 0 || duration <0)
        {
            return;
        }

        timer -= Time.deltaTime;
        var f = Vector3.Lerp(startPoint, endPoint, (duration - timer) / duration);

        UpdateCurl(f);
    }

    [ContextMenu("CasePlay")]
    public void CasePlay()
    {
        InitPageCurl();
        PlayAnimation();
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        PlayAnimation(durationTime, kHeight);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration">动画时长</param>
    /// <param name="kHeight">右侧起点的高度比例(0~1)</param>
    public void PlayAnimation(float time, float kHeight)
    {
        if (root == null)
        {
            InitPageCurl();
        }

        if(kHeight > 1)
        {
            kHeight = 1;
        }

        if(kHeight <= 0)
        {
            kHeight = 0.001f;
        }

        startPoint = new Vector3(root.rect.xMax, root.rect.yMin + root.rect.height * kHeight, 0);
        timer = time;
        duration = time;
    }

    public void InitPageCurl()
    {
        if (root != null)
        {
            return;
        }

        root = GetComponent<RectTransform>();

        float pageWidth = root.rect.width;
        float pageHeight = root.rect.height;
        hypotenous = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);

        clippingPlane.sizeDelta = new Vector2(hypotenous, hypotenous);

        shadow.sizeDelta = new Vector2(pageWidth, hypotenous);
        shadow.pivot = new Vector2(0, 0.5f);

        clippingPlane.pivot = new Vector2(0, 0.5f);

        front.position = root.position;
        front.localEulerAngles = new Vector3(0, 0, 0);
        front.SetParent(clippingPlane, true);
        front.SetAsFirstSibling();

        back.pivot = new Vector2(1, 0);
        back.eulerAngles = new Vector3(0, 0, 0);
        back.SetParent(clippingPlane, true);

        endPoint = new Vector3(root.rect.xMin, root.rect.yMin, 0);
    }

    public void UpdateCurl(Vector3 point)
    {
        var c = Calc_C_Position(point);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, new Vector3(root.rect.xMin, root.rect.yMin, 0), out t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        if ((int)t1.x <= (int)root.rect.xMin)
        {
            clippingPlane.localEulerAngles = Vector3.zero;
        }
        else
        {
            clippingPlane.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        }
        clippingPlane.localPosition = t1;

        back.position = root.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        if ((int)t1.x <= (int)root.rect.xMin)
        {
            back.localEulerAngles = Vector3.zero;
        }
        else
        {
            back.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 90 - clipAngle);
        }

        front.localPosition = clippingPlane.InverseTransformPoint(root.TransformPoint(Vector3.zero));
        front.eulerAngles = Vector3.zero;

        shadow.localPosition = back.InverseTransformPoint(clippingPlane.TransformPoint(Vector3.zero));
        shadow.eulerAngles = clippingPlane.eulerAngles;
    }

    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        float F_SB_dy = followLocation.y - root.rect.yMin;
        float F_SB_dx = followLocation.x - 0;
        float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        float radius1 = root.rect.width * 0.5f;
        Vector3 sb = new Vector3(0, root.rect.yMin, 0);
        Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

        float F_SB_distance = Vector2.Distance(followLocation, sb);
        if (F_SB_distance < radius1)
            c = followLocation;
        else
            c = r1;

        Vector3 st = new Vector3(0, root.rect.height / 2);

        float F_ST_dy = c.y - st.y;
        float F_ST_dx = c.x - st.x;
        float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
        Vector3 r2 = new Vector3(hypotenous * Mathf.Cos(F_ST_Angle),
           hypotenous * Mathf.Sin(F_ST_Angle), 0) + st;
        float C_ST_distance = Vector2.Distance(c, st);
        if (C_ST_distance > hypotenous)
            c = r2;
        return c;
    }

    private float CalcClipAngle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        float T0_CORNER_dy = bookCorner.y - t0.y;
        float T0_CORNER_dx = bookCorner.x - t0.x;
        float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);

        float T1_X = t0.x - T0_CORNER_dy * Mathf.Max(Mathf.Tan(T0_CORNER_Angle), 0);
        t1 = new Vector3(T1_X, root.rect.yMin, 0);

        float T0_T1_dy = t1.y - t0.y; 
        float T0_T1_dx = t1.x - t0.x;
        return Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
    }
}
