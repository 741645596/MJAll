// @Author: tanjinhua
// @Date: 2021/12/14  19:06


using System;
using System.Collections.Generic;
using LitJson;
using WLCore;
using WLHall;
using WLHall.Game;

namespace Common
{
    public abstract class ReplayStage : BaseGameStage
    {
        /// <summary>
        /// 总局数
        /// </summary>
        public int totalGameCount { get; private set; }

        /// <summary>
        /// 回放信息
        /// </summary>
        public RecordInfo recordInfo { get; private set; }

        /// <summary>
        /// 初始化回放记录ID, 支持重写自定义
        /// </summary>
        protected virtual int initialRecordId => 0;

        /// <summary>
        /// 结束回放记录ID，支持重写自定义
        /// </summary>
        protected virtual int endRecordId => 6;


        private ReplayController _replayController;
        private Dictionary<int, string> _recordTypeMap;
        private bool _isShutdown;


        public ReplayStage(RecordInfo info) : base(new ReplayRoom(info))
        {
            recordInfo = info;

            isReplay = true;

            RegisterRecords();
        }


        /// <summary>
        /// 注册回放记录与id的映射关系
        /// </summary>
        protected abstract void RegisterRecords();


        /// <summary>
        /// 实例化后触发
        /// </summary>
        public override void OnInitialize()
        {
            base.OnInitialize();

            InitGamePlayers();

            RequestGameCount();

            // order传999确保ReplayController.OnSceneLoaded是最后一个响应
            _replayController = AddController<ReplayController>(999);

            AddController<GameSettingController>();
        }

        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            _replayController?.Update(deltaTime);
        }


        /// <summary>
        /// 退出场景时触发
        /// </summary>
        public override void OnShutdown()
        {
            base.OnShutdown();

            _isShutdown = true;

            _recordTypeMap?.Clear();
        }


        /// <summary>
        /// 请求单局数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="callback"></param>
        public void RequestRecordData(int index, Action callback = null)
        {
            ApiHelper.RequestRecordDataStream(recordInfo.hashcode, index, (buffer, code) =>
            {
                if (_isShutdown)
                {
                    return;
                }

                if (code != 200)
                {
                    WLDebug.LogWarning($"ReplayStage.RequestOneGame: 请求第{index}局回放数据失败，错误码：{code}");
                    return;
                }

                var msg = new MsgHeader(buffer);

                var records = ParseRecords(msg);

                _replayController?.SetRecords(records);

                callback?.Invoke();
            });
        }


        /// <summary>
        /// 注册记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        protected void RegisterRecord<T>(int id) where T : BaseRecord, new()
        {
            if (_recordTypeMap == null)
            {
                _recordTypeMap = new Dictionary<int, string>();
            }

            var type = typeof(T);

            _recordTypeMap[id] = type.FullName;
        }


        private void RequestGameCount()
        {
            ApiHelper.RequestRecordGameCount(recordInfo.hashcode, (json, code) =>
            {
                if (_isShutdown)
                {
                    return;
                }

                if (code != 200)
                {
                    WLDebug.LogWarning($"ReplayStage.StartRequest: 请求回放局数失败，错误码：{code}");
                    return;
                }

                totalGameCount = (int)JsonMapper.ToObject(json)["total"];

                // 进入回放时默认请求第一局数据，返回后加载场景
                RequestRecordData(0, () => LoadScene());
            });
        }


        private List<BaseRecord> ParseRecords(MsgHeader msg)
        {
            if (_recordTypeMap == null || !_recordTypeMap.ContainsKey(initialRecordId) || string.IsNullOrEmpty(_recordTypeMap[initialRecordId]))
            {
                WLDebug.LogWarning("ReplayStage.ParseRecords: 解析回放数据失败，未注册初始化记录");
                return new List<BaseRecord>();
            }

            var initializeRecord = CreateRecord(_recordTypeMap[initialRecordId], 0);
            initializeRecord.Read(msg);
            var records = new List<BaseRecord>
            {
                initializeRecord
            };

            int index = 1;
            int recordId = -1;
            while(recordId != endRecordId)
            {
                recordId = msg.ReadByte();
                if (recordId == initialRecordId)
                {
                    WLDebug.LogWarning("ReplayStage.ParseRecords: 解析回放数据失败，单局回放数据存在2个InitializeRecord。请检查服务器逻辑");
                    return records;
                }

                if (!_recordTypeMap.ContainsKey(recordId) || string.IsNullOrEmpty(_recordTypeMap[recordId]))
                {
                    WLDebug.LogWarning($"ReplayStage.ParseRecords: 解析回放数据失败, 未找到recordId = {recordId}对应的记录类");
                    return records;
                }

                var record = CreateRecord(_recordTypeMap[recordId], index);

                record.Read(msg);

                records.Add(record);

                index++;
            }
            return records;
        }


        private BaseRecord CreateRecord(string typeFullName, int index)
        {
            var record = AppDomainManager.Instantiate<BaseRecord>(typeFullName);

            record.stage = this;

            record.index = index;

            record.OnInitialize();

            return record;
        }


        private void InitGamePlayers()
        {
            for (ushort i = 0; i < recordInfo.players.Length; i++)
            {
                var playerInfo = recordInfo.players[i];
                var gamePlayer = gameData.OnCreatePlayer();
                gamePlayer.userInfo = new UserInfo
                {
                    id = playerInfo.user_id,
                    nickName = playerInfo.nickname,
                    avatarPath = playerInfo.avatar,
                    sex = playerInfo.sex
                };
                gamePlayer.userGameInfo = new UserGameInfo
                {
                    wChairID = i
                };
                gameData.SetPlayer(gamePlayer);
            }
        }
    }
}
