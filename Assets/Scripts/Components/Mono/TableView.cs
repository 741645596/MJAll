using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 可复用的Scroll View，主要用于如排行榜这类可复用组件，以提高性能
/// 使用方法，鼠标右键：UI -> Weile -> Table View会帮忙创建所需组件
/// </summary>
public class TableView : MonoBehaviour
{
    private int _allCount;
    private Vector2 _cellSize;
    private Queue<TableViewCell> _reclaimCellQueue;
    private List<TableViewCell> _liveCellList;
    private Func<TableView, int> _numberOfCellsCB;
    private Func<TableView, Vector2> _tableCellSizeCB;
    private Func<TableView, int, TableViewCell> _tableCellAtIndexCB;

    private RectTransform _rectTransform;
    private RectTransform _contentTransform;
    private ScrollRect _scrollRect;

    /// <summary>
    /// 总数量回调，必须注册该调用
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public TableView NumberOfCellsCallback(Func<TableView, int> func)
    {
        _numberOfCellsCB = func;
        return this;
    }

    /// <summary>
    /// 创建或复用Cell回调，必须注册该调用
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public TableView TableCellAtIndexCallback(Func<TableView, int, TableViewCell> func)
    {
        _tableCellAtIndexCB = func;
        return this;
    }

    /// <summary>
    /// 每个cell高度，必须注册该调用
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public TableView TableCellSizeForIndexCallback(Func<TableView, Vector2> func)
    {
        _tableCellSizeCB = func;
        return this;
    }

    /// <summary>
    /// 从缓存队列取得复选Cell，如果无返回null
    /// </summary>
    /// <returns></returns>
    public TableViewCell DequeueCell()
    {
        if (_reclaimCellQueue.Count == 0)
        {
            return null;
        }

        return _reclaimCellQueue.Dequeue();
    }

    /// <summary>
    /// 重新加载数据
    /// </summary>
    /// <returns></returns>
    public TableView ReloadData()
    {
        ReloadDataFromPos(0, 0);

        return this;
    }

    /// <summary>
    /// 新增接口，必须大版本更新才能使用
    /// 主要用于需要在指定位置开始初始化，可以减少第一次ReloadData
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TableView ReloadDataFromPos(float x, float y)
    {
        Debug.Assert(this._numberOfCellsCB != null, "必须设置NumberOfCellsCallback回调");
        Debug.Assert(this._tableCellAtIndexCB != null, "必须设置TableCellAtIndexCallback回调");
        Debug.Assert(this._tableCellSizeCB != null, "必须设置TableCellHeightForIndexCallback回调");

        this._allCount = this._numberOfCellsCB(this);
        this._cellSize = this._tableCellSizeCB(this);

        // 重置高度/宽度
        this._ResetInnerContentSize();

        // 回收资源
        this._RecoveryLiveCells();

        // 重置原点
        _contentTransform.anchoredPosition = Vector2.zero;

        // 开始刷新
        this._ReloadCells();

        return this;
    }

    /// <summary>
    /// 获取当前显示的所有cell
    /// </summary>
    /// <returns></returns>
    public List<TableViewCell> GetLiveCells()
    {
        return _liveCellList;
    }

    /// <summary>
    /// 跳到指定位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void JumpToPos(float x, float y)
    {
        _contentTransform.anchoredPosition = new Vector2(x, y);
        _ValueChanged();
    }

    protected void Awake()
    {
        _reclaimCellQueue = new Queue<TableViewCell>();
        _liveCellList = new List<TableViewCell>();
        _allCount = 0;
        _cellSize = Vector2.zero;

        _rectTransform = transform as RectTransform;
        _contentTransform = transform.FindReference("Content") as RectTransform;

        // 滚动回调
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener((v) =>
        {
            this._ValueChanged();
        });

    }

    private void _ReloadCells()
    {
        var firstIndex = this._GetFirstIndex();
        var lastIndex = this._GetLastIndex(firstIndex);
        for (int index = firstIndex; index <= lastIndex; index++)
        {
            this._ShowCell(index);
        }
    }

