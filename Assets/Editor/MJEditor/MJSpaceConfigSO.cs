// MJSpaceConfigOS.cs
// Author: shihongyang shihongyang@Unity.com
// Data: 2021/8/5

using UnityEngine;

namespace MJEditor
{
    [CreateAssetMenu(menuName = "Assets/Create MJSpace Config")]
    public class MJSpaceConfigSO : ScriptableObject
    {
        [SerializeField]
        public MJWallConfig wallConfig;
        [SerializeField]
        public MJDeskCardConfig deskConfig;
        [SerializeField]
        public MJMeldConfig meldConfig;
        [SerializeField]
        public MJHandSetConfig handConfig;
        [SerializeField]
        public MJWinSetConfig winSetConfig;


        public void LoadFromJson(string json)
        {
            var data = LitJson.JsonMapper.ToObject(json);
            wallConfig = LitJson.JsonMapper.ToObject<MJWallConfig>(data["wallConfig"].ToJson());
            deskConfig = LitJson.JsonMapper.ToObject<MJDeskCardConfig>(data["deskConfig"].ToJson());
            meldConfig = LitJson.JsonMapper.ToObject<MJMeldConfig>(data["meldConfig"].ToJson());
            handConfig = LitJson.JsonMapper.ToObject<MJHandSetConfig>(data["handConfig"].ToJson());
            winSetConfig = LitJson.JsonMapper.ToObject<MJWinSetConfig>(data["winSetConfig"].ToJson());
        }
    }

}
//49 44 40 36 32 28 24 20 16 12 8 4