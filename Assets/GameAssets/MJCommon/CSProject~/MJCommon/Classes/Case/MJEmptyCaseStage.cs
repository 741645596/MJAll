// MJEmptyCaseStage.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/9
using System.Collections.Generic;
using LitJson;
using Unity.Core;
using UnityEngine;
using static MJCommon.MJMeld;

namespace MJCommon
{
    public class MJEmptyCaseStage : BaseCaseStage
    {
        private MJSpace space;
        private MJSpaceConfig spaceConfig;

        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);
            base.OnInitialize();

            new MJMainCamera();

            new MJDecoration().AddTo(root);

            ResetSpace();
        }

        private void ResetSpace()
        {
            if (space != null)
            {
                space.Destroy();
            }

            var asset = AssetsManager.Load<TextAsset>("MJCommon/MJ/mj_config", "space.json");
            WLDebug.Log(asset.text);
            spaceConfig = JsonMapper.ToObject<MJSpaceConfig>(asset.text);
            space = new MJSpace(spaceConfig);

            var walls = space.wallRoot.GetWallArray();
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].Reload(spaceConfig.wallConfig.stackCountPerSide[i]);
                walls[i].Refresh();
            }

            var ownHand = space.handRoot.GetMJHandSet(MJOrientation.Down);
            var width = ownHand.maxWidth;
            Vector3 pos = ownHand.gameObject.transform.position;
            pos.x += ownHand.GetHandCardAnchor() == HandCardAnchor.Left ? width * 0.5f : -width * 0.5f;
            space.handSetCamera.SetupCamera(pos, width, spaceConfig.handConfig.maxHandCardCount == 17 ? 0.2f : 0.165f);
        }

        public void CaseCountdown()
        {
            ResetSpace();

            var countDown = new MJCountDown();
            countDown.SetParent(space.gameObject);

            countDown.StartCountdown(2, () =>
            {
                Debug.Log("time up1!");

                countDown.StartCountdown(3, () =>
                {
                    Debug.Log("time up2!");
                });
            });
        }

        public void CaseHandCardDealAnimation()
        {
            ResetSpace();
            space.ResumeHandCard(0, new int[] { 1,1,1,2,3, 3, 5, 5, 5, 7, 8, 33, 34 });

            var handset = space.handRoot.GetMJHandSet(MJOrientation.Down);
            for (int i = 0; i < handset.count; i++)
            {
                var card = handset.GetCard(i);
                card.gameObject.SetActive(false);
            }
            MJAnimationHelper.PlayHandCardDealAnimation(handset, null);
        }

        public void CaseGangMeldAnimation()
        {
            ResetSpace();
            space.ResumeHandCard(0, new int[] { 3, 3, 5, 5, 5, 7, 8, 33, 34 });
            space.ResumeMeldCard(0, new int[] { 1, 2, 3 });

            var handset = space.handRoot.GetMJHandSet(MJOrientation.Down);
            var meld_stack = space.meldRoot.GetMJMeldStack(0);

            // 删除的手牌
            var delete_values = new List<int> {5, 5, 5};
            // 提牌动画
            MJAnimationHelper.PlaySelectHandCardAnimation(handset, delete_values, null,
            // 提牌结束回调
            () =>
            {
                // 删除提起的手牌
                handset.Remove(delete_values);
                // 手牌刷新动画
                MJAnimationHelper.PlayRefreshHandsetAnimation(handset);

                // 创建副子
                var meld_data = new Args()
                {
                    cardValues = new List<int>() { 5, 5, 5, 5 },
                    showType = ShowType.Stacking
                };
                var meld = new MJMeld(meld_data);
                meld_stack.Append(meld);

                // 副子移动动画
                MJAnimationHelper.PlayAddMeldAnimation(meld_stack);
            });
        }

        public void CaseNormalMeldAnimation()
        {
            ResetSpace();
            space.ResumeHandCard(0, new int[] { 3, 3, 5, 5, 7, 8, 33, 34 });
            space.ResumeMeldCard(0, new int[] { 1, 2, 3 });

            var handset = space.handRoot.GetMJHandSet(MJOrientation.Down);
            var meld_stack = space.meldRoot.GetMJMeldStack(0);

            // 删除的手牌
            var delete_values = new List<int> { 5, 5};

            MJAnimationHelper.PlaySelectHandCardAnimation(handset, delete_values, null, () =>
            {
                // 删除提起的手牌
                handset.Remove(delete_values);
                // 手牌刷新动画
                MJAnimationHelper.PlayRefreshHandsetAnimation(handset);

                // 创建副子
                var meld_data = new Args()
                {
                    cardValues = new List<int>() { 5, 5, 5},
                    showType = ShowType.Tiling
                };
                var meld = new MJMeld(meld_data);
                meld_stack.Append(meld);

                MJAnimationHelper.PlayAddMeldAnimation(meld_stack);
            });
        }

        public void CaseInsertAnimation()
        {
            ResetSpace();
            space.ResumeHandCard(0, new int[] { 1, 2, 3, 3, 3, 5, 5, 5, 6, 7, 8, 33, 34, 35 });

            var handset = space.handRoot.GetMJHandSet(MJOrientation.Down);

            // 模拟换三张，扔出3张牌
            handset.Remove(10);
            handset.Remove(9);
            handset.Remove(8);

            MJAnimationHelper.PlayInsert3Cards(handset, new List<int>() { 13, 4, 5 }, () =>
            {
                Debug.Log("over");
            });
        }

        public void CaseExchangeAnimation()
        {
            ResetSpace();

            var anim = new MJExchangeAnimation();
            anim.SetParent(space.gameObject);

            anim.ShowCard(0);
            anim.ShowCard(1);
            anim.ShowCard(2);
            anim.ShowCard(3);
            anim.Play(ExchangeCardType.Cross, () => anim.RemoveFromParent());
        }

        public void CasePlayOutCardAnimation()
        {
            ResetSpace();
            space.wallRoot.InitializeWalls(spaceConfig.wallConfig, 0, 1, 1);
            space.ResumeHandCard(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 17, 18, 19, 21, 20 });
            space.ResumeHandCard(1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            var self = space.handRoot.GetMJHandSet(MJOrientation.Down);
            var outIndex = 5;
            self.Remove(outIndex);
            var seq = MJAnimationHelper.PlayInsertCardAnimation(self);
            seq.onComplete += () =>
            {
                //self.Refresh();
                Debug.Log("finish");
            };
        }

        public void CasePlayLiftAnimation()
        {
            ResetSpace();
            MJAnimationHelper.PlayLiftAnimation(space);
        }

        public void CaseSendCardAnimation()
        {
            ResetSpace();
            space.wallRoot.InitializeWalls(spaceConfig.wallConfig, 0, 1, 1);
            space.ResumeHandCard(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5 });
            space.ResumeHandCard(1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            var self = space.handRoot.GetMJHandSet(MJOrientation.Down);
            MJAnimationHelper.PlaySendCardAnimation(self);
        }

        //public void CasePlayDealAnimation()
        //{
        //    ResetSpace();
        //    space.wallRoot.InitializeWalls(spaceConfig.wallConfig, 0, 1, 1);
        //    space.ResumeHandCard(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5 });
        //    space.ResumeHandCard(1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4});
        //    space.ResumeHandCard(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4});
        //    space.ResumeHandCard(3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4});
        //    MJAnimationHelper.PlayDealAnimation(space, 0);
        //}

        public void CasePlayDealAnimation()
        {
            ResetSpace();
            space.wallRoot.InitializeWalls(spaceConfig.wallConfig, 0, 1, 1);
            space.ResumeHandCard(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5 });
            space.ResumeHandCard(1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            MJAnimationHelper.PlayDealAnimation2(space, 0, null, null, ()=>
            {
                Debug.Log("finish");
            });
        }

        public void CaseCardMoAnimation()
        {
            var card = new MJCard(255);
            card.HideShadow();
            card.gameObject.transform.position = new Vector3(0.435f, MJDefine.MJCardSizeY * 0.5f, -0.412f);
            card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "card_mo.asset", (t) =>
            {
                card.Destroy();
            });
        }

        public void CaseCardHuAnimation()
        {
            var card = new MJCard(1);
            card.HideShadow();
            card.gameObject.transform.position = new Vector3(0.435f, MJDefine.MJCardSizeY * 0.5f, -0.412f);
            card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "card_hu.asset", (t)=>
            {
                card.ShowShadow();
            });
        }


        public void CasePlayDiceAnimation()
        {
            MJDice dice = new MJDice(new System.Collections.Generic.List<int> { 2, 3});

            MJAnimationHelper.PlayDiceAnimation(dice);
        }

        public void CasePlayOutCardAnimation2()
        {
            MJCard card0 = new MJCard(1);
            card0.gameObject.transform.localPosition = new Vector3(0f - MJDefine.MJCardSizeX, MJDefine.MJCardSizeY * 0.5f, -0.15f);

            MJCard card = new MJCard(1);
            card.gameObject.transform.localPosition = new Vector3(0.2f, 0.2f, -0.4f);
            MJAnimationHelper.PlayOutCardAnimation(card, new Vector3(0f, MJDefine.MJCardSizeY * 0.5f, -0.15f));
        }

        public void CasePlayShowHandCardSet()
        {
            ResetSpace();
            space.wallRoot.InitializeWalls(spaceConfig.wallConfig, 0, 1, 1);
            space.ResumeHandCard(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5 });
            space.ResumeHandCard(1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });
            space.ResumeHandCard(3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4 });

            for (int i = 0; i < 4; i++)
            {
                var handset = space.handRoot.GetMJHandSet((MJOrientation)i);
                MJAnimationHelper.PlayShowHandCardAnimation(handset, () =>
                {
                    Debug.Log("finish");
                });
            }
        }
    }
}
