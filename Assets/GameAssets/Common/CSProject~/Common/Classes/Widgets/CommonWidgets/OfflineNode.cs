// @Author: lili
// @Date: 2021/5/11  19:12

using UnityEngine;
using Unity.Widget;

namespace Common
{
    /// <summary>
    /// 离线标识
    /// </summary>
    public class OfflineNode : WNode
    {
        //private ArtText m_timeTxt;
        private float _time;
        private bool _isCountDownMode;

        public OfflineNode(float curOfflineTime = 0f)
        {
            //m_timeTxt = FindReference<ArtText>("TimeText");
            //m_isCountDownMode = false;
            //m_time = curOfflineTime;
            //SetTimeText();

            //RegisterScheduler(SchedulerType.Update, Update);
        }

        public void ShowOfflineTime(float offlineTime)
        {
            _isCountDownMode = false;
            _time = offlineTime;
            SetTimeText();
        }

        public void ShowCountDown(float countdown)
        {
            _isCountDownMode = true;
            _time = countdown;
            SetTimeText();
        }

        private void Update()
        {
            if (_isCountDownMode)
            {
                _time -= Time.deltaTime;
                _time = Mathf.Max(0, _time);
            }
            else
            {
                _time += Time.deltaTime;
            }

            SetTimeText();
        }

        private void SetTimeText()
        {
            int countdown = Mathf.CeilToInt(_time);
            int min = Mathf.FloorToInt(countdown / 60);
            int sec = countdown - min * 60;
            string str = string.Format("{0:D2}_{1:D2}", min, sec);
            //m_timeTxt.text = str;
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/CommonWidgets/Offline/OfflineNode.prefab");
        //}
    }
}
