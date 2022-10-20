// @Author: tanjinhua
// @Date: 2021/12/14  20:24

using System.Collections.Generic;
using Common;
using WLHall;
using WLHall.Game;

namespace MJCommon
{
    public class MJReplayStage : ReplayStage
    {

        protected override int initialRecordId => Record.Initial;


        protected override int endRecordId => Record.End;


        public MJReplayStage(RecordInfo recordInfo) : base(recordInfo)
        {
        }

        /// <summary>
        /// 注册回放记录类
        /// </summary>
        protected override void RegisterRecords()
        {
            RegisterRecord<MJInitialRecord>(Record.Initial);
            RegisterRecord<MJOutCardRecord>(Record.OutCard);
            RegisterRecord<MJSendCardRecord>(Record.SendCard);
            RegisterRecord<MJActionEventRecord>(Record.ActionEvent);
            RegisterRecord<MJActionResultRecord>(Record.ActionResult);
            RegisterRecord<MJScrambleActionResultRecord>(Record.ScrambleActionResult);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            AddController<MJSpaceController>();
            AddController<MJEnvController>();
            AddController<MJAvatarController>();
            AddController<MJAudioController>();
            AddController<MJSettleController>();
            AddController<MJTableController>();
            AddController<MJHandController>();
            AddController<MJMeldController>();
            AddController<MJDeskCardController>();
            AddController<MJWallController>();
            AddController<MJWinCardController>();
            AddController<MJActionButtonController>();
            AddController<MJActionEffectController>();
        }


        protected override void OnSceneLoaded()
        {
            PreInitCardPool();

            base.OnSceneLoaded();
        }


        public override void OnShutdown()
        {
            base.OnShutdown();

            MJCardPool.Clear();
        }

        /// <summary>
        /// 返回BuildSetting中对应场景名称
        /// </summary>
        /// <returns></returns>
        protected override string GetBuildSceneName() => "MJScene";


        /// <summary>
        /// 创建游戏数据
        /// </summary>
        /// <returns></returns>
        protected override BaseGameData OnCreateGameData() => new MJGameData();



        #region Utils
        /// <summary>
        /// 服务器座位号转换为以relativeChairId视角为准的视图座位号
        /// </summary>
        /// <param name="serverChairId"></param>
        /// <param name="relativeChairId"></param>
        /// <returns></returns>
        public override int ToViewChairIdRelative(int serverChairId, int relativeChairId)
        {
            int basicViewChairId = base.ToViewChairIdRelative(serverChairId, relativeChairId);

            return MJChairConverter.ToView(gameData.maxPlayerCount, basicViewChairId);
        }


        /// <summary>
        /// 视图座位号转换为服务器座位号
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public override int ToServerChairId(int viewChairId)
        {
            int basicViewChairId = MJChairConverter.ToViewBasic(gameData.maxPlayerCount, viewChairId);

            return base.ToServerChairId(basicViewChairId);
        }


        /// <summary>
        /// 获取下家视图座位号
        /// </summary>
        /// <param name="currentViewChairId"></param>
        /// <returns></returns>
        public override int NextViewChairId(int currentViewChairId)
        {
            return MJChairConverter.ToNextView(gameData.maxPlayerCount, currentViewChairId);
        }
        #endregion


        private void PreInitCardPool()
        {
            var values = new List<int>
            {
                // 万
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,

                // 条
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,

                // 筒
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,

                // 风牌（东南西北）
                0x31, 0x32, 0x33, 0x34,
                0x31, 0x32, 0x33, 0x34,
                0x31, 0x32, 0x33, 0x34,
                0x31, 0x32, 0x33, 0x34,
            };

            values.ForEach(v =>
            {
                MJCardPool.Recycle(new MJCard(v));
                MJCardPool.Recycle(new MJHandCard(v));
            });

            for (int i = 0; i < 144; i++)
            {
                MJCardPool.Recycle(new MJCard(0));
            }
        }
    }
}
