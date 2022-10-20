// @Author: tanjinhua
// @Date: 2021/12/16  17:36

using System.Collections.Generic;
using Unity.Widget;
using WLHall.Game;

namespace Common
{
    public class ReplayController : BaseGameController
    {
        private ReplayStage _stage;
        private List<BaseRecord> _records;
        private ReplayControlPanel _controlPanel;

        private bool _sceneLoaded;
        private BaseRecord _current;
        private bool _paused = true;

        public override void OnSceneLoaded()
        {
            _sceneLoaded = true;
            _stage = stage as ReplayStage;
            _controlPanel = new ReplayControlPanel();
            _controlPanel.AddTo(WDirector.GetRootLayer());
            _controlPanel.SetGameCount(_stage.totalGameCount);
            _controlPanel.onSelectGame = OnSelectGame;
            _controlPanel.onStepChanged = OnStepChanged;
            _controlPanel.onClickPlay = OnPlay;
            _controlPanel.onClickPause = OnPause;
            _controlPanel.onClickClose = () => _stage.Exit();
            if (_records != null)
            {
                _controlPanel.SetStepCount(_records.Count);
            }

            ExecuteInitial();
        }

        /// <summary>
        /// 设置回放记录列表
        /// </summary>
        /// <param name="records"></param>
        public void SetRecords(List<BaseRecord> records)
        {
            _records?.ForEach(r => r.Depose());

            _records = records;

            _paused = true;

            _current = null;

            _controlPanel?.SetStepCount(_records.Count);

            _controlPanel?.ShowPaused();

            if (_sceneLoaded)
            {
                ExecuteInitial();
            }
        }

        protected virtual void OnSelectGame(int index)
        {
            _stage.RequestRecordData(index);
        }

        protected virtual void OnStepChanged(int index)
        {
            _paused = true;

            _controlPanel.ShowPaused();

            if (_current == null)
            {
                return;
            }

            if (index == _current.index)
            {
                return;
            }

            var curIdx = _current.index;
            if (index > curIdx)
            {
                for (int i = curIdx+1; i <= index; i++)
                {
                    if (_current.interval > 0)
                    {
                        _current.Abort();
                    }
                    _current = _records[i];
                    _current.interval = _current.Execute();
                }
                return;
            }

            for (int i = curIdx; i > index; i--)
            {
                if (_current.interval > 0)
                {
                    _current.Abort();
                }
                _current.Undo();
                _current = _records[i-1];
                if (_current.index != 0)
                {
                    _current.Replay();
                }
            }
        }

        protected virtual void OnPlay()
        {
            _paused = false;
        }

        protected virtual void OnPause()
        {
            _paused = true;
        }

        internal void Update(float dt)
        {
            if (_current != null)
            {
                _current.interval -= dt;
            }

            if (_paused)
            {
                return;
            }

            if (_current != null && _current.interval <= 0 && _current.index + 1 < _records.Count)
            {
                var index = _current.index + 1;
                _current = _records[index];
                _current.interval = _current.Execute();
                _controlPanel.SetCurrentStep(index, false);
            }
        }

        private void ExecuteInitial()
        {
            if (_records == null || _records.Count == 0)
            {
                return;
            }

            _current = _records[0];

            _current.interval = _current.Execute();
        }

        public override void OnShutdown()
        {
            base.OnShutdown();

            _records?.ForEach(r => r.Depose());

            _records?.Clear();

            _records = null;
        }
    }
}
