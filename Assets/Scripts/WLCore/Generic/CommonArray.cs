// CommonArray.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/7/16
using System.Collections.Generic;
namespace WLCore.Generic
{
    public class CommonArray
    {
        private readonly List<object> list = new List<object>();

        public CommonArray(params object[] args)
        {
            if (args == null)
            {
                return;
            }
            for (int i = 0; i < args.Length; i++)
            {
                list.Add(args[i]);
            }
        }

        public object this[int index]
        {
            get
            {
                if (index >= list.Count)
                {
                    return null;
                }
                return list[index];
            }
            private set
            {
                list[index] = value;
            }
        }

        public void Add(object obj)
        {
            list.Add(obj);
        }

        public void Clear()
        {
            list.Clear();
        }

        public string GetString(int index, string defaultValue = "")
        {
            if (this[index] != null)
            {
                return this[index].ToString();
            }

            return defaultValue;
        }

        public int GetInt(int index, int defaultValue = 0)
        {
            if (this[index] != null)
            {
                int.TryParse(this[index].ToString(), out int result);
                return result;
            }
            return defaultValue;
        }

        public float GetFloat(int index, float defaultValue = 0f)
        {
            if (this[index] != null)
            {
                float.TryParse(this[index].ToString(), out float result);
                return result;
            }
            return defaultValue;
        }

        public object GetObject(int index)
        {
            if (this[index] != null)
            {
                return this[index];
            }
            return null;
        }

        public bool GetBool(int index, bool defaultValue = false)
        {
            if (this[index] != null)
            {
                bool.TryParse(this[index].ToString(), out bool result);
                return result;
            }
            return defaultValue;
        }
    }
}
