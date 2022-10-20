// AnimationTest.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;
using Unity.Core;
using UnityEngine;

public class AnimationT : MonoBehaviour
{
    public DOTweenGraph graph;
    public float repeatTime;

    [ContextMenu("InitCards")]
    public void InitCards()
    {
        // AssetsManager.SetLoadType(AssetsManager.LoadType.Local);
        var prefab = AssetsManager.Load<GameObject>("MJCommon/MJ/mj_cards", "prefabs/mj_card.prefab");
        foreach (Transform item in transform.GetComponentsInChildren<Transform>())
        {
            WLDebug.Log("transform name = ", item.name);
            if(item.name.StartsWith("Bone"))
            {
                var card = Instantiate(prefab, item);
                card.transform.localPosition = new Vector3(-0.02745409f, 0, 0);
                card.transform.localRotation = Quaternion.Euler(0, 90, 90);
            }
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        transform.transform.localPosition = Vector3.zero;

        var root = graph.GetRootNode();
        var sequence = DOTween.Sequence();
        var tweenNode = root.GetOutputPort("next").Connection.node as DOBaseNode;

        while (tweenNode != null)
        {
            var tween = tweenNode.GenerateTween(transform);
            if(tween != null)
            {
                sequence.Append(tween);
            }
            tweenNode = tweenNode.GetNextNode();
        }

        sequence.Play();
    }
}
