// @Author: tanjinhua
// @Date: 2021/4/15  11:36


using System.Collections.Generic;
using Unity.Widget;

namespace MJCommon
{
    public class MJActionEffect : WNode
    {
        public static KeyValuePair<string, string> GetDefaultConfig(int showType)
        {
            switch(showType)
            {
                case ActionShowType.Peng:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_tishi", "peng_tishi_h_01_01.prefab");

                case ActionShowType.Gang:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_tishi", "gang_tishi_h_01_01.prefab");

                case ActionShowType.Hu:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_tishi", "hu_tishi_h_01_01 .prefab");

                case ActionShowType.Zimo:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_tishi", "zimo_duifang_tishi_h_01_01.prefab");
                default:
                    WLDebug.LogWarning($"MJActionEffect.GetDefaultConfig: 通用不支持showyType = {showType}的动作特效");
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_tmp", "eff_chi.prefab");
            }
        }

        public string identifier { get; private set; }

        public MJActionEffect(string asset, string key)
        {
            InitGameObject(asset, key);

            identifier = asset + key;
        }
    }
}
