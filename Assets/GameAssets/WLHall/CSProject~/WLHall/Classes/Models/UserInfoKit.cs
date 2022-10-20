// @ UserInfoKit.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/31 星期三 17:44:53
// @ Description :

using System;
using System.Collections.Generic;
using UnityEngine;
using WLCore;

namespace WLHall
{
    public static class UserInfoKit
    {
        //public static ClothesModel shopModel = new ClothesModel();
        public static Dictionary<uint, uint> proplist = new Dictionary<uint, uint>();

        private static UserInfo _userInfo;
        public static UserInfo UserInfo
        {
            get
            {
                // 未领取过新手礼包的 默认给它加5000
                if (!HasGotNewbieMoney() && _userInfo != null)
                {
                    _userInfo.money += 5000;
                }
                return _userInfo;
            }
            internal set { }
        }
        private static MsgSystemTime m_vipEnd;


        // extra datas
        public static void DeserializeUserInfo(MsgHeader msg)
        {
            _userInfo = UserInfo.FromEx(msg);

            // 抛出用户信息更新消息
            //NotificationCenter.PostNotification(WLEvent.HALL_UPDATE_USER_DATA, _userInfo);
        }

        public static uint GetSelfID()
        {
            return _userInfo.id;
        }

        public static int GetSex()
        {
            if (_userInfo != null)
            {
                return _userInfo.sex;
            }
            return 0;
        }

        public static void SetSex(int sex)
        {
            _userInfo.sex = sex == 0 ? 0 : 1; // 大厅服务器可能会下发一个非 0 1 的值，需要做一下兼容
        }

        public static long GetMoney()
        {
            return _userInfo.money;
        }

        public static long GetBankMoney()
        {
            return _userInfo.bankMoney;
        }

        public static long GetAllMoney()
        {
            return _userInfo.money + _userInfo.bankMoney;
        }

        public static void AddMoney(long num)
        {
            _userInfo.money += num;
        }

        public static void AddBankMoney(long num)
        {
            _userInfo.bankMoney += num;
        }

        public static string GetNickName()
        {
            if (_userInfo != null)
            {
                return _userInfo.nickName;
            }
            return "";
        }

        public static long GetDiamonds()
        {
            if (_userInfo != null)
            {
                return Convert.ToInt64(_userInfo.xzMoney);
            }

            return 0;
        }

        public static bool HasGotNewbieMoney()
        { 
            return true; //TODO cfx
            //            if (!this.isUseNewVersionNewbieGift())
            //            {
            //                // 不能使用新版本新手礼包，当做已经领取过处理
            //                return true;
            //            }
            //
            //            if (this.isAllGameCountEnough(1))
            //            {
            //                // 已经玩过游戏，说明是老用户，完全不给奖励
            //                return true;
            //            }
            //
            //            if (EffortDataKit.GetMapEffortCompleteCount(MISSION_EFFORT_NEWBIE_MONEY_APP) != 0)
            //            {
            //                // 已经领取过了
            //                return true;
            //            }
            //
            //            if (EffortDataKit.GetMapEffortCompleteCount(MISSION_EFFORT_NEWBIE_MONEY_MINI) != 0)
            //            {
            //                // 已经领取过了
            //                return true;
            //            }

            //return false;
        }

        public static long GetHandleMoneyStep()
        {
            // 新版本新用户 如果未领取到小游戏2000/app5000虚拟值时 特殊处理
            if (!HasGotNewbieMoney())
            {
                // 新手玩家为玩家身上的携带的豆豆(此条件成立时玩家携带的豆豆包含小游戏2000/app5000虚拟值)向上取整+1000 确保玩家存不了即可
                return Convert.ToInt64(Mathf.Ceil((UserInfoKit.GetMoney()) / 1000) * 1000) + 1000;
            }

            //1.携带+背包的5%为存取单位，最小单位为2000，零头就余算，2000~1万单位为整千，1万以上单位为整万；
            long totleBeans = UserInfoKit.GetAllMoney();
            float step = Convert.ToSingle(totleBeans) * 10 * 0.01f;
            if (step < 2000)
            {
                return 2000;
            }
            else if (step >= 2000 && step < 10000)
            {
                return Convert.ToInt64(Mathf.Floor(step / 1000) * 1000);
            }
            else
            {
                return Convert.ToInt64(Mathf.Floor(step / 10000) * 10000);
            }
        }

        public static Tuple<uint, uint> UpdateUserInfo(MsgHeader msg)
        {
            _userInfo.money = msg.ReadInt64();
            _userInfo.bankMoney = msg.ReadInt64();
            _userInfo.lottery = msg.ReadUint32();
            if (msg.len > msg.position)
            { 
                // 后面还有额外的道具更新
                uint dwValue = 0;
                uint dwPropID = msg.ReadDword();
                if (dwPropID == Constants.PROP_ID_VIP)
                {
                    m_vipEnd = MsgSystemTime.From(msg);
                    dwValue = msg.ReadDword();
                    if (dwValue != 0)
                    {
                        _userInfo.vipValue = dwValue;
                    }
                    else
                    {
                        dwValue = 0;
                    }

                    if (_userInfo.userType == Constants.USER_TYPE_NORMAL)
                    {
                        _userInfo.userType = Constants.USER_TYPE_VIP;
                    }
                }
                else if (dwPropID == Constants.PROP_ID_XZMONEY)
                {
                    dwValue = msg.ReadDword();
                    _userInfo.xzMoney = dwValue;
                }
                //else if (dwPropID == Constants.PROP_ID_PRESTIGE) // 处理警告
                //{
                //    dwValue = msg.ReadDword();
                //    _userInfo.clothID = dwValue;
                //}
                else
                {
                    dwValue = msg.ReadDword();
                }
                return Tuple.Create(dwPropID, dwValue);
            }
            return null;
        }

        internal static void DeserializePropList(MsgHeader msg)
        {
            var dwPropCount = msg.ReadDword();

            proplist = new Dictionary<uint, uint>();
            for (var i = 0; i < dwPropCount; i++)
            {
                var dwID = msg.ReadDword();
                var dwCount = msg.ReadDword();
                proplist.Add(dwID, dwCount);
            }
        }
    }
}
