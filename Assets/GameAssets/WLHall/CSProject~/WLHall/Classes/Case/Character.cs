// Character.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/18

using System.Collections.Generic;
using Unity.Core;
using UnityEngine;
using WLCore.Entity;
using Object = UnityEngine.Object;

namespace WLHall
{
    public class Character : BaseEntity
    {
        protected Dictionary<string, Transform> allBones = new Dictionary<string, Transform>();
        protected readonly Dictionary<int, GameObject> equipment = new Dictionary<int, GameObject>();

        public Character()
        {
            Init("WLHall/Dress", "Bip001.prefab");
        }

        protected void Init(string asset, string skeleton_res)
        {
            gameObject = new GameObject("Character");
            gameObject.AddComponent<Animator>();

            var skeleton_prefab = AssetsManager.Load<GameObject>(asset, skeleton_res);
            var skeleton = Object.Instantiate(skeleton_prefab, gameObject.transform);
            skeleton.name = skeleton.name.Replace("(Clone)", "");

            allBones.Clear();

            var children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (var t in children)
            {
                allBones.Add(t.name, t);
            }
        }

        public void Equip(int partIndex, string asset, string key)
        {
            HideParts(partIndex);
            var prefab = AssetsManager.Load<GameObject>(asset, key);
            if(prefab == null)
            {
                WLDebug.LogWarning($"Character Equip prefab is null {partIndex}:{key}");
                return;
            }

            if (equipment.ContainsKey(partIndex))
            {
                equipment[partIndex] = prefab;
            }
            else
            {
                equipment.Add(partIndex, prefab);
            }

            Rebuild(partIndex);
        }

        private void HideParts(int partIndex)
        {
            if(equipment.ContainsKey(partIndex) == false)
            {
                return;
            }

            var prefab = equipment[partIndex];
            var name = prefab.name;

            var tran = gameObject.transform.Find(name);

            if (tran != null)
            {
                tran.gameObject.SetActive(false);
            }
        }


        protected void Rebuild(int partIndex)
        {
            if(equipment.ContainsKey(partIndex) == false)
            {
                return;
            }

            var prefab = equipment[partIndex];
            if (prefab == null)
            {
                return;
            }

            GameObject part;
            var t = gameObject.transform.Find(prefab.name);
            if (t != null)
            {
                part = t.gameObject;
                part.SetActive(true);
            }
            else
            {
                part = Object.Instantiate(prefab);
                part.name = prefab.name;
                // 将SkinnedMeshRenderer父对象设置为骨骼根节点
                part.transform.parent = gameObject.transform;
            }


            var skins = part.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int k = 0; k < skins.Length; k++)
            {
                var skin = skins[k];
                List<Transform> bones = new List<Transform>();
                Transform[] skinBones = skin.bones;

                for (int i = 0; i < skinBones.Length; i++)
                {
                    if (allBones.ContainsKey(skinBones[i].name))
                    {
                        bones.Add(skinBones[i]);
                    }
                }
                skin.bones = bones.ToArray();
            }
        }
        /// <summary>
        /// 根据equipment的值，重新生成模型
        /// </summary>
        protected void Rebuild()
        {
            foreach (var item in equipment)
            {
                Rebuild(item.Key);
            }
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}
