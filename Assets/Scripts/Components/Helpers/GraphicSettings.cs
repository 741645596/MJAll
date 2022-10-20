// @Author: tanjinhua
// @Date: 2021/1/21  23:38


using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[Serializable]
public class GraphicSettings
{
    public enum DirtyFlag
    {
        Skew,
        Color,
        Maskable,
        GreyScaleOnly,
        DarkenAmount,
        BlendMode
    }

    public event Action<DirtyFlag> onDirty;


    [SerializeField]
    private Color32 m_color = Color.white;
    public Color32 color
    {
        get
        {
            return m_color;
        }
        set
        {
            if (m_color.Equals(value))
            {
                return;
            }
            m_color = value;
            onDirty?.Invoke(DirtyFlag.Color);
        }
    }
    public byte opacity
    {
        get
        {
            return m_color.a;
        }
        set
        {
            if (m_color.a == value)
            {
                return;
            }
            Color32 c = m_color;
            c.a = value;
            color = c;
        }
    }

    [SerializeField]
    private bool m_greyScaleOnly = false;
    public bool greyScaleOnly
    {
        get
        {
            return m_greyScaleOnly;
        }
        set
        {
            if (m_greyScaleOnly == value)
            {
                return;
            }
            m_greyScaleOnly = value;
            onDirty?.Invoke(DirtyFlag.GreyScaleOnly);
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float m_darkenAmount = 0f;
    public float darkenAmount
    {
        get
        {
            return m_darkenAmount;
        }
        set
        {
            float amount = Mathf.Clamp01(value);
            if (m_darkenAmount == amount)
            {
                return;
            }
            m_darkenAmount = amount;
            onDirty?.Invoke(DirtyFlag.DarkenAmount);
        }
    }

    [SerializeField]
    private BlendMode m_blendModeSrc = BlendMode.SrcAlpha;
    public BlendMode blendModeSrc
    {
        get
        {
            return m_blendModeSrc;
        }
        set
        {
            if (m_blendModeSrc == value)
            {
                return;
            }
            m_blendModeSrc = value;
            onDirty?.Invoke(DirtyFlag.BlendMode);
        }
    }

    [SerializeField]
    private BlendMode m_blendModeDst = BlendMode.OneMinusSrcAlpha;
    public BlendMode blendModeDst
    {
        get
        {
            return m_blendModeDst;
        }
        set
        {
            if (m_blendModeDst == value)
            {
                return;
            }
            m_blendModeDst = value;
            onDirty?.Invoke(DirtyFlag.BlendMode);
        }
    }

    [SerializeField]
    private bool m_maskable = true;
    public bool maskable
    {
        get
        {
            return m_maskable;
        }
        set
        {
            if (m_maskable == value)
            {
                return;
            }
            m_maskable = value;
            onDirty?.Invoke(DirtyFlag.Maskable);
        }
    }

    [SerializeField]
    private Vector2 m_skew = Vector2.zero;
    public Vector2 skew
    {
        get
        {
            return m_skew;
        }
        set
        {
            if (m_skew == value)
            {
                return;
            }
            m_skew = value;
            onDirty?.Invoke(DirtyFlag.Skew);
        }
    }


    public bool useDefaultMaterial
    {
        get
        {
            return m_darkenAmount == 0f &&
                m_greyScaleOnly == false &&
                m_blendModeSrc == BlendMode.SrcAlpha &&
                m_blendModeDst == BlendMode.OneMinusSrcAlpha;
        }
    }

    public void Reset()
    {
        m_color = Color.white;
        m_greyScaleOnly = false;
        m_darkenAmount = 0f;
        m_blendModeSrc = BlendMode.SrcAlpha;
        m_blendModeDst = BlendMode.OneMinusSrcAlpha;
        m_maskable = true;
        m_skew = Vector2.zero;
    }

    public GraphicSettings Clone()
    {
        return new GraphicSettings().Copy(this);
    }

    public GraphicSettings Copy(GraphicSettings other)
    {
        m_color = other.color;
        m_greyScaleOnly = other.greyScaleOnly;
        m_darkenAmount = other.darkenAmount;
        m_blendModeSrc = other.blendModeSrc;
        m_blendModeDst = other.blendModeDst;
        m_maskable = other.maskable;
        m_skew = other.skew;
        return this;
    }

    public GraphicSettings SetFromGraphic(Graphic graphic)
    {
        return Copy(graphic.FetchGraphicSettings());
    }

    public GraphicSettings Mix(GraphicSettings other)
    {
        if (other == null)
        {
            return this;
        }

        Color ca = color, cb = other.color;
        color = new Color(ca.r * cb.r, ca.g * cb.g, ca.b * cb.b, ca.a * cb.a);
        darkenAmount += other.darkenAmount;

        return this;
    }

    public override string ToString()
    {
        string str = "color: " + m_color + "\t";
        str += "greyScaleOnly: " + m_greyScaleOnly + "\t";
        str += "blendModeSrc: " + m_blendModeSrc + "\t";
        str += "blendModeDst: " + m_blendModeDst + "\t";
        str += "maskable: " + m_maskable + "\t";
        str += "darkenAmount: " + m_darkenAmount + "\t";
        str += "skew" + m_skew;
        return str;
    }
}