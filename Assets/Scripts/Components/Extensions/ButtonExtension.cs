using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    /// <summary>
    /// 设置能否点击，按钮置灰
    /// </summary>
    /// <param name="bt"></param>
    public static void SetEnable(this Button bt, bool enable)
    {
        bt.enabled = enable;
        bt.interactable = enable;
    }
}
