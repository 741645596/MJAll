// @Author: tanjinhua
// @Date: 2021/12/18  10:26

namespace Common
{
    public abstract class InitialRecord : BaseRecord
    {

        private GameData _gameData;
        private GameAvatarController _avatarController;

        public override void OnInitialize()
        {
            _gameData = stage.gameData as GameData;

            _avatarController = stage.GetController<GameAvatarController>();
        }

        public override float Execute()
        {
            // 显示头像
            _gameData.TraversePlayer(p =>
            {
                int viewChairId = stage.ToViewChairId(p.chairId);

                _avatarController.Append(viewChairId, p.gender, p.avatarUrl, p.userId);

                _avatarController.UpdateScore(viewChairId);
            });

            // TODO: 显示朋友场信息气泡

            return 0f;
        }
    }
}
