// @Author: tanjinhua
// @Date: 2021/8/20  11:41


using System;
using UnityEngine;
using WLCore.Entity;
using System.Collections.Generic;

namespace MJCommon
{
    /// <summary>
    /// 麻将桌
    /// </summary>
    public class MJTable : BaseEntity
    {
        /// <summary>
        /// 视图座位号0的方位，即自家方位
        /// </summary>
        public Wind startWind
        {
            get
            {
                return _startWin;
            }
            set
            {
                if (_startWin == value)
                {
                    return;
                }

                _startWin = value;

                _directionPointer.localEulerAngles = GetDiceMachineEluer(value);
            }
        }

        /// <summary>
        /// 升降器
        /// </summary>
        public Transform lifter;


        private MJCountDown _countdown;
        private GameObject _diceCap;
        private MJDice _dice;
        private Transform _directionPointer;
        private Dictionary<Wind, Transform> _winLights;
        private Wind _startWin = Wind.South;


        public MJTable()
        {
            gameObject.name = "MJTable";

            _countdown = new MJCountDown();
            _countdown.gameObject.transform.SetParent(gameObject.transform, false);
            _countdown.SetActive(false);

            var trs = gameObject.transform;
            _directionPointer = trs.FindReference("directions");
            _winLights = new Dictionary<Wind, Transform>();
            _winLights.Add(Wind.East, trs.FindReference("east_light"));
            _winLights.Add(Wind.South, trs.FindReference("south_light"));
            _winLights.Add(Wind.West, trs.FindReference("west_light"));
            _winLights.Add(Wind.North, trs.FindReference("north_light"));
        }

        /// <summary>
        /// 开始计时器
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="onTimeout"></param>
        public void StartTimer(int timeout, Action<int> onTick = null)
        {
            _diceCap.SetActive(true);

            _countdown.SetActive(true);

            _dice?.Destroy();

            _countdown.onTick = t => onTick?.Invoke(timeout - t);

            _countdown.StartCountdown(timeout);
        }


        /// <summary>
        /// 停止计时器
        /// </summary>
        public void StopTimer()
        {
            _countdown.StopCountdown();

            _countdown.onTick = null;

            _countdown.SetActive(false);
        }


        /// <summary>
        /// 播放骰子动画
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void PlayDice(int v1, int v2)
        {
            if (_dice != null && _dice.gameObject != null)
            {
                _dice.Destroy();
            }

            _countdown.SetActive(false);

            _diceCap.gameObject.SetActive(false);

            _dice = new MJDice(new List<int> { v1, v2 });

            MJAnimationHelper.PlayDiceAnimation(_dice);
        }

        /// <summary>
        /// 同上，一个骰子的动画
        /// </summary>
        /// <param name="v"></param>
        public void PlayDice(int v)
        {
            if (_dice != null && _dice.gameObject != null)
            {
                _dice.Destroy();
            }

            _countdown.SetActive(false);

            _diceCap.gameObject.SetActive(false);

            _dice = new MJDice(new List<int> { v });

            MJAnimationHelper.PlayDiceAnimation(_dice);
        }

        /// <summary>
        /// 暂停倒计时
        /// </summary>
        public void PauseTimer()
        {
        }


        /// <summary>
        /// 恢复倒计时
        /// </summary>
        public void ResumeTimer()
        {
        }


        /// <summary>
        /// 播放指向动画
        /// </summary>
        /// <param name="viewChairId"></param>
        public void PlayPointing(int viewChairId)
        {
            SetAllDirectionLightActive(false);

            Wind dir = (Wind)(((int)_startWin + viewChairId) % 4);

            _winLights[dir].gameObject.SetActive(true);
        }


        /// <summary>
        /// 停止播放指向动画
        /// </summary>
        public void StopPointing()
        {
            SetAllDirectionLightActive(false);
        }

        private Vector3 GetDiceMachineEluer(Wind selfDirection)
        {
            switch (selfDirection)
            {
                case Wind.South:
                    return Vector3.zero;
                case Wind.West:
                    return new Vector3(0, 90, 0);
                case Wind.North:
                    return new Vector3(0, 180, 0);
                case Wind.East:
                    return new Vector3(0, 270, 0);
            }
            return Vector3.zero;
        }

        private void SetAllDirectionLightActive(bool active)
        {
            foreach(var pair in _winLights)
            {
                pair.Value.gameObject.SetActive(active);
            }
        }


        protected override GameObject CreateGameObject()
        {
            var obj = ObjectHelper.Instantiate("MJCommon/MJ/mj_env", "prefabs/mj_table_01.prefab");

            lifter = obj.transform.Find("mj_table_01/mj_table_01_a");

            _diceCap = obj.transform.Find("dice_machine/timer_cap").gameObject;

            return obj;
        }
    }
}
