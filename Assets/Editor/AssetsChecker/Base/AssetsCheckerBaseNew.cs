// AssetsCheckerBase.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/08


using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class AssetsCheckerBaseNew<T> where T : AssetInfoBase
{
    protected List<AssetRuleBase>               _rules      = new List<AssetRuleBase>();
    protected Dictionary<string, AssetRuleBase> _dictionary = new Dictionary<string, AssetRuleBase>();
    protected List<T>                           _assetInfoList;

    public AssetsCheckerBaseNew()
    {
    }

    public void AddRule(string ruleName, AssetRuleBase rule)
    {
        _dictionary.Add(ruleName, rule);
    }

    // public List<AssetRuleBase> GetRuleList()
    // {
    //     return _rules;
    // }

    public Dictionary<string, AssetRuleBase> GetRuleDictionary()
    {
        return _dictionary;
    }

    /// <summary>
    /// 遍历所有资源信息，
    /// 违反规则，
    /// 且打开需要修复功能，
    /// 的所有资源全修复
    /// </summary>
    public void FixAll()
    {
        for (int i = 0; i < _assetInfoList.Count; i++)
        {
            var  info =  _assetInfoList[i];
            bool rule = Check(info);
            if (rule)
            {
                continue;
            }

            if (!CanFix(info))
            {
                continue;
            }

            Fix(info);
        }
    }

    public List<T> GetAssetInfoList()
    {
        if (_assetInfoList == null)
        {
            _assetInfoList = CollectAssetInfoList();
        }

        return _assetInfoList;
    }

    public abstract List<T> CollectAssetInfoList();

    public void Fix(T info)
    {
        foreach (var rule in _dictionary)
        {
            if (rule.Value.Pass(info) == false)
            {
                rule.Value.Fix(info);
            }
        }

        // for (int i = 0; i < _rules.Count; i++)
        // {
        //     var rule = _rules[i];
        //     if (rule.Pass(info) == false)
        //     {
        //         rule.Fix(info);
        //     }
        // }
    }

    public bool Check(T info)
    {
        var isPass = true;
        // for (int i = 0; i < _rules.Count; i++)
        // {
        //     var rule = _rules[i];
        //     isPass &= rule.Pass(info);
        // }

        foreach (var rule in _dictionary)
        {
            isPass &= rule.Value.Pass(info);
        }

        return isPass;
    }


    public bool CanFix(T info)
    {
        var canFix = false;
        // for (int i = 0; i < _rules.Count; i++)
        // {
        //     var rule = _rules[i];
        //     if (rule.CanFix() == false)
        //     {
        //         continue;
        //     }
        //
        //     canFix |= !rule.Pass(info);
        // }

        foreach (var rule in _dictionary)
        {
            if (rule.Value.CanFix() == false)
            {
                continue;
            }

            canFix |= !rule.Value.Pass(info);
        }

        return canFix;
    }

    public string GetCheckInfo(T info)
    {
        var sb = new StringBuilder();

        // for (int i = 0; i < _rules.Count; i++)
        // {
        //     var rule = _rules[i];
        //     if (rule.Pass(info))
        //     {
        //         continue;
        //     }
        //
        //     sb.Append(" ");
        //     sb.Append(rule.Info());
        // }

        foreach (var rule in _dictionary)
        {
            if (rule.Value.Pass(info))
            {
                continue;
            }

            sb.Append(" ");
            sb.Append(rule.Value.Info());
        }

        return sb.ToString();
    }
}
