// @Author: tanjinhua
// @Date: 2021/1/1  23:57


using UnityEngine;
using UnityEngine.EventSystems;


public struct TouchData3D
{
    /// <summary>
    /// 事件接收者
    /// </summary>
    public GameObject receiver;

    /// <summary>
    /// 射线检测索引，用来表示第几个被射线击中，-1表示射线没有击中， 0表示第一个击中
    /// </summary>
    public int hitIndex;

    /// <summary>
    /// 射线检测信息
    /// </summary>
    public RaycastHit hitInfo;

    /// <summary>
    /// UnityEngine.EventSystems返回的数据
    /// </summary>
    public PointerEventData eventData;
}