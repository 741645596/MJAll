using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image组件无法K帧，可以挂载该方法通过设置属性值的方式k帧（但是这个只支持k一个属性，后续在考虑多属性情况）
/// </summary>
[ExecuteAlways]
public class ImageMaterialFloat : MonoBehaviour
{
    public string materialProperty;
    [SerializeField]
    private float _value;
    public float value
    {
        get { return _value; }
        set
        {
            if (material == null)
            {
                var image = GetComponent<Image>();
                material = image.material;
            }
            material.SetFloat(materialProperty, value);
        }
    }

    
    private float lastValue;
    private Material material;
    
    void Start()
    {
        var image = GetComponent<Image>();
        material = image.material;        
    }

    private void Update()
    {
        if(material == null)
        {
            return;
        }

        if(lastValue == value)
        {
            return;
        }
        material.SetFloat(materialProperty, value);
        lastValue = value;
    }
}
