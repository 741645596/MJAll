// MJSpace.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/5

using LitJson;
using Unity.Utility;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    public class MJSpace : BaseEntity
    {
        public MJTable table;
        public MJWallRoot wallRoot;
        public MJDeskCardRoot deskRoot;
        public MJMeldRoot meldRoot;
        public MJHandSetRoot handRoot;
        public MJWinCardRoot winCardRoot;
        public MJHandSetCamera handSetCamera;

        public MJSpace(MJSpaceConfig spaceConfig)
        {
            gameObject = new GameObject("MJSpace");

            InitSpace(spaceConfig);
        }

        public MJSpace(string config)
        {
            gameObject = new GameObject("MJSpace");

            InitSpace(config);
        }

        public virtual void InitSpace(MJSpaceConfig spaceConfig)
        {
            table = new MJTable();
            table.SetParent(this);

            wallRoot = new MJWallRoot(spaceConfig.wallConfig);
            wallRoot.SetParent(this);

            deskRoot = new MJDeskCardRoot(spaceConfig.deskConfig);
            deskRoot.SetParent(this);

            meldRoot = new MJMeldRoot(spaceConfig.meldConfig);
            meldRoot.SetParent(this);

            winCardRoot = new MJWinCardRoot(spaceConfig.winSetConfig);
            winCardRoot.SetParent(this);

            handSetCamera = new MJHandSetCamera();

            handRoot = new MJHandSetRoot(spaceConfig.handConfig);
            handRoot.SetParent(this);
            handRoot.InitializeHands(handSetCamera.camera, CameraUtil.GetMainCamera());
        }

        public void MockWinCardSet()
        {
            // 测试代码
            var config = new MJWinSetConfig()
            {
                position = new DVector3[]
                {
                    new DVector3(0.36f, 0, -0.36f),
                    new DVector3(0.5f, 0, 0.36f),
                    new DVector3(-0.36f, 0, 0.36f),
                    new DVector3(-0.36f, 0, -0.36f),
                },

                scale = new double[] { 1.12f, 1.12f, 1.12f, 1.12f }
            };

            var winroot = new MJWinCardRoot(config);
            winroot.SetParent(this);

            for (int i = 0; i < 4; i++)
            {
                var set = winroot.GetMJWinSet((MJOrientation)i);
                set.Append(1);
                set.Append(1);
                set.Append(1);
                set.Append(1);
                set.Append(2);
                set.Append(2);
                set.Append(2);
                set.Append(2);
                set.Append(3);
                set.Append(3);
            }
        }

        public void InitSpace(string config)
        {
            MJSpaceConfig spaceConfig = JsonMapper.ToObject<MJSpaceConfig>(config);

            InitSpace(spaceConfig);
        }

        public void ResumeOutCard(int o, int[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                deskRoot.PushCard((MJOrientation)o, cards[i]);
            }
        }

        public void ResumeHandCard(int o, int[] cards)
        {
            var hand = handRoot.GetMJHandSet((MJOrientation)o);
            hand.Reload(cards);
            hand.Refresh();
        }

        public void ResumeMeldCard(int o, int[] cards)
        {
            var args = new MJMeld.Args
            {
                showType = MJMeld.ShowType.Tiling,
                cardValues = new System.Collections.Generic.List<int>(cards),
                providerViewChairId = Chair.Invalid,
            };
            var meld = new MJMeld();
            meld.Initialize(args);
            meldRoot.PushMeld((MJOrientation)o, meld);
        }

        public void ResumeWinCard(int o, int[] cards)
        {
            var winSet = winCardRoot.GetMJWinSet((MJOrientation)o);
            if (winSet != null)
            {
                winSet.Reload(cards);
                winSet.Refresh();
            }
        }

        public void ResumeWallCard(MJWallConfig wallConfig, int bankerViewChairId, int dic1, int dic2, int count1, int count2)
        {
            wallRoot.InitializeWalls(wallConfig, bankerViewChairId, dic1, dic2);
            for (int i = 0; i < count1; i++)
            {
                wallRoot.Take();
            }
            for (int i = 0; i < count2; i++)
            {
                wallRoot.Take(true);
            }
        }

        public void ResumeWallCard(string wallConfigJson, int bankerViewChairId, int dic1, int dic2, int count1, int count2)
        {
            var wallConfig = JsonMapper.ToObject<MJWallConfig>(wallConfigJson);

            ResumeWallCard(wallConfig, bankerViewChairId, dic1, dic2, count1, count2);
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}