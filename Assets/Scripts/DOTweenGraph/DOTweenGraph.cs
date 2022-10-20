// DOTweenGraph.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "DOTweenGraph", fileName ="DOTweenGraph")]
public class DOTweenGraph : NodeGraph {

    private readonly Dictionary<string, Action> events = new Dictionary<string, Action>();

    public DORootNode GetRootNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if(nodes[i] is DORootNode)
            {
                return nodes[i] as DORootNode;
            }
        }
        return null;
    }

    public void SetEvent(string key, Action action)
    {
        if(events.ContainsKey(key))
        {
            events[key] = action;
        }
        else
        {
            events.Add(key, action);
        }
    }

    public Action GetEvent(string key)
    {
        if(events.ContainsKey(key))
        {
            return events[key];
        }

        return null;
    }

    public void ClearEvents()
    {
        events.Clear();
    }
}