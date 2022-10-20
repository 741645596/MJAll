using System;
using Unity.Utility;

namespace WLHall
{

    public class BasePlayer
    {
        /// <summary>
        /// 服务器座位号
        /// </summary>
        public ushort chairId
        {
            get => _userGameInfo.wChairID;
            set => _userGameInfo.wChairID = value;
        }


        /// <summary>
        /// 桌子ID
        /// </summary>
        public ushort deskId
        {
            get => _userGameInfo.wDeskID;
            set => _userGameInfo.wDeskID = value;
        }


        /// <summary>
        /// 用户ID
        /// </summary>
        public uint userId => _userInfo.id;


        /// <summary>
        /// 性别
        /// </summary>
        public int gender => _userInfo.sex;


        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string nickname => _userInfo.nickName;


        /// <summary>
        /// 头像地址
        /// </summary>
        public string avatarUrl => _userInfo.avatarPath;


        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo userInfo
        {
            get => _userInfo;
            set => _userInfo = value;
        }


        /// <summary>
        /// 游戏信息
        /// </summary>
        public UserGameInfo userGameInfo
        {
            get => _userGameInfo;
            set => _userGameInfo = value;
        }


        /// <summary>
        /// 是否已经离开房间
        /// </summary>
        public bool left { get; set; } = false;



        private UserGameInfo _userGameInfo;
        private UserInfo _userInfo;
        private object _userData;


        public uint GetID()
        {
            return _userInfo.id;
        }


        public ushort GetChair()
        {
            return chairId;
        }


        // 是否玩家自己
        public bool IsMe()
        {
            // todo xzh
            return true;
            //return GetID() == UserInfoKit.GetSelfID();
        }


        /**
         * 获得昵称
         *
         * @returns utf16编码昵称
         */
        public string GetNickNameUtf16()
        {
            return _userInfo.nickName;
        }


        /**
         * 获得昵称
         *
         *  @returns utf8编码昵称
         */
        public string GetNickName()
        {
            return EncodeHelper.Utf16ToUtf8(GetNickNameUtf16());
        }


        /**
         * 获取玩家比赛夺冠次数
         *
         * @returns 获取玩家比赛夺冠次数
         */
        public uint GetPKKempCount()
        {
            return _userGameInfo.dwPKKempCount;
        }


        /**
         * 获取玩家比赛获奖次数
         *
         * @returns 获取玩家比赛获奖次数
         */
        public uint GetPKAwardCount()
        {
            return _userGameInfo.dwPKAwardCount;
        }


        /**
         * 获取玩家参与比赛次数
         *
         * @returns 获取玩家参与比赛次数
         */
        public uint GetPKCount()
        {
            return _userGameInfo.dwPKCount;
        }


        /**
         * 获取玩家比赛赢局数
         *
         * @returns 比赛赢局数
         */
        public uint GetPKWinCount()
        {
            return _userGameInfo.dwPKWinCount;
        }


        /**
         * 获取玩家比赛淘汰次数
         *
         * @returns 比赛淘汰次数
         */
        public uint GetPKEliminate()
        {
            return _userGameInfo.dwPKEliminate;
        }


        /**
         * 设置游戏自定义数据1
         *
         * @param {*} num
         */
        public void SetGameData1(int num)
        {
            _userGameInfo.nGameData1 = num;
        }


        /**
         * 游戏自定义数据1
         *
         * @returns 游戏自定义数据1
         */
        public int GetGameData1()
        {
            return _userGameInfo.nGameData1;
        }


        /**
         * 设置游戏自定义数据2
         *
         * @param {*} num
         */
        public void SetGameData2(int num)
        {
            _userGameInfo.nGameData2 = num;
        }


        /**
         * 游戏自定义数据2
         *
         * @returns 游戏自定义数据2
         */
        public int GetGameData2()
        {
            return _userGameInfo.nGameData2;
        }


        /**
         * 获得用户性别
         *
         * @returns 1:男； 0:女
         */
        public int GetPlayerSex()
        {
            return _userInfo.sex;
        }


        /**
         * 获取用户头像ID
         *
         * @returns 头像id
         */
        public uint GetAvatarID()
        {
            return _userInfo.avatarId;
        }


        /**
         * 获取用户时装ID
         *
         * @returns 时装ID
         */
        public uint GetClothID()
        {
            return _userInfo.prestige;
        }


        /**
         * 获取用户VIP成长经验值
         *
         * @returns 经验值
         */
        public uint GetVIPValue()
        {
            return _userInfo.vipValue;
        }


