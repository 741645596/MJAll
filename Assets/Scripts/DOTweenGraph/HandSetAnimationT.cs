using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class HandSetAnimation
{
    public DOTweenGraph graph;
    public int step;
    public float delay;
}

public class HandSetAnimationT : MonoBehaviour
{
    private List<Transform> transforms = new List<Transform>();

    public List<HandSetAnimation> graphList;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform tran in transform)
        {
            transforms.Add(tran);
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        var root_sequence = DOTween.Sequence();
        for (int k = 0; k < graphList.Count; k++)
        {
            var ani = graphList[k];
            var graph = ani.graph;
            int step = ani.step;
            float delay = ani.delay;
            if(step == 0)
            {
                step = transforms.Count;
            }
            var sequence = DOTween.Sequence();

            for (int i = 0; i < transforms.Count; i+=step)
            {
                WLDebug.Log($"i={i}");

                for (int j = i; j < Mathf.Min(i+step, transforms.Count); j++)
                {
                    WLDebug.Log($"j={j}");
                    var tran = transforms[j];
                    var root = graph.GetRootNode();
                    var tweenNode = root.GetOutputPort("next").Connection.node as DOBaseNode;
                    var sigle_card_sequence = DOTween.Sequence();
                    sigle_card_sequence.SetDelay(delay * Mathf.Floor(i / (float)step));

                    while (tweenNode != null)
                    {
                        var tween = tweenNode.GenerateTween(tran);
                        if (tween != null)
                        {
                            sigle_card_sequence.Append(tween);
                        }
                        tweenNode = tweenNode.GetNextNode();
                    }
                    sequence.Join(sigle_card_sequence);
                }
            }
            root_sequence.Append(sequence);
        }

        root_sequence.Play();
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            var tran = transforms[i];
            tran.localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
}
