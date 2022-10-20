// MJSpaceMockData.cs
// Author: shihongyang shihongyang@Unity.com
// Data: 2021/8/6

using System.Collections.Generic;
using UnityEngine;

namespace MJEditor
{
    [CreateAssetMenu(menuName = "Assets/Create MJSpace MockData")]
    public class MJSpaceMockSO : ScriptableObject
    {
        public List<string> deskCards;
        public List<string> handCards;
        public List<string> winCards;

        public List<string> meldCards0;
        public List<string> meldCards1;
        public List<string> meldCards2;
        public List<string> meldCards3;

        public int dice1;
        public int dice2;
        public int takeWallCount;
        public int takeWallCountBack;
    }
}