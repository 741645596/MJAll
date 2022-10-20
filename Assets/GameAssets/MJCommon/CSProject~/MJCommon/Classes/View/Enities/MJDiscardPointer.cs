// @Author: tanjinhua
// @Date: 2021/3/25  16:15


using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{

    /// <summary>
    /// 出牌指示器
    /// </summary>
    public class MJDiscardPointer : BaseEntity
    {
        protected override GameObject CreateGameObject()
        {
            return ObjectHelper.Instantiate("MJCommon/MJ/mj_zm_effe_tishi", "tishi_x_01_01.prefab");
        }
    }
}
