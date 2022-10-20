// @Author: tanjinhua
// @Date: 2021/5/1  16:56


using System;
using System.Collections.Generic;

public static class ArrayExtension
{

    /// <summary>
    /// 切片
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static T[] Slice<T>(this T[] source, int start, int end)
    {
        if (end < 0)
        {
            end = source.Length + end;
        }
        int len = end - start;

        T[] res = new T[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = source[i + start];
        }
        return res;
    }

    /// <summary>
    /// 根据条件计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static int CountByCondition<T>(this T[] array, Func<T, bool> condition)
    {
        return new List<T>(array).CountByCondition(condition);
    }

    /// <summary>
    /// 判断是否包含某个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool Contains<T>(this T[] array, T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(item))
            {
                return true;
            }
        }

        return false;
    }
}
