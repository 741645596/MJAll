// @Author: lili
// @Date: 2021/6/1 14:27:54

using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace Common
{
    public class GameDialog : WNode
    {
        public enum ClickType
        {
            Cancle = 0,
            OK = 1,
            Escape = -1,
        }

        //private static readonly int GameDialogTag = 100004;

        private Action<ClickType> onClick;        
        //private Button _okButton;
        //private Button _cancelButton;

        /// <summary>
        /// 游戏内对话框
        /// </summary>
        /// <param name="text">显示的文本信息</param>
        public GameDialog(string text)
        {
            //m_okButton = FindReference<Button>("OkButton");
            //m_cancelButton = FindReference<Button>("CancelButton");

            //m_okButton.onClick.AddListener(() => OnClick(ClickType.OK));
            //m_cancelButton.onClick.AddListener(() => OnClick(ClickType.Cancle));

            //// 对话文本
            //SKText dialogText = FindReference<SKText>("Dialog");
            //dialogText.text = text;

            //// 弹出动画
            //Node2D bgNode = FindReference("BG") as Node2D;
            //bgNode.scale = new Vector2(0.6f, 0.6f);
            //Action2D.NewSequence()
            //    .ScaleTo(0.1f, 1.1f)
            //    .ScaleTo(0.08f, 1f)
            //    .Run(bgNode);

            //// 吞噬所有点击事件
            //RegisterTouchEvents2D(data => true);

            // TODO:DialogLayerUtils.addShowDialogLayerCount()
        }

        /// <summary>
        /// 底下小行文字提示
        /// </summary>
        /// <param name="tips"></param>
        public void ShowTips(string tips)
        {
            if (!string.IsNullOrEmpty(tips))
            {
                //Text tipsText = FindReference<Text>("Tips");
                //Node2D dialogNode = FindReference("Dialog") as Node2D;
                //tipsText.gameObject.SetActive(true);
                //tipsText.text = tips;
                //dialogNode.LayoutWithParent(Layouts.center, new Vector2(0, 30));
            }
        }

        /// <summary>
        /// 只显示确定按钮
        /// </summary>
        /// <param name="callback"></param>
        public void ShowOK(Action<ClickType> callback)
        {
            onClick = callback;

            //Node2D okNode = FindReference("OkButton") as Node2D;
            //okNode.LayoutWithParent(Layouts.centerBottom, new Vector2(0, 52));
            //m_cancelButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示确定、取消两个按钮
        /// </summary>
        /// <param name="callback"></param>
        public void ShowOKCancel(Action<ClickType> callback)
        {
            onClick = callback;
        }

        /// <summary>
        /// 显示确定、自定义两个按钮
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="customBtnRes"></param>
        /// <param name="customTextRes"></param>
        public void ShowOKCustom(Action<ClickType> callback, string customBtnRes, string customTextRes)
        {
            onClick = callback;
            ShowClose();

            // 自定义按钮
            //Image cancelButton = FindReference<Image>("CancelButton");
            //Image cancelLight = FindReference<Image>("CancelLight");
            //if (!string.IsNullOrEmpty(customBtnRes))
            //{
            //    cancelButton.sprite = AssetsLoader.Instance.Load<Sprite>(customBtnRes);
            //    cancelButton.SetNativeSize();
            //    cancelLight.gameObject.SetActive(false);
            //}
            //else
            //{
            //    cancelButton.sprite = AssetsLoader.Instance.Load<Sprite>("WLHall/Funcs/Common/Res/btn_dt_mid_3.png");
            //    cancelLight.sprite = AssetsLoader.Instance.Load<Sprite>("WLHall/Funcs/Common/Res/light_3.png");
            //}
            //if (!string.IsNullOrEmpty(customTextRes))
            //{
            //    SetCancelImage(customTextRes);
            //}

        }

        /// <summary>
        /// 重置确定按钮文字图片
        /// </summary>
        /// <param name="imgPath"></param>
        public void SetOKImage(string imgPath)
        {
            //Image image = FindReference<Image>("OkImage");
            //image.sprite = AssetsLoader.Instance.Load<Sprite>(imgPath);
            //image.SetNativeSize();
        }

        /// <summary>
        /// 重置取消按钮文字图片
        /// </summary>
        /// <param name="imgPath"></param>
        public void SetCancelImage(string imgPath)
        {
            //Image image = FindReference<Image>("CancelImage");
            //image.sprite = AssetsLoader.Instance.Load<Sprite>(imgPath);
            //image.SetNativeSize();
        }

        /// <summary>
        /// 显示关闭按钮
        /// </summary>
        public void ShowClose()
        {
            //Button closeButton = FindReference<Button>("CloseButton");
            //closeButton.gameObject.SetActive(true);
            //closeButton.onClick.AddListener(() => OnClick(ClickType.Escape));
        }

        /// <summary>
        /// 支持按返回键取消对话框
        /// </summary>
        public void EnableKeyBack()
        {
            //RegisterKeyInputEvents(KeyCode.Backspace, kedCode => true, null, keyCode => OnClick(ClickType.Escape));
            //RegisterKeyInputEvents(KeyCode.Escape, kedCode => true, null, keyCode => OnClick(ClickType.Escape));
        }

        private void OnClick(ClickType index)
        {
            onClick?.Invoke(index);
            //RemoveFromParent();
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/CommonWidgets/GameDialog/GameDialog.prefab");
        //}


        /// <summary>
        /// 游戏内的弹出对话框，显示确定和取消按钮
        /// </summary>
        /// <param name="text"></param>
        /// <param name="callback"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        public static GameDialog CreateWithOKCancel(string text, Action<ClickType> callback, string tips = null)
        {
            GameDialog gameDialog = GameDialog.Create(text);
            gameDialog.ShowTips(tips);
            gameDialog.ShowOKCancel(callback);
            return gameDialog;
        }

        /// <summary>
        /// 游戏内的弹出对话框，显示确定、自定义两个按钮
        /// </summary>
        /// <param name="text"></param>
        /// <param name="callback"></param>
        /// <param name="customBtnRes"></param>
        /// <param name="customTextRes"></param>
        /// <returns></returns>
        public static GameDialog CreateWithOKCustom(string text, Action<ClickType> callback, string customBtnRes, string customTextRes)
        {
            GameDialog gameDialog = GameDialog.Create(text);
            gameDialog.ShowOKCustom(callback, customBtnRes, customTextRes);
            return gameDialog;
        }

        /// <summary>
        /// 游戏内的弹出对话框，只有一个确定按钮
        /// </summary>
        /// <param name="text"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static GameDialog CreateWithOK(string text, Action<ClickType> callback)
        {
            GameDialog gameDialog = GameDialog.Create(text);
            gameDialog.ShowOK(callback);
            return gameDialog;
        }

        public static GameDialog Create(string text)
        {
            GameDialog gameDialog = new GameDialog(text);
            //gameDialog.zorder = GameZorder.GameDialog;
            //gameDialog.tag = GameDialogTag;
            //gameDialog.EnableKeyBack();
            return gameDialog;
        }

        public static void Remove()
        {
            //SceneMaster.Instance.runningScene?.root2D.RemoveChildByTag(GameDialogTag);
        }

    }
}