    private void _ShowCell(int index)
    {
        TableViewCell cell = this._tableCellAtIndexCB(this, index);
        if (cell.GetParent() == null)
        {
            cell.SetParent(_contentTransform);
        }

        cell.SetIndex(index);
        cell.SetActive(true);
        
        var pos = this._GetPosByIndex(index);
        cell.transform.anchoredPosition = pos;

        this._liveCellList.Add(cell);
    }

    private Vector2 _GetPosByIndex(int index)
    {
        var size = _contentTransform.sizeDelta;
        if (_scrollRect.vertical == true)
        {
            return new Vector2(0, size.y - index * this._cellSize.y);
        }
        else
        {
            return new Vector2(index * this._cellSize.x, size.y);
        }
    }

    // 从0开始算
    private int _GetFirstIndex()
    {
        float value = 0;
        int index = 0;
        if (_scrollRect.vertical == true)
        {
            value = _contentTransform.anchoredPosition.y;
            index = (int)Math.Floor(value / this._cellSize.y);
        }
        else
        {
            value = Math.Abs(_contentTransform.anchoredPosition.x);
            index = (int)Math.Floor(value / this._cellSize.x);
        }
        return Math.Max(0, index);
    }

    private int _GetLastIndex(int firstIndex)
    {
        var size = _rectTransform.sizeDelta;
        int count = 0;
        int lastIndex = 0;
        if (_scrollRect.vertical == true)
        {
            // 非整除的需要加上余量，比如可显示的是3个半 要加上半个的偏差
            var offset = size.y % _cellSize.y;
            float y = _contentTransform.anchoredPosition.y + offset;
            y = Math.Max(0f, y);
            y = y % this._cellSize.y;
            var height = size.y - y;
            count = (int)Math.Ceiling(height / this._cellSize.y);
            lastIndex = count + firstIndex;
        }
        else
        {
            var offset = size.x % _cellSize.x;
            float x = Math.Abs(_contentTransform.anchoredPosition.x) + offset;
            x = Math.Max(0f, x);
            x = x % this._cellSize.x;
            var width = size.x - x;
            count = (int)Math.Ceiling(width / this._cellSize.x);
            lastIndex = count + firstIndex;
        }
        return Math.Min(lastIndex, this._allCount - 1);
    }

    private void _ResetInnerContentSize()
    {
        var size = _rectTransform.sizeDelta;
        if (_scrollRect.vertical == true)
        {
            size.y = this._allCount * this._cellSize.y;
        }
        else
        {
            size.x = this._allCount * this._cellSize.x;
        }
        _contentTransform.sizeDelta = size;
    }

    private void _RecoveryLiveCells()
    {
        for (int i = 0; i < _liveCellList.Count; i++)
        {
            _liveCellList[i].SetActive(false);
            _reclaimCellQueue.Enqueue(_liveCellList[i]);
        }
        _liveCellList.Clear();
    }

    private void _ValueChanged()
    {
        var firstIndex = this._GetFirstIndex();
        var lastIndex = this._GetLastIndex(firstIndex);

        // 回收在界面外
        this._RecoveryOutCells(firstIndex, lastIndex);

        // 显示新增
        this._ShowNewCells(firstIndex, lastIndex);
    }

    private bool _IsIndexExist(int index)
    {
        for (int i = 0; i < _liveCellList.Count; i++)
        {
            if (_liveCellList[i].GetIndex() == index)
            {
                return true;
            }
        }

        return false;
    }

    private void _ShowNewCells(int firstIndex, int lastIndex)
    {
        for (int index = firstIndex; index <= lastIndex; index++)
        {
            if (this._IsIndexExist(index) == false)
            {
                this._ShowCell(index);
            }
        }
    }

    private void _RecoveryOutCells(int firstIndex, int lastIndex)
    {
        for (int i = this._liveCellList.Count - 1; i >= 0; i--)
        {
            var cell = this._liveCellList[i];
            int index = cell.GetIndex();
            if (index < firstIndex || index > lastIndex)
            {
                cell.SetActive(false);
                _reclaimCellQueue.Enqueue(cell);
                this._liveCellList.RemoveAt(i);
            }
        }
    }
}