        /**
         * 获取玩家禁言限制
         *
         * @returns 禁言限制
         */
        public int GetChatLimit()
        {
            return _userInfo.chatLimit;
        }


        /**
         * 获取玩家疲劳值, 防沉迷
         *
         * @returns 疲劳值
         */
        public uint GetTireTime()
        {
            return _userInfo.tireTime;
        }


        /**
         * 获得头像路径
         *
         * @returns 头像路径
         */
        public string GetAvatarPath()
        {
            return _userInfo.avatarPath;
        }


        /**
         * 返回玩家是否是旁观状态
         *
         * @returns 是否旁观
         */
        public bool IsLookOn()
        {
            return _userGameInfo.byLookOn != 0;
        }


        /**
         * 设置玩家是否是旁观状态
         *
         * @param {any} bLookOn
         */
        public void SetLookOn(byte bLookOn)
        {
            _userGameInfo.byLookOn = bLookOn;
        }


        /**
         * 获得用户声望值
         *
         * @returns 声望值
         */
        public uint GetPrestige()
        {
            return _userInfo.dwPrestige;    // 用服装编号发送声望值，原本大厅就是为了兼容考虑的，照做就是
        }


        // 获取银行豆豆数量
        public long GetBankMoney()
        {
            return _userInfo.bankMoney;
        }


        // 获取钻石数量
        public uint GetXZMoney()
        {
            return _userInfo.xzMoney;
        }


        // 获取元宝数量
        public uint GetLottery()
        {
            return _userInfo.lottery;
        }


        // 获取用户类型
        public byte GetUserType()
        {
            return _userInfo.userType;
        }


        /**
         * 获得用户金钱数
         *
         * @returns 64 位整数
         */
        public long GetMoney()
        {
            return _userInfo.money;
        }


        /**
         * 设置玩家豆豆数量
         *
         * @param {*} num 豆豆数量
         */
        public void SetMoney(long num)
        {
            _userInfo.money = num;
        }


        /**
         * 判断用户是否是VIP
         *
         * @returns true VIP，false 不是 VIP
         */
        public bool IsVIP()
        {
            // todo xzh
            return false;
            // return userInfo.userFrom >= (int)USER_TYPE.USER_TYPE_NORMAL;
        }


        /**
         * 判断用户是否准备状态
         *
         */
        public bool IsReady()
        {
            return _userGameInfo.byReady == 1;
        }


        /**
         * 设置玩家是否准备
         *
         * @param {any} bReady
         */
        public void SetReady(byte bReady)
        {
            _userGameInfo.byReady = bReady;
        }


        public void SetReady(bool isReady)
        {
            byte r = (byte)(isReady ? 1 : 0);
            SetReady(r);
        }


        /**
         * 获得用户积分值
         *
         * @returns 64 位整数
         */
        public long GetScore()
        {
            return _userGameInfo.llScore;
        }


        /**
         * 设置玩家积分
         *
         * @returns 64 位整数
         */
        public void SetScore(long num)
        {
            _userGameInfo.llScore = num;
        }


        /**
         * 获得用户总游戏局数
         *
         */
        public uint GetGameCount()
        {
            return GetWinCount() + GetLostCount() + GetDrawCount() + GetFleeCount();
        }


        public double GetWinRate()
        {
            var winCount = GetWinCount();
            var totleCount = GetGameCount();
            if (totleCount == 0)
            {
                return 0;
            }
            else
            {
                // 胜率 = 赢局 / 总局数
                return Math.Floor((double)(winCount / totleCount * 100));
            }
        }


        /**
         * 获得用户赢局数
         *
         * @returns 32 位整数
         */
        public uint GetWinCount()
        {
            return _userGameInfo.dwWinCount;
        }


        /**
         * 获取玩家胜局数
         *
         * @param {*} num 32 位整数
         */
        public void SetWinCount(uint num)
        {
            _userGameInfo.dwWinCount = num;
        }


        /**
         * 获得用户输局数
         *
         * @returns 32 位整数
         */
        public uint GetLostCount()
        {
            return _userGameInfo.dwLostCount;
        }


        /**
         * 设置玩家输局数
         *
         * @param {*} num 32 位整数
         */
        public void SetLostCount(uint num)
        {
            _userGameInfo.dwLostCount = num;
        }


        /**
         * 获得用户平局数
         *
         * @returns 32 位整数
         */
        public uint GetDrawCount()
        {
            return _userGameInfo.dwDrawCount;
        }


