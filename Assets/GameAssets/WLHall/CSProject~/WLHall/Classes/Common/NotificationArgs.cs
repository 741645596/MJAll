using System;
using WLCore.Generic;

namespace WeiLe.Notification
{
    public class NotificationArgs : EventArgs
    {
        public CommonArray data;

        public NotificationArgs(params object[] data)
        {
            this.data = new CommonArray(data);
        }

        public string GetString(int index, string defaultValue = "")
        {
            return data.GetString(index, defaultValue);
        }

        public int GetInt(int index, int defaultValue = 0)
        {
            return data.GetInt(index, defaultValue);
        }

        public float GetFloat(int index, float defaultValue = 0f)
        {
            return data.GetFloat(index, defaultValue);
        }

        public object GetObject(int index)
        {
            return data.GetObject(index);
        }

        public bool GetBool(int index, bool defaultValue = false)
        {
            return data.GetBool(index, defaultValue);
        }
    }
}
