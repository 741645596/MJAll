
// AudioManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/14

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Core
{
    /// <summary>
    /// 音乐/音效管理者
    /// </summary>
    public static class AudioMgr
    {
        private static GameObject _audioMgrObject;
        private static AudioSource _musicSource;
        private static Dictionary<string, AudioSource> _effectSourceDic = new Dictionary<string, AudioSource>();

        /// <summary>
        /// 启动游戏后需要初始化下
        /// </summary>
        public static void Init()
        {
            _audioMgrObject = GameObject.Find("AudioManager");
            if (_audioMgrObject == null)
            {
                _audioMgrObject = new GameObject("AudioManager");
                Object.DontDestroyOnLoad(_audioMgrObject);
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        /// <param name="loop"></param>
        public static void PlayMusic(string assetName, string key, bool loop = true)
        {
            Debug.Assert(_audioMgrObject != null, "错误提示：未初始化GameAudio.Init");

            var clip = AssetsManager.Load<AudioClip>(assetName, key);
            if (clip == null)
            {
                WLDebug.LogWarning("clip is null");
                return;
            }
            if (_musicSource == null)
            {
                _musicSource = _audioMgrObject.AddComponent<AudioSource>();
            }
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public static void StopMusic()
        {
            if (_musicSource == null)
            {
                return;
            }

            if (_musicSource.isPlaying == false)
            {
                return;
            }

            _musicSource.Stop();
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public static void PauseMusic()
        {
            if (_musicSource == null)
            {
                return;
            }

            if (_musicSource.isPlaying == false)
            {
                return;
            }

            _musicSource.Pause();
        }

        /// <summary>
        /// 对应PauseMusic，恢复背景音乐
        /// </summary>
        public static void ResumeMusic()
        {
            if (_musicSource == null)
            {
                return;
            }

            if (_musicSource.isPlaying == true)
            {
                return;
            }

            _musicSource.UnPause();
        }

        /// <summary>
        /// 删除背景音乐对象
        /// </summary>
        public static void RemoveMusic()
        {
            StopMusic();

            // 清除背景音乐
            if (_musicSource != null)
            {
                Object.Destroy(_musicSource);
                _musicSource = null;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        /// <param name="loop"></param>
        public static void PlayEffect(string assetName, string key)
        {
            Debug.Assert(_audioMgrObject != null, "错误提示：未初始化GameAudio.Init");

            var clip = AssetsManager.Load<AudioClip>(assetName, key);
            if (clip == null)
            {
                return;
            }

            var effect = _GetEffectSource(assetName, key);
            effect.PlayOneShot(clip);
        }

        /// <summary>
        /// 播放循环型的音效
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        public static void PlayLoopEffect(string assetName, string key)
        {
            Debug.Assert(_audioMgrObject != null, "错误提示：未初始化GameAudio.Init");

            var clip = AssetsManager.Load<AudioClip>(assetName, key);
            if (clip == null)
            {
                return;
            }

            var effect = _GetEffectSource(assetName, key);
            effect.clip = clip;
            effect.loop = true;
            effect.Play();
        }

        /// <summary>
        /// 异步播放音效
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        /// <param name="loop"></param>
        public static void PlayEffectAsync(string assetName, string key)
        {
            Debug.Assert(_audioMgrObject != null, "错误提示：未初始化GameAudio.Init");

            AssetsManager.LoadAsync<AudioClip>(assetName, key, (clip) =>
            {
                if (clip == null)
                {
                    return;
                }

                var effect = _GetEffectSource(assetName, key);
                effect.PlayOneShot(clip);
            });
        }

        /// <summary>
        /// 循环播放音效
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        public static void PlayLoopEffectAsync(string assetName, string key)
        {
            Debug.Assert(_audioMgrObject != null, "错误提示：未初始化GameAudio.Init");

            AssetsManager.LoadAsync<AudioClip>(assetName, key, (clip) =>
            {
                if (clip == null)
                {
                    return;
                }

                var effect = _GetEffectSource(assetName, key);
                effect.clip = clip;
                effect.loop = true;
                effect.Play();
            });
        }

        /// <summary>
        /// 只停止音效不删除，如果要删除使用RemoveEffect
        /// </summary>
        /// <param name="key"></param>
        public static void StopEffect(string assetName, string key)
        {
            var effectKey = _GetEffectKey(assetName, key);
            if (_effectSourceDic.ContainsKey(effectKey))
            {
                var effect = _effectSourceDic[effectKey];
                if(effect != null)
                {
                    effect.Stop();
                }
            }         
        }

        /// <summary>
        /// 停止播放所有背景音效
        /// </summary>
        public static void StopAllEffects()
        {
            foreach (var effect in _effectSourceDic.Values)
            {
                if(effect != null)
                {
                    effect.Stop();
                }
            }
        }

        /// <summary>
        /// 从内存删除音效对象
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveEffect(string assetName, string key)
        {
            StopEffect(assetName, key);

            var effectKey = _GetEffectKey(assetName, key);
            if (_effectSourceDic.ContainsKey(effectKey))
            {
                Object.Destroy(_effectSourceDic[effectKey]);
                _effectSourceDic.Remove(effectKey);
            }
        }

        /// <summary>
        /// 删除所有音效
        /// </summary>
        public static void RemoveAllEffect()
        {
            // 清除所有音效
            foreach (var item in _effectSourceDic)
            {
                Object.Destroy(item.Value);
            }
            _effectSourceDic = new Dictionary<string, AudioSource>();
        }

        /// <summary>
        /// 停止背景音乐/清除所有音效
        /// </summary>
        public static void Clear()
        {
            RemoveMusic();

            RemoveAllEffect();
        }

        /// <summary>
        /// 删除整个Audio对象，重新初始化
        /// </summary>
        public static void DestoryAudioObject()
        {
            _effectSourceDic.Clear();
            if (_audioMgrObject != null)
            {
                GameObject.DestroyImmediate(_audioMgrObject);
                _audioMgrObject = null;
            }
            _musicSource = null;

            Init();
        }

        private static string _GetEffectKey(string assetName, string key)
        {
            return assetName + key;
        }

        private static AudioSource _GetEffectSource(string assetName, string key)
        {
            var effectKey = _GetEffectKey(assetName, key);
            if (_effectSourceDic.ContainsKey(effectKey))
            {
                return _effectSourceDic[effectKey];
            }

            var effect = _audioMgrObject.AddComponent<AudioSource>();
            _effectSourceDic.Add(effectKey, effect);
            return effect;
        }
    }
}
