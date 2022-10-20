// @Author: tanjinhua
// @Date: 2021/8/12  14:50


using Unity.Core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Unity.Utility;
using WLCore.Helper;
using System.Diagnostics;
using System.IO;
using DG.Tweening;
using Unity.Widget;
using UnityEngine.EventSystems;

namespace Common
{
    public class MyTouchLayer : WLayer
    {
        public MyTouchLayer()
        {
            Init();

            EnableTouchEvent();
        }

        protected override bool OnTouchBegan(PointerEventData d)
        {
            WLDebug.Log("=========== OnTouchBegan ", gameObject.name);
            return true;
        }

        protected override void OnTouchMove(PointerEventData d)
        {
            WLDebug.Log("=========== OnTouchMove", d.LocationPosition(), d.PreLocationPosition());
        }

        protected override void OnTouchEnd(PointerEventData d)
        {
            WLDebug.Log("=========== OnTouchEnd");
        }
    }

    public class MyNode : WLayer
    {
        public MyNode()
        {
            InitGameObject("Common/Module2/prefabs3", "CCPrefabCase.prefab");
        }

        protected override void Start()
        {
            WLDebug.Log("########### StartEvent");
        }

        protected override void OnDestroy()
        {
            WLDebug.Log("########### OnDestroyEvent");
        }

        protected override void OnEnable()
        {
            WLDebug.Log("########### OnEnableEvent");
        }

        protected override void OnDisable()
        {
            WLDebug.Log("########### OnDisableEvent");
        }
    }

    public class CCPrefabCase : BaseCaseStage
    {
        private WNode _instance;
        private Coroutine _routine;
        private Coroutine _delayInvokeRoutine;


        public override void OnInitialize()
        {
            //AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();

            _instance = new MyNode();
            _instance.AddTo(root);
        }


        public void CaseAssertBundleLoad()
        {
            //var ani = _instance.FindInChildren<Animation>("Image (1)");

            //var watch = new Stopwatch();
            //watch.Restart();


            //watch.Stop();
            //WLDebug.Log("=============== ", watch.ElapsedMilliseconds);
        }

        public void CaseXXXX()
        {
            WNode.Create("MJCommon/MJ", "mj_ui_effe_tishi/hptishi_l_01_03.prefab")
                .AddTo(root);

            //var imgTrans = _instance.FindReference("Image") as RectTransform;
            //imgTrans.Layout(layout.center);

            //var imgObject = ImageHelper.CreateImage("Common/Module2/test_images", "choose_bg.png");
            //var imgTrans = imgObject.transform as RectTransform;
            //imgTrans.SetParent(root.gameObject.transform, false);

            //var watch = new Stopwatch();
            //watch.Restart();

            //watch.Stop();
            //WLDebug.Log("=============== ", watch.ElapsedMilliseconds);
        }

        public void CaseTouch()
        {
            var layer = new MyTouchLayer();
            layer.AddTo(root, -200);
            layer.SetName("Layer 1");

            //var layer2 = new MyTouchLayer();
            //layer2.AddTo(root, -300);
            //layer2.SetName("Layer 2");
        }

        public void CaseColorTouch()
        {
            WLayer.Create(Color.red)
                .AddTo(root);
        }

        public void CaseOnButtonClick()
        {
            // 注册按钮点击事件
            var btTrans = _instance.FindReference("Button");
            var bt = btTrans.GetComponent<Button>();
            bt.onClick.AddListener(() =>
            {
                WLDebug.Log("Click Button");
                //btTrans.RunTweenGraph("Common/Module2/prefabs3", "DOTweenGraph_1.asset", (t)=>
                //{
                //});
            });
        }

        public void CaseGetAllChildren()
        {
            List<Transform> allChildren = _instance.GetAllChildren();
            string info = "";
            allChildren.ForEach(c => info += c.name + "\n");
            WLDebug.Log(info);
        }

        public void GetFirstChildren()
        {
            List<Transform> allChildren = _instance.GetChildren();
            string info = "";
            allChildren.ForEach(c => info += c.name + "\n");
            WLDebug.Log(info);
        }

        public void CaseFindReference()
        {
            // find by name
            var reference = _instance.FindReference("Image");
            reference.GetComponent<Image>().color = Color.red;

            // find component
            var scrollRect = _instance.FindReference<ScrollRect>("Scroll View");
            WLDebug.Log(scrollRect);

            // find by condition
            var viewPort = _instance.FindReference(t => t == scrollRect.viewport);
            WLDebug.Log(viewPort);
            //viewPort.GetComponent<Mask>().showMaskGraphic = true;
        }

        public void CaseStartCoroutine()
        {
            _routine = _instance.StartCoroutine(IECoroutineCase());
        }

        public void CaseStopCoroutine()
        {
            if (_routine != null)
            {
                _instance.StopCoroutine(_routine);
                _routine = null;
            }
        }

        public void CaseDelayInvoke()
        {
            _delayInvokeRoutine = _instance.DelayInvoke(2, () =>
            {
                WLDebug.Log("Delay Invoke after 2 sec.");
                _delayInvokeRoutine = null;
            });
        }

        public void CaseDelayInvokeRepeating()
        {
            _delayInvokeRoutine = _instance.DelayInvokeRepeating(0.5f, () => WLDebug.Log("Delay Invoke repeating tick."), 0.5f);
        }

        public void CaseCancelInvoke()
        {
            if (_delayInvokeRoutine != null)
            {
                _instance.CancelInvoke(_delayInvokeRoutine);
            }
        }

        public void CaseRegisterAnimationEvent()
        {
            _instance.RegisterAnimationEvent("MoveImageDone", () => WLDebug.Log("Animation Event: move image done"));

            var animator = _instance.gameObject.GetComponent<Animator>();
            animator.SetTrigger("MoveImage");
        }

        public void CaseUnregisterAnimationEvent()
        {
            _instance.UnregisterAnimationEvent("MoveImageDone");
            
        }

        private IEnumerator IECoroutineCase()
        {
            yield return new WaitForSeconds(1f);

            WLDebug.Log("IECoroutineCase");

            _routine = null;
        }
    }
}
