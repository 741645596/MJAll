using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Utility
{
    public static class ArrayIntEx
    {
        /// <summary>
        /// 默认系统提供的排序是不稳定排序，这个是稳定排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static List<int> SortByCondition(List<int> array, Func<int, int, bool> compare)
        {
            var count = array.Count;
            if (count < 2)
            {
                return array;
            }

            var len = array.Count;
            for (var i = 0; i < len - 1; i++)
            {
                for (var j = 0; j < len - 1 - i; j++)
                {
                    if (compare(array[j + 1], array[j]))// 相邻元素两两对比
                    {
                        var temp = array[j + 1];        // 元素交换
                        array[j + 1] = array[j];
                        array[j] = temp;
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<int> Copy(List<int> array)
        {
            return new List<int>(array);
        }

        /// <summary>
        /// 反向排序
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<int> Reverse(List<int> array)
        {
            array.Reverse();
            return array;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="appedList"></param>
        /// <returns></returns>
        public static List<int> Append(List<int> array, List<int> appedList)
        {
            array.AddRange(appedList);
            return array;
        }

        /// <summary>
        /// 删除指定数据，如果有多个只删除第一个
        /// </summary>
        /// <param name="array"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<int> RemoveObject(List<int> array, int obj)
        {
            array.Remove(obj);
            return array;
        }

        /// <summary>
        /// 删除数组最后一个数据
        /// </summary>
        /// <param name="array"></param>
        public static void RemoveLastObject(List<int> array)
        {
            if (array.Count == 0)
            {
                return;
            }

            array.RemoveAt(array.Count - 1);
        }

        /// <summary>
        /// 将新数据插入到首位位置
        /// </summary>
        /// <param name="array"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static List<int> InsertObjects(List<int> array, List<int> objs)
        {
            for (var i = objs.Count - 1; i >= 0; i--)
            {
                array = InsertObject(array, objs[i]);
            }
            return array;
        }

        /// <summary>
        /// 指定位置插入数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="indexs"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static List<int> InsertObjectsAtIndexs(List<int> array, int[] indexs, List<int> objs)
        {
            for (int i = 0; i < indexs.Length; i++)
            {
                var index = indexs[i] + i;
                var obj = objs[i];
                array = InsertObjectsAtIndex(array, index, obj);
            }
            return array;
        }

        /// <summary>
        /// 在首位插入数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<int> InsertObject(List<int> array, int obj)
        {
            array.Insert(0, obj);
            return array;
        }

        /// <summary>
        /// 指定位置插入数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<int> InsertObjectsAtIndex(List<int> array, int index, int obj)
        {
            if (index >= array.Count)
            {
                array.Add(obj);
                return array;
            }

            array.Insert(index, obj);
            return array;
        }

        /// <summary>
        /// 根据自定义条件查找需要的数据集合
        /// </summary>
        /// <param name="array"></param>
        /// <param name="condFun"></param>
        /// <returns></returns>
        public static List<int> FindSetByCondition(List<int> array, Func<int, bool> condFun)
        {
            var values = new List<int>();
            for (int i = 0; i < array.Count; i++)
            {
                if (condFun(array[i]))
                {
                    values.Add(array[i]);
                }
            }
            return values;
        }

        /// <summary>
        /// 自定义条件查找数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="condFun"></param>
        /// <returns></returns>
        public static int Find(List<int> array, Func<int, bool> condFun)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (condFun(array[i]) == true)
                {
                    return array[i];
                }
            }
            return default;
        }

        /// <summary>
        /// 查找value在array的索引值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns>查找不到返回 -1否则返回具体数值</returns>
        public static int FindIndex(List<int> array, int value)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (value.Equals(array[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 以joinStr链接字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="joinStr"></param>
        /// 示例：Concat(["a", "b", "c"], "-") -> "a-b-c"
        public static string Concat(List<string> source, string joinStr)
        {
            return string.Join(joinStr, source);
        }

        /// <summary>
        /// 删除所有包含toCompare数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public static List<int> RemoveAllObject(List<int> array, int toCompare)
        {
            array.RemoveAll(item => { return item == toCompare; });
            return array;
        }

        /// <summary>
        /// 重小到大排序，不稳定排序
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<int> Sort(List<int> array)
        {
            array.Sort();
            return array;
        }

        /// <summary>
        /// 分类排序
        /// </summary>
        /// <param name="array"></param>
        /// 示例：如[1, 2, 3, 4, 5, 6]想把3、4排前面变成[3, 4, 1, 2, 5, 6]
        /*  classify([1, 2, 3, 4, 5, 6], function(v)
            {
                return v==3 || v==4
            })
        */
        public static List<int> Classify(List<int> array, Func<int, bool> condFun)
        {
            var trueArr = new List<int>();
            var falseArr = new List<int>();
            for (var i = 0; i < array.Count; i++)
            {
                var value = array[i];
                if (condFun(value))
                {
                    trueArr.Add(value);
                }
                else
                {
                    falseArr.Add(value);
                }
            }

            for (var i = 0; i < trueArr.Count; i++)
            {
                array[i] = trueArr[i];
            }

            var startIndex = trueArr.Count;
            for (var i = 0; i < falseArr.Count; i++)
            {
                array[i + startIndex] = falseArr[i];
            }
            return array;
        }



        /// <summary>
        /// 把数组分类成指定类型分类
        /// </summary>
        /*
            * 参数：condFun = function(v) { return key }
            * 示例：classifyintoArray([ {k:1, v:"a"}, {k:1, v:"b"}, {k:2, v:"c"}, {k:2, v:"d"}, {k:3, v:"c"} ], function(v)
            {
                reutrn v.k
            }) ->
            [
                [ {k:1, v:"a"}, {k:1, v:"b"} ],
                [ {k:2, v:"c"}, {k:2, v:"d"} ],
                [ {k:3, v:"c"} ]
            ]
        */
        public static List<List<int>> ClassifyintoArray(List<int> array, Func<int, string> func)
        {
            var tmpData = new Dictionary<string, List<int>>();
            foreach (var value in array)
            {
                var key = func(value);
                var data = tmpData.ContainsKey(key) ? tmpData[key] : new List<int>();
                data.Add(value);
                tmpData[key] = data;
            }

            var newData = new List<List<int>>();
            foreach (var item in tmpData)
            {
                newData.Add(item.Value);
            }
            return newData;
        }

        /// <summary>
        /// 删除array最后一个值，并且将返回最后一个值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Pop(List<int> array)
        {
            int count = array.Count;
            if (count == 0)
            {
                return default;
            }

            int lastValue = array[count - 1];
            array.RemoveAt(count - 1);
            return lastValue;
        }
    }

}