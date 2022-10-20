// @Author: lili
// @Date: 2021/4/27  20:01

using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace MJCommon
{
    public class MJRecordDesNode : WNode
    {
        private Text m_desTxt;
        private Text m_scoreTxt;

        public MJRecordDesNode(string des, string score)
        {
            m_desTxt = FindReference<Text>("des");
            m_scoreTxt = FindReference<Text>("score");
            SetData(des, score);
        }

        public void SetData(string des, string score)
        {
            m_desTxt.text = des;
            m_scoreTxt.text = score;
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("MJCommon/Images/RecordStatistics/Prefabs/RecordDesNode.prefab");
        //}
    }
}