        /**
         * 设置玩家平局数
         *
         * @param {*} num 32 位整数
         */
        public void SetDrawCount(uint num)
        {
            _userGameInfo.dwDrawCount = num;
        }


        /**
         * 获得用户逃跑局数
         *
         * @returns 32 位整数
         */
        public uint GetFleeCount()
        {
            return _userGameInfo.dwFleeCount;
        }


        /**
         * 设置玩家逃跑局数
         *
         * @param {*} num 32 位整数
         */
        public void SetFleeCount(uint num)
        {
            _userGameInfo.dwFleeCount = num;
        }


        /**
         * 为用户绑定一个自定义数据
         *
         * @param {any} userdata
         */
        public void SetUserData(object userdata)
        {
            _userData = userdata;
        }


        /**
         * 获得为用户绑定的自定义数据
         *
         * @returns 用户数据对象
         */
        public object GetUserData()
        {
            return _userData;
        }


        /**
         * 设置附带任务数据,该数据在当前游戏内共享
         *
         * @param {any} dwKey 数据索引,目前索引只能是[0,31]之间
         * @param {any} dwValue 为索引指定的值
         */
        public void SetTaskData(uint dwKey, byte dwValue)
        {
            _userGameInfo.cbTaskData[dwKey] = dwValue;
        }


        /**
         * 获得附带任务数据,该数据在当前游戏内共享
         *
         * @param {any} dwKey 数据索引,目前索引只能是[0,31]之间
         * @returns 返回指定的任务数据值
         */
        public byte GetTaskData(uint dwKey)
        {
            return _userGameInfo.cbTaskData[dwKey];
        }


        /**
         * 设置附带任务数据,该数据在当前游戏内共享
         *
         * @param {any} dwIndex 数据索引,目前索引只能是[0,127]之间
         * @param {any} cbValue 为索引指定的值
         */
        public void SetTaskByte(uint dwIndex, byte cbValue)
        {
            _userGameInfo.cbTaskData[dwIndex] = cbValue;
        }


        /**
         * 获得附带任务数据,该数据在当前游戏内共享
         *
         * @param {any} dwIndex 数据索引,目前索引只能是[0,127]之间
         * @returns 返回指定的任务数据值
         */
        public byte GetTaskByte(uint dwIndex)
        {
            return _userGameInfo.cbTaskData[dwIndex];
        }


        public byte[] GetTaskFullByte()
        {
            return _userGameInfo.cbTaskData;
        }


        /**
         * 设置比赛数据附带数据,该数据在当前游戏内共享(每月清理一次
         *
         * @param {any} dwIndex 数据索引，目前索引只能在[0,15]之间
         * @param {any} dwValue 设置新的数据
         */
        public void SetMatchData(uint dwIndex, byte dwValue)
        {
            _userGameInfo.cbMatchData[dwIndex] = dwValue;
        }


        /**
         * 获得比赛数据附带数据,该数据在当前游戏内共享（每月清理一次)
         *
         * @param {any} dwIndex 数据索引，目前索引只能在[0,31]之间,其中[16,31]为底层使用
         * @returns
         */
        public byte GetMatchData(uint dwIndex)
        {
            return _userGameInfo.cbMatchData[dwIndex];
        }


        /**
         * 设置比赛数据附带数据,该数据在当前游戏内共享(每月清理一次)
         *
         * @param {any} dwIndex 数据索引，目前索引只能在[0,127]之间
         * @param {any} cbValue 设置新的数据
         */
        public void SetMatchByte(uint dwIndex, byte cbValue)
        {
            _userGameInfo.cbMatchData[dwIndex] = cbValue;
        }


        /**
         * 获得比赛数据附带数据,该数据在当前游戏内共享（每月清理一次)
         *
         * @param {any} dwIndex 数据索引，目前索引只能在[0,127]之间,其中[64,127]为底层使用
         * @returns 返回指定的比赛数据
         */
        public byte GetMatchByte(uint dwIndex)
        {
            return _userGameInfo.cbMatchData[dwIndex];
        }


        /**
         * 获得用户基本信息
         *
         */
        public UserInfo GetUserInfo()
        {
            return _userInfo;
        }


        /**
         * 设置用户基本信息
         *
         */
        public void SetUserInfo(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }


        /**
         * 获得用户游戏信息
         *
         */
        public UserGameInfo GetUserGameInfo()
        {
            return _userGameInfo;
        }


        /**
         * 设置用户游戏信息
         *
         */
        public void SetUserGameInfo(UserGameInfo userGameInfo)
        {
            _userGameInfo = userGameInfo;
        }
    }
}
