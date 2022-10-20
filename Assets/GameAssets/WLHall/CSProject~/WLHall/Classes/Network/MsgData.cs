using System.Collections;
using System.Collections.Generic;


namespace WLHall
{
    public class MsgData
    {
        private Hashtable hashtable;
        private ArrayList format;

        public MsgData(Hashtable hashtable = null, ArrayList format = null)
        {
            this.hashtable = hashtable ?? new Hashtable();
            this.format = format;
        }

        public object this[string key]
        {
            get
            {
                CheckKey(key);
                return hashtable?[key];
            }
            set
            {
                if (hashtable.ContainsKey(key))
                {
                    hashtable[key] = value;
                }
                else
                {
                    hashtable.Add(key, value);
                }
            }
        }

        public MsgData DeepClone()
        {
            if (format == null)
            {
                WLDebug.LogWarning("MsgData.DeepClone: 需要format");
                return null;
            }
            MsgData clone = new MsgData(null, format);
            for (var i = 0; i < format.Count; i++)
            {
                ArrayList subFormat = format[i] as ArrayList;
                string key = (string)subFormat[1];
                if (subFormat.Count == 2)
                {
                    clone.SetValue(key, hashtable[key]);
                }
                if (subFormat.Count == 3)
                {
                    ArrayList list = hashtable[key] as ArrayList;
                    ArrayList cloneList = new ArrayList();
                    foreach (var v in list)
                    {
                        cloneList.Add(v);
                    }
                    clone.SetValue(key, cloneList);
                }
                if (subFormat.Count == 4)
                {
                    ArrayList list = hashtable[key] as ArrayList;
                    ArrayList cloneList = new ArrayList();
                    foreach (ArrayList subList in list)
                    {
                        ArrayList cloneSubList = new ArrayList();
                        foreach (var v in subList)
                        {
                            cloneSubList.Add(v);
                        }
                        cloneList.Add(cloneSubList);
                    }
                    clone.SetValue(key, cloneList);
                }
            }
            return clone;
        }

        public T Get<T>(string key)
        {
            if (CheckKey(key))
            {
                return (T)hashtable[key];
            }
            return default;
        }

        public int GetInt(string key)
        {
            return Get<int>(key);
        }

        public bool GetBool(string key)
        {
            return Get<bool>(key);
        }

        public uint GetLong(string key)
        {
            return Get<uint>(key);
        }

        public float GetFloat(string key)
        {
            return Get<float>(key);
        }

        public double GetDouble(string key)
        {
            return Get<double>(key);
        }

        public string GetString(string key)
        {
            return Get<string>(key);
        }

        public int GetActionType(string key)
        {
            return GetInt(key);
        }

        public uint GetEventType(string key)
        {
            return Get<uint>(key);
        }

        public uint GetDWord(string key)
        {
            return Get<uint>(key);
        }

        public int GetByte(string key)
        {
            return GetInt(key);
        }

        public int GetCard(string key)
        {
            return GetInt(key);
        }

        public ushort GetWord(string key)
        {
            return Get<ushort>(key);
        }

        public long GetLongLong(string key)
        {
            return Get<long>(key);
        }

        public FuziData GetFuzi(string key)
        {
            return Get<FuziData>(key);
        }

        public T[] GetArray<T>(string key)
        {
            if (CheckKey(key))
            {
                ArrayList list = hashtable[key] as ArrayList;
                T[] result = new T[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    result[i] = (T)list[i];
                }
                return result;
            }
            return default;
        }

        public List<T> GetList<T>(string key)
        {
            if (CheckKey(key))
            {
                ArrayList list = hashtable[key] as ArrayList;
                List<T> result = new List<T>();
                for (int i = 0; i < list.Count; i++)
                {
                    result.Add((T)list[i]);
                }
                return result;
            }
            return default;
        }

        public T[][] GetCrossArray<T>(string key)
        {
            if (CheckKey(key))
            {
                ArrayList list = hashtable[key] as ArrayList;
                T[][] result = new T[list.Count][];
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList subList = list[i] as ArrayList;
                    T[] subArr = new T[subList.Count];
                    for (int j = 0; j < subList.Count; j++)
                    {
                        subArr[j] = (T)subList[j];
                    }
                    result[i] = subArr;
                }
                return result;
            }

            return default;
        }

        public List<List<T>> GetCrossList<T>(string key)
        {
            if (CheckKey(key))
            {
                ArrayList list = hashtable[key] as ArrayList;
                List<List<T>> result = new List<List<T>>();
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList subList = list[i] as ArrayList;
                    List<T> subArr = new List<T>();
                    for (int j = 0; j < subList.Count; j++)
                    {
                        subArr.Add((T)subList[j]);
                    }
                    result.Add(subArr);
                }
                return result;
            }

            return default;
        }

        // 下列方法方便测试用
        public void SetValue(string key, object value)
        {
            if (hashtable.ContainsKey(key))
            {
                hashtable[key] = value;
                return;
            }
            hashtable.Add(key, value);
        }

        private bool CheckKey(string key)
        {
            bool hasValue = hashtable.ContainsKey(key);
            if (!hasValue)
            {
                WLDebug.LogError(string.Format("MsgData.CheckKey: 不存在key【{0}】", key));
            }
            return hasValue;
        }

        public override string ToString()
        {
            string str = "";
            foreach (var key in hashtable.Keys)
            {
                str += key;
                str += ":";
                str += hashtable[key];
                str += "\n";
            }
            return str;
        }
    }

}
