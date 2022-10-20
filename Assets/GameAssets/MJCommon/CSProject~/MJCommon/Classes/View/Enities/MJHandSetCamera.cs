// MJHandSetCamera.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/13

using System;
using Unity.Widget;
using UnityEngine;
using WLCore.Entity;
using WLCore.Helper;

namespace MJCommon
{
    public class MJHandSetCamera : BaseEntity
    {
        /// <summary>
        /// 相机
        /// </summary>
        public Camera camera { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cardSet">手牌对象</param>
        public MJHandSetCamera() : base()
        {
            var go = GameObject.Find("HandCardCamera");
            if (go == null)
            {
                go = ObjectHelper.Instantiate("MJCommon/MJ/mj_env", "prefabs/hand_camera.prefab");
                go.name = "HandCardCamera";
            }
            gameObject = go;
            camera = go.GetComponent<Camera>();
            camera.cullingMask = LayerMask.GetMask(MJDefine.MJHandMask);
        }

        /// <summary>
        /// 手牌相机适配，宽度固定
        /// </summary>
        /// <param name="point">目标视野下边缘中心点的世界坐标</param>
        /// <param name="width">目标视野的宽度</param>
        public void SetupCamera(Vector3 point, float width, float sizeLimit)
        {
            if (camera == null || camera.orthographic == false)
            {
                return;
            }

            float size = width * 0.5f / display.aspect;
            size = Mathf.Max(sizeLimit, size);

            var angle = 15f;
            var zoffset = 0.2f;
            float yoffset = Mathf.Tan(Mathf.Deg2Rad * angle) * zoffset;

            float x = point.x;
            float y = point.y + size + yoffset;
            float z = point.z - zoffset;
            camera.transform.position = new Vector3(x, y, z);
            camera.transform.eulerAngles = new Vector3(angle, 0, 0);
            camera.orthographicSize = size;
        }

        // 实现抽象方法，创建GameObject的操作在构造方法内完成
        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}
