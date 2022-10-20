using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置字体间距
/// </summary>
[AddComponentMenu("UI/Text Spacing")]
public class TextSpacing : BaseMeshEffect
{
    public enum HorizontalAligmentType
    {
        Left,
        Center,
        Right
    }

    public float Spacing = 1f;
    public List<UIVertex> vertexs = new List<UIVertex>();
    private HorizontalAligmentType alignment;
    private Text text;

    protected override void OnEnable()
    {
        text = GetComponent<Text>();
        if (text.alignment == TextAnchor.LowerLeft || text.alignment == TextAnchor.MiddleLeft || text.alignment == TextAnchor.UpperLeft)
        {
            alignment = HorizontalAligmentType.Left;
        }
        else if (text.alignment == TextAnchor.LowerCenter || text.alignment == TextAnchor.MiddleCenter || text.alignment == TextAnchor.UpperCenter)
        {
            alignment = HorizontalAligmentType.Center;
        }
        else
        {
            alignment = HorizontalAligmentType.Right;
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
        {
            return;
        }

        if (text == null)
        {
            WLDebug.LogError("Missing Text component");
            return;
        }

        vertexs.Clear();
        vh.GetUIVertexStream(vertexs);
        UIVertex vt;
        var charCount = (text.text.Length) * 6;
        for (var j = 0; j < charCount; j++)
        {
            vt = vertexs[j];
            if (alignment == HorizontalAligmentType.Left)
            {
                vt.position.x += Spacing * (j / 6);
            }
            else if (alignment == HorizontalAligmentType.Right)
            {
                vt.position.x += Spacing * (-(charCount - j) / 6 + 1);
            }
            else if (alignment == HorizontalAligmentType.Center)
            {
                var offset = (charCount / 6) % 2 == 0 ? 0.5f : 0f;
                vt.position.x += Spacing * (j / 6 - charCount / 12 + offset);
            }

            vertexs[j] = vt;
            // 以下注意点与索引的对应关系
            if (j % 6 <= 2)
            {
                vh.SetUIVertex(vt, (j / 6) * 4 + j % 6);
            }

            if (j % 6 == 4)
            {
                vh.SetUIVertex(vt, (j / 6) * 4 + j % 6 - 1);
            }
        }

        //int indexCount = vh.currentIndexCount;
        //UIVertex vt;
        //for (int i = 6; i < indexCount; i++)
        //{

        //    //第一个字不用改变位置
        //    vt = vertexs[i];
        //    vt.position.x += Spacing * (i / 6);
        //    vertexs[i] = vt;
        //    //以下注意点与索引的对应关系
        //    if (i % 6 <= 2)
        //    {
        //        vh.SetUIVertex(vt, (i / 6) * 4 + i % 6);
        //    }
        //    if (i % 6 == 4)
        //    {
        //        vh.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
        //    }
        //}
    }
}