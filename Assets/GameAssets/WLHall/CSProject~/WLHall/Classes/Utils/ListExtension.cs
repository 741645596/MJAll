// @Author: tanjinhua
// @Date: 2021/4/29  20:15


using System;
using System.Collections.Generic;


public static class ListExtension
{
    /// <summary>
    /// 分类排序，把满足条件的元素放在列表前端
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="condition"></param>
    public static void Classify<T>(this List<T> list, Func<T, bool> condition)
    {
        List<T> satisfied = new List<T>();
        List<T> unsatisfied = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            T v = list[i];
            if (condition(v))
            {
                satisfied.Add(v);
            }
            else
            {
                unsatisfied.Add(v);
            }
        }

        list.Clear();

        list.AddRange(satisfied);

        list.AddRange(unsatisfied);
    }

    /// <summary>
    /// 提取。把满足条件的元素放到一个新列表中返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static List<T> Fetch<T>(this List<T> list, Func<T, bool> condition)
    {
        List<T> result = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            T item = list[i];
            if (condition(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// 根据自定义条件获取元素索引，找到第一个后马上返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static int GetIndexByCondition<T>(this List<T> list, Func<T, bool> condition)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (condition(list[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 获取多个元素的索引列表，每个索引只找一次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static List<int> IndicesOf<T>(this List<T> list, List<T> items)
    {
        List<int> result = new List<int>();

        items.ForEach(item =>
        {
            int index = -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (item.Equals(list[i]) && !result.Contains(i))
                {
                    index = i;
                    break;
                }
            }

            result.Add(index);
        });

        return result;
    }

    /// <summary>
    /// 克隆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Clone<T>(this List<T> list)
    {
        return new List<T>(list);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static int Count<T>(this List<T> list, T item)
    {
        int result = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(item))
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// 根据条件计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static int CountByCondition<T>(this List<T> list, Func<T, bool> condition)
    {
        int result = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (condition(list[i]))
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// 根据条件删除元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="condition"></param>
    public static void RemoveByCondition<T>(this List<T> list, Func<T, bool> condition)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (condition(list[i]))
            {
                list.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static List<T> Slice<T>(this List<T> list, int start, int end)
    {
        start = start < 0 ? list.Count + start : start;

        List<T> result = new List<T>();

        int count = end - start;

        for (int i = start; i < count; i++)
        {
            result.Add(list[i]);
        }

        return result;
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static List<T> Slice<T>(this List<T> list, int start)
    {
        int end = list.Count;

        return list.Slice(start, end);
    }

    /// <summary>
    /// 交错列表平铺
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="crossList"></param>
    /// <returns></returns>
    public static List<T> Tile<T>(this List<List<T>> crossList)
    {
        List<T> result = new List<T>();

        for (int i = 0; i < crossList.Count; i++)
        {
            List<T> list = crossList[i];

            for (int j = 0; j > list.Count; j++)
            {
                result.Add(list[j]);
            }
        }

        return result;
    }

    /// <summary>
    /// 检查索引是否有效
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool IndexValid<T>(this List<T> list, int index, string errorMsg = null)
    {
        bool valid = index >= 0 && index < list.Count;

        if (!valid && !string.IsNullOrEmpty(errorMsg))
        {
            WLDebug.LogWarning(errorMsg);
        }

        return valid;
    }

    /// <summary>
    /// 是否与传入的列表具有相同元素(顺序无关)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool HasSameElements<T>(this List<T> list, List<T> other) where T : IComparable
    {
        if (list.Count != other.Count)
        {
            return false;
        }

        List<T> thisCopy = new List<T>(list);
        List<T> otherCopy = new List<T>(other);
        thisCopy.Sort((a, b) => a.CompareTo(b));
        otherCopy.Sort((a, b) => a.CompareTo(b));

        for (int i = 0; i < thisCopy.Count; i++)
        {
            if (!thisCopy[i].Equals(otherCopy[i])) {
                return false;
            }
        }

        return true;
    }
}