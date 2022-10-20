// @Author: tanjinhua
// @Date: 2021/4/21  9:57


using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class GameMenu : WLayer
    {

        public enum Alignment
        {
            Left,
            Right
        }


        public enum State
        {
            Show,
            Hide
        }

        /// <summary>
        /// 对齐
        /// </summary>
        public Alignment alignment
        {
            get => _alignment;

            set
            {
                if (_alignment == value)
                {
                    return;
                }

                _alignment = value;

                SetupAlignment();
            }
        }


        /// <summary>
        /// 状态
        /// </summary>
        public State state
        {
            get => _state;

            set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;

                _emptyBtn.gameObject.SetActive(_state == State.Show);

                Refresh();
            }
        }


        private Alignment _alignment = Alignment.Left;
        private Image _bgLeft;
        private Image _bgRight;
        private List<MenuButton> _menuButtons;
        private State _state = State.Hide;
        private Button _emptyBtn;



        public GameMenu()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "game_meun.prefab");

            _bgLeft = FindInChildren<Image>("bg_left");

            _bgRight = FindInChildren<Image>("bg_right");

            _emptyBtn = FindInChildren<Button>("empty_btn");

            _emptyBtn.onClick.AddListener(() => PlayHide());
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configs"></param>
        public void Initialize(List<MenuButton.Config> configs)
        {
            _menuButtons = new List<MenuButton>();
            for (int i = 0; i < configs.Count; i++)
            {
                MenuButton menuButton = OnCreateMenuButton(configs[i]);

                menuButton.AddTo(this);

                SetupMenuButton(menuButton);

                _menuButtons.Add(menuButton);
            }

            Refresh();
        }


        /// <summary>
        /// 插入菜单按钮
        /// </summary>
        /// <param name="index"></param>
        /// <param name="config"></param>
        public void InsertMenuButton(int index, MenuButton.Config config)
        {
            MenuButton menuButton = OnCreateMenuButton(config);

            menuButton.AddTo(this);

            SetupMenuButton(menuButton);

            _menuButtons.Insert(index, menuButton);

            Refresh();
        }


        /// <summary>
        /// 移除菜单按钮
        /// </summary>
        /// <param name="name"></param>
        public void RemoveMenuButton(string name)
        {
            for (int i = _menuButtons.Count - 1; i >= 0; i--)
            {
                MenuButton menuButton = _menuButtons[i];

                if (menuButton.gameObject.name == name)
                {
                    _menuButtons.Remove(menuButton);

                    Object.Destroy(menuButton.gameObject);

                    break;
                }
            }

            Refresh();
        }


        /// <summary>
        /// 设置菜单按钮是否可见
        /// </summary>
        /// <param name="name"></param>
        /// <param name="visible"></param>
        public void SetMenuButtonVisible(string name, bool visible)
        {
            for (int i = 0; i < _menuButtons.Count; i++)
            {
                MenuButton menuButton = _menuButtons[i];

                if (menuButton.gameObject.name == name)
                {
                    menuButton.gameObject.SetActive(visible);

                    break;
                }
            }

            Refresh();
        }


        /// <summary>
        /// 播放显示动画
        /// </summary>
        public void PlayShow()
        {
            _state = State.Show;

            _emptyBtn.gameObject.SetActive(true);

            var acitveBackground = _alignment == Alignment.Left ? _bgLeft : _bgRight;

            DOTween.Kill(acitveBackground.gameObject);

            var bgSeq = DOTween.Sequence();
            bgSeq.Append(acitveBackground.DOFade(1, 0.2f));
            bgSeq.SetTarget(acitveBackground.gameObject);
            bgSeq.SetLink(acitveBackground.gameObject);
            bgSeq.SetAutoKill(true);
            bgSeq.Play();

            List<MenuButton> visibleButtons = GetVisibleMenuButtons();

            for (int i = 0; i < visibleButtons.Count; i++)
            {
                MenuButton menuButton = visibleButtons[i];

                DOTween.Kill(menuButton.gameObject);

                Vector2 pos = GetPositionByIndex(i, true);

                float delay = i * 0.1f;

                var trs = menuButton.rectTransform;
                var seq = DOTween.Sequence();
                seq.AppendInterval(delay);
                seq.Append(trs.DOAnchorPos(pos, 0.2f));
                seq.SetTarget(menuButton.gameObject);
                seq.SetLink(menuButton.gameObject);
                seq.SetAutoKill(true);
                seq.Play();
            }
        }


        /// <summary>
        /// 播放隐藏动画
        /// </summary>
        public void PlayHide()
        {
            _state = State.Hide;

            _emptyBtn.gameObject.SetActive(false);

            var acitveBackground = _alignment == Alignment.Left ? _bgLeft : _bgRight;

            DOTween.Kill(acitveBackground.gameObject);

            var bgSeq = DOTween.Sequence();
            bgSeq.Append(acitveBackground.DOFade(0, 0.2f));
            bgSeq.SetTarget(acitveBackground.gameObject);
            bgSeq.SetLink(acitveBackground.gameObject);
            bgSeq.SetAutoKill(true);
            bgSeq.Play();

            List<MenuButton> visibleButtons = GetVisibleMenuButtons();

            int reverseIndex = visibleButtons.Count - 1;
            for (int i = 0; i < visibleButtons.Count; i++)
            {
                MenuButton menuButton = visibleButtons[i];

                DOTween.Kill(menuButton.gameObject);

                Vector2 pos = GetPositionByIndex(i, false);

                float delay = reverseIndex * 0.1f;

                reverseIndex--;

                var trs = menuButton.rectTransform;
                var seq = DOTween.Sequence();
                seq.AppendInterval(delay);
                seq.Append(trs.DOAnchorPos(pos, 0.2f));
                seq.SetTarget(menuButton.gameObject);
                seq.SetLink(menuButton.gameObject);
                seq.SetAutoKill(true);
                seq.Play();
            }
        }


        private void Refresh()
        {
            DisableBackgrounds();

            var activeBackground = _alignment == Alignment.Left ? _bgLeft : _bgRight;

            DOTween.Kill(activeBackground.gameObject);

            activeBackground.gameObject.SetActive(true);

            if (_state == State.Show)
            {
                activeBackground.color = Color.white;
            }
            else
            {
                activeBackground.color = Color.clear;
            }


            List<MenuButton> visibleButtons = GetVisibleMenuButtons();

            for (int i = 0; i < visibleButtons.Count; i++)
            {
                MenuButton menuButton = visibleButtons[i];

                DOTween.Kill(menuButton.gameObject);

                Vector2 pos = GetPositionByIndex(i, _state == State.Show);

                menuButton.SetPosition(pos);
            }
        }


        private Vector2 GetPositionByIndex(int index, bool isShown)
        {
            Vector2 layout = _alignment == Alignment.Left ? new Vector2(0, 1) : new Vector2(1, 1);

            float offsetY = -90 * index - 160;

            if (isShown)
            {
                float offsetX1 = _alignment == Alignment.Left ? 96 : -96;
                return DesignResolution.GetScreenPosition(layout, new Vector2(offsetX1, offsetY));
                //return rectTransform.CalculatePositionByLayout(layout, new Vector2(offsetX1, offsetY), true);
            }

            float offsetX2 = _alignment == Alignment.Left ? -150 : 150;
            return DesignResolution.GetScreenPosition(layout, new Vector2(offsetX2, offsetY));
            //return rectTransform.CalculatePositionByLayout(layout, new Vector2(offsetX2, offsetY));
        }



        private List<MenuButton> GetVisibleMenuButtons()
        {
            List<MenuButton> result = new List<MenuButton>();
            for (int i = 0; i < _menuButtons.Count; i++)
            {
                MenuButton menuButton = _menuButtons[i];

                if (menuButton.gameObject.activeSelf)
                {
                    result.Add(menuButton);
                }
            }
            return result;
        }


        private void SetupAlignment()
        {
            DisableBackgrounds();

            if (_alignment == Alignment.Left)
            {
                _bgLeft.gameObject.SetActive(true);
            }
            else
            {
                _bgRight.gameObject.SetActive(true);
            }

            Refresh();
        }


        private void DisableBackgrounds()
        {
            _bgLeft.gameObject.SetActive(false);

            _bgRight.gameObject.SetActive(false);
        }


        private MenuButton OnCreateMenuButton(MenuButton.Config config)
        {
            MenuButton menuButton = new MenuButton(config);

            menuButton.onClick += () => PlayHide();

            return menuButton;
        }


        private void SetupMenuButton(MenuButton btn)
        {
            var trs = btn.rectTransform;
            trs.anchorMin = Vector2.zero;
            trs.anchorMax = Vector2.zero;
        }
    }
}
