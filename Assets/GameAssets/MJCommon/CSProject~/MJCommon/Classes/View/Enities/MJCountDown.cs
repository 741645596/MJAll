// MJCountDown.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/9/30
using System;
using System.Collections.Generic;
using Unity.Core;
using Unity.Widget;
using UnityEngine;
using WLCore.Coroutine;

namespace MJCommon
{
    public class MJCountDown : WNode3D
    {
        public Action<int> onTick;

        private Dictionary<char, Mesh> numMeshes;
        private MeshFilter num1;
        private MeshFilter num2;

        private int time;
        private Action callback;
        private Coroutine coroutine;

        public MJCountDown()
        {
            InitGameObject("MJCommon/MJ/mj_zm_effe_tishi", "dengdai_x_01_01.prefab");

            num1 = FindReference<MeshFilter>("0");
            num2 = FindReference<MeshFilter>("9");

            numMeshes = new Dictionary<char, Mesh>();

            for (char i = '0'; i <= '9'; i++)
            {
                var prefab = AssetsManager.Load<GameObject>("MJCommon/MJ/mj_zm_effe_tishi", $"{i}.prefab");
                if (!prefab.TryGetComponent(out MeshFilter meshFilter))
                {
                    meshFilter = prefab.GetComponentInChildren<MeshFilter>();
                }
                var mesh = meshFilter.sharedMesh;
                numMeshes.Add(i, mesh);
            }
            UpdateTime();
        }

        public void StartCountdown(int time, Action callback = null)
        {
            StopCountdown();

            this.time = time;
            this.callback = callback;
            UpdateTime();

            coroutine = DelayInvokeRepeating(1, OnTick);
        }

        public void StopCountdown()
        {
            if(coroutine != null)
            {
                CancelInvoke(coroutine);
            }

            SetText("00");
        }

        private void OnTick()
        {
            if (time <= 0)
            {
                onTick?.Invoke(0);

                UpdateTime();

                return;
            }
            time--;

            onTick?.Invoke(time);

            UpdateTime();
        }

        private void UpdateTime()
        {
            time %= 100;
            var str = time.ToString("00");

            num1.mesh = numMeshes[str[0]];
            num2.mesh = numMeshes[str[1]];

            if (time == 0)
            {
                if(coroutine != null)
                {
                    CancelInvoke(coroutine);
                }
                callback?.Invoke();
            }
        }

        private void SetText(string text)
        {
            num1.mesh = numMeshes[text[0]];
            num2.mesh = numMeshes[text[1]];
        }
    }
}
