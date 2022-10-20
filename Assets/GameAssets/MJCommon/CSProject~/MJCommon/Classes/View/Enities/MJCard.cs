// MJCard.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/14

using UnityEngine;
using Unity.Core;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 单张麻将牌
    /// </summary>
    public class MJCard : BaseEntity
    {
        /// <summary>
        /// 通用胡牌提示小图标类型
        /// </summary>
        public enum HintMarkType
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal,
            /// <summary>
            /// 大
            /// </summary>
            Bigger,
            /// <summary>
            /// 多
            /// </summary>
            More,
            /// <summary>
            /// 优
            /// </summary>
            Better
        }

        /// <summary>
        /// 麻将牌值
        /// </summary>
        public int cardValue;

        /// <summary>
        /// 麻将牌本体
        /// </summary>
        public GameObject body;

        /// <summary>
        /// 阴影
        /// </summary>
        protected GameObject shadow;

        /// <summary>
        /// 渲染器组件
        /// </summary>
        protected MeshRenderer renderer;


        public MJCard() : base()
        {
        }

        /// <summary>
        /// 构造方法，创建一个麻将牌对象
        /// </summary>
        /// <param name="value">麻将牌值</param>
        public MJCard(int value) : base()
        {
            Initialize(value);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="value"></param>
        public virtual void Initialize(int value)
        {
            cardValue = value;
            var key = cardValue == Card.Rear ? "prefabs/mj_card_2_rear.prefab" : "prefabs/mj_card_2.prefab";
            var prefab = AssetsManager.Load<GameObject>("MJCommon/MJ/mj_cards", key);

            if (prefab != null)
            {
                gameObject = new GameObject($"mjcard_{cardValue}");
                body = Object.Instantiate(prefab);
                body.transform.SetParent(gameObject.transform, false);
                renderer = cardValue == Card.Rear ? body.GetComponentInChildren<MeshRenderer>() : body.GetComponent<MeshRenderer>();
                SetupMaterial();
            }
            else
            {
                Debug.Log("MJCard prefab is null key=" + key);
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                var mat = renderer.materials[i];
                if (mat != null)
                {
                    mat.SetColor("_BaseColor", color);
                }
            }
        }

        /// <summary>
        /// 设置材质
        /// </summary>
        /// <param name="assetname"></param>
        /// <param name="card_key"></param>
        /// <param name="back_key"></param>
        public void SetMaterial(string assetname, string card_key, string back_key)
        {
            var card_mat = AssetsManager.Load<Material>(assetname, card_key);
            var back_mat = AssetsManager.Load<Material>(assetname, back_key);

            renderer.materials = new Material[] {back_mat, card_mat};
            SetupMaterial();
        }

        /// <summary>
        /// 显示阴影
        /// </summary>
        public void ShowShadow()
        {
            if (shadow == null)
            {
                var key = "prefabs/mj_projection.prefab";
                var prefab = AssetsManager.Load<GameObject>("MJCommon/MJ/mj_cards", key);
                shadow = Object.Instantiate(prefab);
                shadow.transform.SetParent(gameObject.transform, false);
                shadow.transform.localPosition = new Vector3(0, -MJDefine.MJCardSizeY * 0.5f, 0);
                shadow.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                shadow.SetActive(true);
            }
        }

        /// <summary>
        /// 隐藏阴影
        /// </summary>
        public void HideShadow()
        {
            if(shadow != null)
            {
                shadow.SetActive(false);
            }
        }

        /// <summary>
        /// 是否显示了阴影
        /// </summary>
        /// <returns></returns>
        public bool IsShadowShown()
        {
            return shadow != null && shadow.gameObject.activeSelf;
        }

        /// <summary>
        /// 朝后摆放，面向自己
        /// </summary>
        public void TowardBack()
        {
            body.transform.localRotation = Quaternion.Euler(-90, 0, 0);

            if(shadow != null)
            {
                var pos = shadow.transform.localPosition;
                pos.y = -MJDefine.MJCardSizeZ * 0.5f;
                shadow.transform.localPosition = pos;
                shadow.transform.localScale = new Vector3(1, 1, 0.5f);
            }
        }

        /// <summary>
        /// 朝上摆放
        /// </summary>
        public void TowardUp()
        {
            body.transform.localRotation = Quaternion.Euler(0, 0, 0);

            if (shadow != null)
            {
                shadow.transform.localPosition = new Vector3(0, -MJDefine.MJCardSizeY * 0.5f, 0);
                shadow.transform.localEulerAngles = Vector3.zero;
                shadow.transform.localScale = new Vector3(1, 1, 1f);
            }
        }

        /// <summary>
        /// 朝上摆放，绕y轴旋转180
        /// </summary>
        public void TowardUpInvert()
        {
            body.transform.localRotation = Quaternion.Euler(0, 180, 0);

            if (shadow != null)
            {
                shadow.transform.localPosition = new Vector3(0, -MJDefine.MJCardSizeY * 0.5f, 0);
                shadow.transform.localEulerAngles = new Vector3(0, 0, 0);
                shadow.transform.localScale = new Vector3(1, 1, 1f);
            }
        }

        /// <summary>
        /// 朝下摆放
        /// </summary>
        public void TowardDown()
        {
            body.transform.localRotation = Quaternion.Euler(180, 0, 0);

            if (shadow != null)
            {
                shadow.transform.localPosition = new Vector3(0, -MJDefine.MJCardSizeY * 0.5f, 0);
                shadow.transform.localEulerAngles = Vector3.zero;
                shadow.transform.localScale = new Vector3(1, 1, 1f);
            }
        }

        /// <summary>
        /// 获取麻将牌尺寸
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSize()
        {
            return new Vector3(MJDefine.MJCardSizeX, MJDefine.MJCardSizeY, MJDefine.MJCardSizeZ);
        }

        /// <summary>
        /// 添加小图标
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject AddCardMark(string asset, string key, string name = "card_mark")
        {
            var obj = new GameObject(name);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetsManager.Load<Sprite>(asset, key);
            obj.transform.SetParent(body.transform, false);
            obj.layer = body.layer;
            return obj;
        }

        /// <summary>
        /// 获取小图标
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetCardMark(string name)
        {
            var mark = body.FindReference(name);
            if (mark != null)
            {
                return mark.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 显示胡牌提示小图标
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject ShowHintMark(HintMarkType type)
        {
            var asset = "MJCommon/MJ/mj_ui_atlas";
            var key = $"card_mark/hint_mark/{(int)type}.png";

            var mark = GetCardMark("hint_mark");
            if (mark != null)
            {
                mark.gameObject.SetActive(true);
                mark.GetComponent<SpriteRenderer>().sprite = AssetsManager.Load<Sprite>(asset, key);
                return mark;
            }

            mark = AddCardMark(asset, key, "hint_mark");
            mark.transform.localEulerAngles = new Vector3(90, 0, 0);
            mark.transform.localScale = new Vector3(0.05f, 0.05f, 1f);
            mark.transform.localPosition = new Vector3(0, 0, MJDefine.MJCardSizeZ * 0.9f);
            return mark;
        }

        /// <summary>
        /// 隐藏胡牌提示小图标
        /// </summary>
        public void HideHintMark()
        {
            GetCardMark("hint_mark")?.SetActive(false);
        }

        /// <summary>
        /// 显示癞子小图标
        /// </summary>
        public GameObject ShowJokerMark()
        {
            var mark = GetCardMark("joker_mark");
            if (mark != null)
            {
                mark.gameObject.SetActive(true);
                return mark.gameObject;
            }

            var obj = AddCardMark("MJCommon/MJ/mj_ui_atlas", "card_mark/joker.png", "joker_mark");
            obj.transform.localEulerAngles = new Vector3(90, 0, 0);
            obj.transform.localScale = new Vector3(0.05f, 0.05f, 1f);
            float dx = MJDefine.MJCardSizeX, dy = MJDefine.MJCardSizeY, dz = MJDefine.MJCardSizeZ;
            obj.transform.localPosition = new Vector3(-dx * 0.5f, dy * 0.501f, dz * 0.495f);
            return obj;
        }

        /// <summary>
        /// 隐藏癞子小图标
        /// </summary>
        public void HideJokerMark()
        {
            GetCardMark("joker_mark")?.SetActive(false);
        }

        /// <summary>
        /// 显示定缺小图标
        /// </summary>
        public GameObject ShowLackMark()
        {
            var mark = GetCardMark("lack_mark");
            if (mark != null)
            {
                mark.gameObject.SetActive(true);
                return mark.gameObject;
            }

            var obj = AddCardMark("MJCommon/MJ/mj_ui_atlas", "card_mark/lack.png", "lack_mark");
            obj.transform.localEulerAngles = new Vector3(90, 0, 0);
            obj.transform.localScale = new Vector3(0.051f, 0.051f, 1f);
            float dx = MJDefine.MJCardSizeX, dy = MJDefine.MJCardSizeY, dz = MJDefine.MJCardSizeZ;
            obj.transform.localPosition = new Vector3(-dx * 0.5f, dy * 0.501f, dz * 0.495f);
            return obj;
        }

        /// <summary>
        /// 隐藏定缺小图标
        /// </summary>
        public void HideLackMark()
        {
            GetCardMark("lack_mark")?.SetActive(false);
        }

        /// <summary>
        /// 回收重置
        /// </summary>
        public virtual void OnRecycle()
        {
            gameObject.SetLayer(LayerMask.NameToLayer("Default"));
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            body.transform.localScale = Vector3.one;
            SetColor(Color.white);
            TowardUp();
            HideHintMark();
            HideJokerMark();
            HideLackMark();
            HideShadow();
        }

        /// <summary>
        /// 设置牌花uv偏移
        /// </summary>
        protected void SetupMaterial()
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                var mat = renderer.materials[i];
                if (mat != null && mat.name.StartsWith("mj_card"))
                {
                    if (MJDefine.MJCardMatOffsets.ContainsKey(cardValue) == false)
                    {
                        return;
                    }
                    var offset = MJDefine.MJCardMatOffsets[cardValue];
                    mat.SetFloat("_XTitles", offset.x);
                    mat.SetFloat("_YTitles", offset.y);
                }
            }
        }

        /// 实现抽象方法，创建GameObject的操作在构造方法内完成
        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}