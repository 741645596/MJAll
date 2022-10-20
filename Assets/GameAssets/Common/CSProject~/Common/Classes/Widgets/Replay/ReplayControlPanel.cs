// @Author: tanjinhua
// @Date: 2021/12/17  14:03


using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class ReplayControlPanel : WLayer
    {
        /// <summary>
        /// 切换局数
        /// </summary>
        public Action<int> onSelectGame;
        /// <summary>
        /// 点击退出
        /// </summary>
        public Action onClickClose;
        /// <summary>
        /// 点击播放
        /// </summary>
        public Action onClickPlay;
        /// <summary>
        /// 点击暂停
        /// </summary>
        public Action onClickPause;
        /// <summary>
        /// 当前步改变
        /// </summary>
        public Action<int> onStepChanged;

        private int _gameCount;
        private int _curGameIndex;


        private Button _selectGameBtn;
        private Button _nextGameBtn;
        private Button _closeBtn;
        private RectTransform _controlBar;
        private Button _playBtn;
        private Button _pauseBtn;
        private SKText _curStepTxt;
        private SKText _totalStepTxt;
        private Slider _progressSlider;
        private RectTransform _gameList;
        private RectTransform _gameListContainer;
        private List<ReplayGameSelection> _selections;

        private bool _displaying = true; // 控制UI是否正在显示
        private Coroutine _autoHideRoutine;

        public ReplayControlPanel()
        {
            InitGameObject("Common/Game/replay_ui_prefabs", "replay_ctl_panel.prefab");

            FindReference<Button>("contents").onClick.AddListener(() => OnClickBlankArea());
            _selectGameBtn = FindInChildren<Button>("contents/btn_select_game");
            _selectGameBtn.onClick.AddListener(() => OnClickSelectGame());
            _nextGameBtn = FindInChildren<Button>("contents/btn_next_game");
            _nextGameBtn.onClick.AddListener(() => OnClickNextGame());
            _closeBtn = FindInChildren<Button>("contents/btn_close");
            _closeBtn.onClick.AddListener(() => OnClickClose());
            _controlBar = FindInChildren<RectTransform>("contents/control_bar");
            _playBtn = FindInChildren<Button>("contents/control_bar/btn_play");
            _playBtn.onClick.AddListener(() => OnClickPlay());
            _pauseBtn = FindInChildren<Button>("contents/control_bar/btn_pause");
            _pauseBtn.onClick.AddListener(() => OnClickPause());
            FindInChildren<Button>("contents/control_bar/btn_pre_step").onClick.AddListener(() => OnClickPreStep());
            FindInChildren<Button>("contents/control_bar/btn_next_step").onClick.AddListener(() => OnClickNextStep());
            _curStepTxt = FindInChildren<SKText>("contents/control_bar/txt_current_step");
            _totalStepTxt = FindInChildren<SKText>("contents/control_bar/txt_total_step");
            _progressSlider = FindInChildren<Slider>("contents/control_bar/progress_slider");
            _progressSlider.onValueChanged.AddListener(step => OnStepChanged((int)step));
            _gameList = FindInChildren<RectTransform>("contents/game_list");
            _gameList.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                AutoHide();
                _gameList.gameObject.SetActive(false);
            });
            _gameListContainer = FindInChildren<RectTransform>("contents/game_list/game_list_scroll/viewport/container");

            DelayInvoke(0, () =>
            {
                (_selectGameBtn.transform as RectTransform).LayoutScreen(new Vector2(0, 1), new Vector2(104, -74));
                (_closeBtn.transform as RectTransform).LayoutScreen(new Vector2(1, 1), new Vector2(-74, -74));
                (_nextGameBtn.transform as RectTransform).LayoutScreen(new Vector2(1, 1), new Vector2(-254, -74));
            });

            AutoHide();
        }

        /// <summary>
        /// 设置总步数
        /// </summary>
        /// <param name="stepCount"></param>
        public void SetStepCount(int stepCount)
        {
            _progressSlider.minValue = 0;
            _progressSlider.maxValue = stepCount-1;
            _progressSlider.SetValueWithoutNotify(0);
            _curStepTxt.text = "0";
            _totalStepTxt.text = _progressSlider.maxValue.ToString();
        }

        /// <summary>
        /// 设置当前步
        /// </summary>
        /// <param name="step"></param>
        public void SetCurrentStep(int step, bool sendEvent = false)
        {
            if (sendEvent)
            {
                _progressSlider.value = step;
            }
            else
            {
                _progressSlider.SetValueWithoutNotify(step);
            }
            _curStepTxt.text = _progressSlider.value.ToString();
        }

        /// <summary>
        /// 设置总局数
        /// </summary>
        /// <param name="gameCount"></param>
        public void SetGameCount(int gameCount)
        {
            _gameCount = gameCount;
            _curGameIndex = 0;
            _nextGameBtn.gameObject.SetActive(_curGameIndex < _gameCount - 1);

            if (_selections == null)
            {
                _selections = new List<ReplayGameSelection>();
            }
            _selections.Clear();

            var height = gameCount * 120;
            _gameListContainer.RemoveAllChildren();
            _gameListContainer.SetContentSizeInZero(new Vector2(256, height));

            for (int i = 0; i < gameCount; i++)
            {
                var selection = new ReplayGameSelection(i);
                selection.AddTo(_gameListContainer);
                selection.onSelect = idx =>
                {
                    AutoHide();

                    if (_curGameIndex == idx)
                    {
                        return;
                    }
                    _selections.ForEach(s =>
                    {
                        if (s.index != idx)
                        {
                            s.Deselect();
                        }
                    });

                    onSelectGame?.Invoke(idx);

                    _curGameIndex = idx;

                    _nextGameBtn.gameObject.SetActive(_curGameIndex < _gameCount - 1);
                };

                if (i == 0)
                {
                    selection.Select(false);
                }

                _selections.Add(selection);
            }
        }

        public void ShowPlaying()
        {
            _playBtn.gameObject.SetActive(false);
            _pauseBtn.gameObject.SetActive(true);
        }

        public void ShowPaused()
        {
            _playBtn.gameObject.SetActive(true);
            _pauseBtn.gameObject.SetActive(false);
        }

        private void OnClickSelectGame()
        {
            AutoHide();

            _gameList.gameObject.SetActive(true);
        }

        private void OnClickNextGame()
        {
            AutoHide();

            if (_curGameIndex == _gameCount - 1)
            {
                return;
            }

            _selections[_curGameIndex + 1].Select();
        }

        private void OnClickClose()
        {
            AutoHide();

            onClickClose?.Invoke();
        }

        private void OnClickPlay()
        {
            AutoHide();

            ShowPlaying();

            onClickPlay?.Invoke();
        }

        private void OnClickPause()
        {
            AutoHide();

            ShowPaused();

            onClickPause?.Invoke();
        }

        private void OnClickPreStep()
        {
            AutoHide();

            int step = (int)_progressSlider.value - 1;

            SetCurrentStep(step, true);
        }

        private void OnClickNextStep()
        {
            AutoHide();

            int step = (int)_progressSlider.value + 1;

            SetCurrentStep(step, true);
        }

        private void OnStepChanged(int step)
        {
            AutoHide();

            _curStepTxt.text = step.ToString();

            onStepChanged?.Invoke(step);
        }

        private void OnClickBlankArea()
        {
            if (_displaying)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Show()
        {
            _selectGameBtn.gameObject.SetActive(true);
            _nextGameBtn.gameObject.SetActive(_curGameIndex < _gameCount - 1);
            _closeBtn.gameObject.SetActive(true);
            _controlBar.gameObject.SetActive(true);
            _displaying = true;
            AutoHide();
        }

        private void Hide()
        {
            _selectGameBtn.gameObject.SetActive(false);
            _nextGameBtn.gameObject.SetActive(false);
            _closeBtn.gameObject.SetActive(false);
            _controlBar.gameObject.SetActive(false);
            _gameList.gameObject.SetActive(false);
            _displaying = false;
        }

        private void AutoHide()
        {
            if (_autoHideRoutine != null)
            {
                CancelInvoke(_autoHideRoutine);
            }
            _autoHideRoutine = DelayInvoke(5f, () => Hide());
        }
    }
}
