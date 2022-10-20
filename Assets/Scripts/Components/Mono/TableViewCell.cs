using UnityEngine;
using System.Collections;

public class TableViewCell
{
    public GameObject gameObject;
    public RectTransform transform;

    private Transform _parent;
    private int _index;
    private bool _isActive = true;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void InitGameObject(GameObject obj)
    {
        gameObject = obj;
        transform = gameObject.transform as RectTransform;
        Debug.Assert(transform != null, "Transform root is not RectTransform");
    }

    public void SetIndex(int index)
    {
        this._index = index;
    }

    public int GetIndex()
    {
        return this._index;
    }

    public void SetActive(bool r)
    {
        if (_isActive == r)
        {
            return;
        }
        gameObject.SetActive(r);
        _isActive = r;
    }

    public Transform GetParent()
    {
        return _parent;
    }

    public void SetParent(Transform p)
    {
        Debug.Assert(p != null, "SetParent pararm is null");

        _parent = p;
        transform.SetParent(p, false);
    }

}
