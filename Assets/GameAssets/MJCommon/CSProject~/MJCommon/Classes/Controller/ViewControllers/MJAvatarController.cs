
using System;
using System.Collections.Generic;
using Common;
using Unity.Core;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJAvatarController : GameAvatarController
    {
        protected string bankerIconName = "banker_icon";
        protected string readyHandIconName = "ting_icon";

        private MJGameData _gameData;
        private MJAvatarPanel _panel;
        private MJAvatar[] _avatars;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _panel = OnCreateAvatarPanel();

            _avatars = new MJAvatar[4];

        }

        /// <summary>
        /// 显示庄家小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        public RectTransform ShowBankerIcon(int viewChairId)
        {
            if (!_gameData.showBankerIcon)
            {
                return null;
            }

            var icon = GetBankerIcon(viewChairId);
            if (icon != null)
            {
                return icon;
            }

            var avatar = GetAvatar<MJAvatar>(viewChairId);
            if (avatar != null)
            {
                var newIcon = OnCreateBankerIcon();
                avatar.AddIcon(bankerIconName, newIcon);

                var pos = OnGetBankerIconPos(viewChairId);
                newIcon.SetPositionInZero(pos);

                return newIcon;
            }

            return null;
        }

        /// <summary>
        /// 获取庄家小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public RectTransform GetBankerIcon(int viewChairId)
        {
            var avatar = GetAvatar<MJAvatar>(viewChairId);
            if (avatar != null)
            {
                return avatar.GetIcon(bankerIconName);
            }

            return null;
        }

        /// <summary>
        /// 显示听牌小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        public RectTransform ShowReadyHandIcon(int viewChairId)
        {
            var icon = GetReadyHandIcon(viewChairId);
            if (icon != null)
            {
                return icon;
            }

            var avatar = GetAvatar<MJAvatar>(viewChairId);
            if (avatar != null)
            {
                var newIcon = OnCreateReadyHandIcon();
                avatar.AddIcon(readyHandIconName, newIcon);

                var pos = OnGetReadyHandIconPos(viewChairId);
                newIcon.SetPositionInZero(pos);

                return newIcon;
            }

            return null;
        }

        /// <summary>
        /// 获取听牌小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public RectTransform GetReadyHandIcon(int viewChairId)
        {
            var avatar = GetAvatar<MJAvatar>(viewChairId);
            if (avatar != null)
            {
                return avatar.GetIcon(readyHandIconName);
            }

            return null;
        }

        /// <summary>
        /// 移除听牌小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        public void RemoveReadyHandIcon(int viewChairId)
        {
            var avatar = GetAvatar<MJAvatar>(viewChairId);
            avatar?.RemoveIcon(readyHandIconName);
        }

        /// <summary>
        /// 切换光效显示
        /// </summary>
        /// <param name="viewChairId"></param>
        public void SwitchLightEffect(int viewChairId)
        {
            Traverse<MJAvatar>((a, i) => a.SetLightEffectActive(viewChairId == i));
        }

        /// <summary>
        /// 隐藏所有光效
        /// </summary>
        public void HideAllLightEffect()
        {
            Traverse<MJAvatar>((a, i) => a.SetLightEffectActive(false));
        }

        /// <summary>
        /// 创建头像层，添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected virtual MJAvatarPanel OnCreateAvatarPanel()
        {
            var panel = new MJAvatarPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.AvatarPanel);

            return panel;
        }


        protected virtual RectTransform OnCreateBankerIcon()
        {
            var icon = WNode.Create("MJCommon/MJ/mj_ui_prefabs", "avatar_icon_2.prefab");
            var txtImg = icon.FindReference<Image>("text_image");
            txtImg.sprite = AssetsManager.Load<Sprite>("MJCommon/MJ/mj_ui_atlas", "avatar/icons/txt/banker.png");
            txtImg.SetNativeSize();
            return icon.rectTransform;
        }


        protected virtual RectTransform OnCreateReadyHandIcon()
        {
            var icon = WNode.Create("MJCommon/MJ/mj_ui_prefabs", "avatar_icon_2.prefab");
            var txtImg = icon.FindReference<Image>("text_image");
            txtImg.sprite = AssetsManager.Load<Sprite>("MJCommon/MJ/mj_ui_atlas", "avatar/icons/txt/ting.png");
            txtImg.SetNativeSize();
            return icon.rectTransform;
        }
        

        protected virtual Vector2 OnGetBankerIconPos(int viewChairId)
        {
            return new List<Vector2>
            {
                new Vector2(165, 160),
                new Vector2(-23, 160),
                new Vector2(-23, 160),
                new Vector2(165, 160)
            }[viewChairId];
        }


        protected virtual Vector2 OnGetReadyHandIconPos(int viewChairId)
        {
            var pos = new List<Vector2>
            {
                new Vector2(165, 160),
                new Vector2(-23, 160),
                new Vector2(-23, 160),
                new Vector2(165, 160)
            }[viewChairId];

            if (GetBankerIcon(viewChairId) != null)
            {
                pos -= new Vector2(0, 40);
            }

            return pos;
        }


        #region Inherit
        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public override AvatarBase GetAvatar(int viewChairId)
        {
            return _avatars[viewChairId];
        }

        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public override void Traverse(Action<AvatarBase, int> handler)
        {
            for (int i = 0; i < _avatars.Length; i++)
            {
                AvatarBase avatar = _avatars[i];

                if (avatar != null && avatar.gameObject != null)
                {
                    handler?.Invoke(avatar, i);
                }
            }
        }

        /// <summary>
        /// 添加头像
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="gender"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override AvatarBase Append(int viewChairId, int gender, string avatarUrl, uint userId)
        {
            Remove(viewChairId);

            MJAvatar newAvatar = OnCreateAvatar(viewChairId, gender, avatarUrl, userId);

            _avatars[viewChairId] = newAvatar;

            newAvatar.onClick = id => OnClickAvatar(id);

            newAvatar.AddTo(_panel);

            var pos = GetAvatarPosition(viewChairId);
            newAvatar.rectTransform.SetPositionInZero(pos);

            return newAvatar;
        }


        protected virtual MJAvatar OnCreateAvatar(int viewChairId, int gender, string avatarUrl, uint userId)
        {
            return new MJAvatar(viewChairId, userId, gender, avatarUrl);
        }

        /// <summary>
        /// 移除头像
        /// </summary>
        /// <param name="viewChairId"></param>
        public override void Remove(int viewChairId)
        {
            MJAvatar current = GetAvatar<MJAvatar>(viewChairId);

            if (current != null && current.gameObject != null)
            {
                current.RemoveFromParent();
            }

            _avatars[viewChairId] = null;
        }

        /// <summary>
        /// 头像位置
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public override Vector2 GetAvatarPosition(int viewChairId)
        {
            Vector2[][] layouts = new Vector2[][]
            {
                new Vector2[]{ layout.left_bottom, new Vector2(75, 300) },
                new Vector2[]{ layout.right_center, new Vector2(-75, 150) },
                new Vector2[]{ layout.center_top, new Vector2(475, -100) },
                new Vector2[]{ layout.left_center, new Vector2(75, 150) },
            };
            Vector2[] info = layouts[viewChairId];
            return DesignResolution.GetScreenPosition(info[0], info[1]);
        }

        /// <summary>
        /// 删除所有头像节点
        /// </summary>
        public override void Clear()
        {
            foreach (MJAvatar avatar in _avatars)
            {
                if (avatar != null && avatar.gameObject != null)
                {
                    avatar.RemoveFromParent();
                }
            }

            _avatars = new MJAvatar[4];
        }
        #endregion
    }
}
