// AssetsManager.cs
// Date: 2019/6/20

using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Core
{
    /// <summary>
    /// 预加载AB包
    /// </summary>
    public class AssertsPreload
    {
        private List<string> _abNames;
        private Action<int, int> _progress;
        private int _curIndex;

        /// <summary>
        /// 预加载ab包合集
        /// </summary>
        /// <param name="abNames"></param>
        /// <param name="progress"></param>
        public static void LoadAsync(List<string> abNames, Action<int, int> progress)
        {
            var r = new AssertsPreload();
            r.Load(abNames, progress);
        }

        /// <summary>
        /// 预加载ab包合集
        /// </summary>
        /// <param name="abNames"> ab包合集 </param>
        /// <param name="progress"> 进度回调(当前进度，总数量)，进度==总数量时表示预加载全部完成 </param>
        public void Load(List<string> abNames, Action<int, int> progress)
        {
            if (abNames.Count == 0)
            {
                WLDebug.LogWarning("错误提示：abNames.Count == 0");
                return;
            }

            _curIndex = 0;
            _abNames = abNames;
            _progress = progress;
            
            _load();
        }

        private void _load()
        {
            if (_abNames.Count == _curIndex)
            {
                _progress(_curIndex, _curIndex);
                return;
            }

            _progress(_curIndex, _abNames.Count);
            AssetsManager.LoadAllAsync(_abNames[_curIndex], (v) =>
            {
                _curIndex++;
                _load();
            });
        }

    }
}
