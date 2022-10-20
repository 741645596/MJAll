using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeiLe.Notification
{
    public static class NotificationCenter
    {
        private static readonly Dictionary<string, List<object>> _notiMgr = new Dictionary<string, List<object>>();
        private static readonly List<object> _toRemoveList = new List<object>();

        /// <summary>
        /// 通知模式：注册通知，所有AddNotification必须有RemoveNotification对应，否则会报错
        /// 通知模式性能不是太好，请勿用在一些频繁调用的场景上
        /// </summary>
        /// <param name="target"> 任意C#对象 </param>
        /// <param name="notiName"> 注册的C#对象必须要有notiName相应的方法名 </param>
        /// 示例：notiName = "NotificiFun"，则需要声明函数 public void NotificiFun(NotificationArgs e)
        public static void AddNotification(object target, string notiName)
        {
            if (_notiMgr.ContainsKey(notiName) == false)
            {
                _notiMgr[notiName] = new List<object>();
            }

            if (_notiMgr[notiName].Contains(target))
            {
                string msg = string.Format("{0} 重复注册了通知 {1}", target, notiName);
                WLDebug.LogWarning(msg);
                return;
            }

            _notiMgr[notiName].Add(target);
        }

        /// <summary>
        /// 抛通知
        /// </summary>
        /// <param name="notiName"> 与上面方法名参数名称一致 </param>
        /// <param name="data"> 参数数据，是个可变参数，可以不传 </param>
        public static void PostNotification(string notiName, params object[] data)
        {
            _CleanNotification();

            if (_notiMgr.ContainsKey(notiName))
            {
                var list = _notiMgr[notiName];
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item == null)
                    {
                        continue;
                    }
                    Type t = item.GetType();
                    if (t == null)
                    {
                        continue;
                    }

                    MethodInfo mt = t.GetMethod(notiName, BindingFlags.InvokeMethod); //加载方法
                    if (mt != null)
                    {
                        mt.Invoke(item, BindingFlags.InvokeMethod, null, new object[] { new NotificationArgs(data) }, null);
                        continue;
                    }

                    bool isDone = false;
                    while (t.BaseType != null)
                    {
                        t = t.BaseType;
                        mt = t.GetMethod(notiName, BindingFlags.InvokeMethod);
                        //加载父类方法
                        if (mt != null)
                        {
                            mt.Invoke(item, BindingFlags.InvokeMethod, null, new object[] { new NotificationArgs(data) }, null);
                            isDone = true;
                            break;
                        }
                    }

                    if (isDone == false)
                    {
                        WLDebug.LogWarning($"通知模式未找到消息：{t.Name}.{notiName}");
                    }
                }
            }
        }

        /// <summary>
        /// 删通知
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveNotification(object target)
        {
            // 遍历时直接删除有问题，所以先记录后再删
            _toRemoveList.Add(target);
        }

        /// <summary>
        /// 删除指定对象的指定通知
        /// </summary>
        /// <param name="target"></param>
        /// <param name="notiName"></param>
        public static void RemoveNotificationByName(object target, string notiName)
        {
            if (!_notiMgr.ContainsKey(notiName))
            {
                return;
            }

            List<object> list = _notiMgr[notiName];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (target == list[i])
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        private static void _CleanNotification()
        {
            foreach (var target in _toRemoveList)
            {
                foreach (List<object> list in _notiMgr.Values)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (target == list[i])
                        {
                            list.Remove(list[i]);
                        }
                    }
                }
            }

            _toRemoveList.Clear();
        }
    }
}
