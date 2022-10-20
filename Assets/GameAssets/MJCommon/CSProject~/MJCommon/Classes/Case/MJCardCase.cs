// MJCardCase.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/29

using UnityEngine;
using Unity.Core;
using System.Collections.Generic;
using Unity.Utility;

namespace MJCommon
{
    public class MJCardCase : BaseCaseStage
    {
        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();
        }

        public void CaseDOTweenGraph2()
        {
            var card = new MJCard(1);

            card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween",
                "DOTweenGraph.asset", new System.Collections.Generic.Dictionary<string, System.Action>()
                {
                    { "event1", ()=>{
                        Debug.Log("event1");
                    } },
                    { "event2", ()=>{
                        Debug.Log("event2");
                    } },
                });
        }


        public void CaseMJCard()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            var z1 = 0.2f;
            // 4个玩家方位的朝向
            for (int i = 0; i < (int)MJOrientation.Count; i++)
            {
                var orientation = (MJOrientation)i;
                z1 -= 0.12f;
                var mj1 = new MJCard(1);
                mj1.TowardBack();
                mj1.gameObject.transform.position = new Vector3(-0.6f, 0, z1);
                mj1.gameObject.transform.SetParent(card_root.transform);
                mj1.SetColor(new Color(1, 1, 0.8f));
                var mj2 = new MJCard(1);
                mj2.TowardUp();
                mj2.gameObject.transform.position = new Vector3(-0.45f, 0, z1);
                mj2.gameObject.transform.SetParent(card_root.transform);
                var mj3 = new MJCard(1);
                mj3.TowardDown();
                mj3.gameObject.transform.position = new Vector3(-0.3f, 0, z1);
                mj3.gameObject.transform.SetParent(card_root.transform);
            }

            var cardvalues = new int[]{
                1,2,3,4,5,6,7,8,9,
                17,18,19,20,21,22,23,24,25,
                33,34,35,36,37,38,39,40,41,
                49,50,51,52,53,54,55,
                65,66,67,68,69,70,71,72,255,0
            };

            var z = 0.2f;
            var x = 0f;
            for (int i = 0; i < cardvalues.Length; i++)
            {
                var card = new MJCard(cardvalues[i]);
                if (i % 9 == 0)
                {
                    z -= MJDefine.MJCardSizeZ;
                    x = 0;
                }
                x += MJDefine.MJCardSizeX;

                var pos = new Vector3(x, 0, z);
                card.gameObject.transform.localPosition = pos;
                card.gameObject.transform.SetParent(card_root.transform);
            }
        }

        public void CaseMJMeld()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            //var meld1 = new MJMeldConcealedKong(new int[] { 5, 5, 5, 5 });
            //meld1.Style2();
            //meld1.gameObject.transform.localPosition = new Vector3(-0.4f, 0, 0);
            //meld1.gameObject.transform.SetParent(card_root.transform);

            //var meld2 = new MJMeldConcealedKong(new int[] { 5, 5, 5, 5 });
            //meld2.Style3();
            //meld2.gameObject.transform.localPosition = new Vector3(-0.15f, 0, 0);
            //meld2.gameObject.transform.SetParent(card_root.transform);

            //var meld4 = new MJMeldConcealedKong(new int[] { 5, 5, 5, 5 });
            //meld4.Style4();
            //meld4.gameObject.transform.localPosition = new Vector3(0.2f, 0, 0);
            //meld4.gameObject.transform.SetParent(card_root.transform);

            //var meld5 = new MJMeldPong(new int[] { 5, 5, 5 }, 1);
            //meld5.gameObject.transform.localPosition = new Vector3(-0.4f, 0, 0.2f);
            //meld5.gameObject.transform.SetParent(card_root.transform);

            //var meld6 = new MJMeldPong(new int[] { 5, 5, 5 }, 2);
            //meld6.gameObject.transform.localPosition = new Vector3(-0.1f, 0, 0.2f);
            //meld6.gameObject.transform.SetParent(card_root.transform);

            //var meld7 = new MJMeldPong(new int[] { 5, 5, 5 }, 3);
            //meld7.gameObject.transform.localPosition = new Vector3(0.2f, 0, 0.2f);
            //meld7.gameObject.transform.SetParent(card_root.transform);

            //var meld8 = new MJMeldPong(new int[] { 5, 5, 5 }, 0);
            //meld8.gameObject.transform.localPosition = new Vector3(-0.4f, 0, -0.2f);
            //meld8.gameObject.transform.SetParent(card_root.transform);

            //var meld9 = new MJMeldPong(new int[] { 5, 5, 5 }, 3);
            //meld9.CollectedKong();
            //meld9.gameObject.transform.localPosition = new Vector3(-0.1f, 0, -0.2f);
            //meld9.gameObject.transform.SetParent(card_root.transform);


            //var meld10 = new MJMeldPong(new int[] { 5, 5, 5 }, 2);
            //meld10.CollectedKong();
            //meld10.gameObject.transform.localPosition = new Vector3(0.2f, 0, -0.2f);
            //meld10.gameObject.transform.SetParent(card_root.transform);

            //Vector3[] pos_array = new Vector3[(int)MJOrientation.Count]
            //{
            //    new Vector3(0.55f, 0, -0.4f),
            //    new Vector3(0.6f, 0, 0.52f),
            //    new Vector3(-0.55f, 0, 0.4f),
            //    new Vector3(-0.6f, 0, -0.52f)
            //};
            //// 4个玩家方位的朝向
            //for (int i = 0; i < (int)MJOrientation.Count; i++)
            //{
            //    var orientation = (MJOrientation)i;

            //    var stack = new MJMeldStack();
            //    stack.gameObject.transform.SetParent(card_root.transform);
            //    stack.gameObject.transform.localPosition = pos_array[i];
            //    stack.SetOrientation(orientation);
            //    stack.PushMeld(new MJMeldChow(new int[] { 1, 2, 3 }));
            //    stack.PushMeld(new MJMeldExposedKong(new int[] { 5, 5, 5, 5 }));
            //    stack.PushMeld(new MJMeldConcealedKong(new int[] { 5, 5, 5, 5 }));
            //    stack.UpdateConfig(new MJMeldConfig()
            //    {
            //        direction = MeldStackDirection.RightToLeft,
            //        interval = 0.02f
            //    });
            //}
        }

        public void CaseMJWall()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            var stack1 = new MJStack();
            stack1.gameObject.transform.localPosition = new Vector3(-0.4f, 0, 0);
            stack1.gameObject.transform.SetParent(card_root.transform);

            var stack2 = new MJStack();
            stack2.Initialize(4);
            stack2.gameObject.transform.localPosition = new Vector3(-0.3f, 0, 0);
            stack2.gameObject.transform.SetParent(card_root.transform);

            var stack3 = new MJStack();
            stack3.RemoveLast();
            stack3.gameObject.transform.localPosition = new Vector3(-0.2f, 0, 0);
            stack3.gameObject.transform.SetParent(card_root.transform);

            var stack4 = new MJStack();
            stack4.RemoveLast();
            stack4.RemoveLast();
            stack4.gameObject.transform.localPosition = new Vector3(-0.1f, 0, 0);
            stack4.gameObject.transform.SetParent(card_root.transform);

            var stack5 = new MJStack();
            stack5.RemoveLast();
            stack5.RemoveLast();
            stack5.Append(0);
            stack5.gameObject.transform.localPosition = new Vector3(0f, 0, 0);
            stack5.gameObject.transform.SetParent(card_root.transform);

            var stack6 = new MJStack();
            stack6.RemoveLast();
            stack6.RemoveLast();
            stack6.Append(0);
            stack6.Append(2);
            stack6.gameObject.transform.localPosition = new Vector3(0.1f, 0, 0);
            stack6.gameObject.transform.SetParent(card_root.transform);

            var stack7 = new MJStack();
            stack7.RemoveLast();
            stack7.RemoveLast();
            stack7.Append(2);
            stack7.Append(1);
            stack7.gameObject.transform.localPosition = new Vector3(0.2f, 0, 0);
            stack7.gameObject.transform.SetParent(card_root.transform);

            var stack8 = new MJStack();
            stack8.RemoveLast();
            stack8.RemoveLast();
            stack8.Append(0);
            stack8.Append(0);
            stack8.gameObject.transform.localPosition = new Vector3(0.3f, 0, 0);
            stack8.gameObject.transform.SetParent(card_root.transform);

            Vector3[] pos_array = new Vector3[(int)MJOrientation.Count]
            {
                new Vector3(0.4f, 0, -0.5f),
                new Vector3(0.6f, 0, 0.4f),
                new Vector3(-0.4f, 0, 0.5f),
                new Vector3(-0.6f, 0, -0.4f)
            };
            // 4个玩家方位的朝向
            for (int i = 0; i < (int)MJOrientation.Count; i++)
            {
                var orientation = (MJOrientation)i;

                var wall = new MJWall();
                wall.Initialize(13);
                wall.gameObject.transform.SetParent(card_root.transform);
                wall.gameObject.transform.localPosition = pos_array[i];
                wall.SetOrientation(orientation);
            }
        }

        public void CaseMJDeskCardSet()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            Vector3[] pos_array = new Vector3[(int)MJOrientation.Count]
            {
                new Vector3(-0.11f, 0, -0.11f),
                new Vector3(0.11f, 0, -0.11f),
                new Vector3(0.11f, 0, 0.11f),
                new Vector3(-0.11f, 0, 0.11f)
            };
            // 4个玩家方位的朝向
            for (int i = 0; i < (int)MJOrientation.Count; i++)
            {
                var orientation = (MJOrientation)i;
                var set = new MJDeskCardSet();
                set.Reload(GetRandomCardArray(20));
                set.Refresh();
                set.gameObject.transform.SetParent(card_root.transform);
                set.gameObject.transform.localPosition = pos_array[i];
                set.SetOrientation(orientation);
                set.UpdateConfig(new MJDeskCardConfig()
                {
                    arrange = new int[] { 5, 6, 7, 8 },
                });
            }
        }

        public void CaseMJHandCardSet()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");


            Vector3[] pos_array = new Vector3[(int)MJOrientation.Count]
            {
                new Vector3(-0f, 0, -0f),
                new Vector3(0f, 0, -0f),
                new Vector3(0f, 0, 0f),
                new Vector3(-0f, 0, 0f)
            };
            // 4个玩家方位的朝向
            for (int i = 0; i < (int)MJOrientation.Count; i++)
            //for (int i = 0; i < 1; i++)
            {
                var orientation = (MJOrientation)i;

                var hand = new MJHandSet();
                hand.Initialize(14, CameraUtil.GetMainCamera());
                hand.Reload(GetRandomCardArray(10));
                hand.Refresh();
                hand.UpdateConfig(new MJHandSetConfig()
                {
                    liftHeight = 0.02f,
                    interval = 0.02f,
                    anchor = HandCardAnchor.Right
                });
                hand.gameObject.transform.SetParent(card_root.transform);
                hand.gameObject.transform.localPosition = pos_array[i];
                hand.Sort();
                hand.SetOrientation(orientation);

                if (i == 0)
                {
                    hand.Append(3);

                    var cards = hand.GetCards();
                    hand.SelectCard(cards[0].gameObject);
                }

                if (i == 3)
                {
                    var cards = hand.GetCards();
                    hand.SelectCard(cards[3].gameObject);
                }

                hand.Refresh();
            }
        }

        public void CaseMJWallRoot()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            var wall_root = new MJWallRoot(new MJWallConfig()
            {
                centreDistance = 0.63f,
                stackCountPerSide = new List<int> { 17, 17, 17, 17 }
            });

            wall_root.InitializeWalls(wall_root.GetConfig(), 0, 1, 3);
            wall_root.TakeStack(2);
            wall_root.Take();
            wall_root.Take();

            wall_root.Take(true);
            wall_root.Take(true);

            wall_root.gameObject.transform.SetParent(card_root.transform);
        }

        public void CaseMJDeskCardRoot()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            var desk_root = new MJDeskCardRoot(new MJDeskCardConfig()
            {
                position = new DVector3[4]
                {
                    new DVector3(-0, 0, -0),
                    new DVector3(0, 0, -0),
                    new DVector3(0, 0, 0),
                    new DVector3(-0, 0, 0)
                },
                arrange = new int[] { 1, 3, 5, 7 }
            });

            var cards = GetRandomCardArray(60);

            for (int i = 0; i < cards.Length; i++)
            {
                desk_root.PushCard((MJOrientation)(i % 4), cards[i]);
            }

            desk_root.PopCard();
            desk_root.gameObject.transform.SetParent(card_root.transform);
        }

        public void CaseMJMeldRoot()
        {
            var card_root = GameObject.Find("MJCardRoot");
            if (card_root != null)
            {
                Object.Destroy(card_root);
            }
            card_root = new GameObject("MJCardRoot");

            var meld_root = new MJMeldRoot(new MJMeldConfig()
            {
                position = new DVector3[]
                {
                    new DVector3(0f, 0, -0.4f),
                    new DVector3(0.6f, 0, 0f),
                    new DVector3(0f, 0, 0.4f),
                    new DVector3(-0.6f, 0, 0f)
                },
                direction = MeldStackDirection.LeftToRight,
                interval = 0.03f
            });

            //for (int i = 0; i < 4; i++)
            //{
            //    meld_root.PushMeld((MJOrientation)i, new MJMeldPong(new int[] { 2, 2, 2 }, 1));
            //    meld_root.PushMeld((MJOrientation)i, new MJMeldChow(new int[] { 1, 2, 3 }));
            //    meld_root.PushMeld((MJOrientation)i, new MJMeldConcealedKong(new int[] { 0, 0, 0, 0 }));
            //}

            meld_root.gameObject.transform.SetParent(card_root.transform);
        }

        //public void CaseMJSpace()
        //{
        //    new MJSpace(new MJSpaceConfig()
        //    {
        //        wallConfig = new MJWallConfig()
        //        {
        //            stackCountPerSide = new int[4] { 17, 17, 17, 17 },
        //            centreDistance = 0.5f
        //        },
        //        deskConfig = new MJDeskCardConfig()
        //        {
        //            arrange = new int[1] { 6 },
        //            position = new DVector3[4]
        //            {
        //                new DVector3(-0f, 0, -0f),
        //                new DVector3(0f, 0, -0f),
        //                new DVector3(0f, 0, 0f),
        //                new DVector3(-0f, 0, 0f)
        //            }
        //        },
        //        meldConfig = new MJMeldConfig()
        //        {
        //            direction = MeldStackDirection.LeftToRight,
        //            interval = 0.03f,
        //            position = new DVector3[4]
        //            {
        //                new DVector3(-0f, 0, -0f),
        //                new DVector3(0f, 0, -0f),
        //                new DVector3(0f, 0, 0f),
        //                new DVector3(-0f, 0, 0f)
        //            }
        //        },
        //        handConfig = new MJHandSetConfig()
        //        {
        //            anchor = HandCardAnchor.Right,
        //            interval = 0.02f,
        //            liftHeight = 0.03f,
        //            position = new DVector3[4]
        //            {
        //                new DVector3(-0f, 0, -0f),
        //                new DVector3(0f, 0, -0f),
        //                new DVector3(0f, 0, 0f),
        //                new DVector3(-0f, 0, 0f)
        //            }
        //        }
        //    });
        //}

        //public void CaseMJSpace2()
        //{
        //    string json = "";
        //    //var ta = AssetsManager.Load<TextAsset>(MJDefine.MJ_ASSET_NAME, "MJ/Config/space_config.json");
        //    var config = LitJson.JsonMapper.ToObject<MJSpaceConfig>(json);

        //    new MJSpace(config);
        //}

        private int[] GetRandomCardArray(int count)
        {
            var cardvalues = new int[]
            {
                1,2,3,4,5,6,7,8,9,
                17,18,19,20,21,22,23,24,25,
                33,34,35,36,37,38,39,40,41,
                49,50,51,52,53,54,55
                //65,66,67,68,69,70,71,72
            };

            var cards = new int[count];
            for (int i = 0; i < count; i++)
            {
                cards[i] = cardvalues[Random.Range(0, cardvalues.Length)];
            }
            return cards;
        }
    }
}