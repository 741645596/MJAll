// @Author: tanjinhua
// @Date: 2021/8/25  10:49

using LitJson;
using Unity.Core;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJSpaceController : BaseGameController
    {
        private MJSpaceConfig _spaceConfig;
        private MJSpace _space;

        public override void OnSceneLoaded()
        {
            _spaceConfig = OnReadConfig();

            _space = OnCreateSpace(_spaceConfig);
        }

        public MJSpace GetSpace()
        {
            return _space;
        }

        public MJSpaceConfig GetSpaceConfig()
        {
            return _spaceConfig;
        }

        protected virtual MJSpaceConfig OnReadConfig()
        {
            var asset = AssetsManager.Load<TextAsset>("MJCommon/MJ/mj_config", "space.json");

            return JsonMapper.ToObject<MJSpaceConfig>(asset.text);
        }

        protected virtual MJSpace OnCreateSpace(MJSpaceConfig config)
        {
            return new MJSpace(config);
        }
    }
}
