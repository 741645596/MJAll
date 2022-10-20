// MJAnimationHelper.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/26

using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public static class MJAnimationHelper
    {
        public static void PlayDiceAnimation(MJDice dice, Action onFinished = null)
        {
            var diceList = dice.GetDiceList();


            for (int i = 0; i < diceList.Count; i++)
            {
                var d = diceList[i];
                Sequence mySequence1 = DOTween.Sequence();
                mySequence1.Append(d.transform.DOLocalRotate(new Vector3(1800, 1800, 1800), 0.6f, RotateMode.LocalAxisAdd))
                    .Append(d.transform.DOLocalRotate(new Vector3(360, 360, 360), 1f, RotateMode.LocalAxisAdd));
            }

            var transform = dice.gameObject.transform;

            Sequence mySequence = DOTween.Sequence();

            mySequence.Append(transform.DOLocalRotate(new Vector3(0, 3600f, 0), 0.6f, RotateMode.FastBeyond360))
                .Append(transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360));

            mySequence.AppendInterval(0.5f);

            mySequence.AppendCallback(() => onFinished?.Invoke());
        }

        public static Sequence PlayGangAnimation(MJMeld meld, Action callback = null)
        {
            var seq = DOTween.Sequence();

            var cards = meld.GetCards();
            var card = cards[meld.count - 1];
            card.gameObject.SetActive(true);
            card.HideShadow();
            var target_pos = card.gameObject.transform.localPosition;
            var start_pos = target_pos + new Vector3(0, 0.25f, 0);
            card.gameObject.transform.localPosition = start_pos;
            var drop = card.gameObject.transform.DOLocalMove(target_pos, 0.1f).SetLink(card.gameObject);
            drop.SetEase(Ease.InCirc);
            seq.Append(drop);

            WNode3D effect = null; // 光效
            seq.AppendCallback(() =>
            {
                effect = WNode3D.Create("MJCommon/MJ/mj_zm_effe_tongyong", "gang_l_01_01.prefab");
                effect.transform.SetParent(cards[1].gameObject.transform, false);
                effect.transform.localPosition = new Vector3(0, -MJDefine.MJCardSizeY*0.5f, 0);
            });

            var card0 = cards[0];
            var target_pos0 = card0.gameObject.transform.localPosition;
            var start_pos0 = target_pos0 + new Vector3(-0.002f, 0, 0);
            var move0_0 = card0.gameObject.transform.DOLocalMove(start_pos0, 0.05f).SetLink(card0.gameObject);
            var move0_1 = card0.gameObject.transform.DOLocalMove(target_pos0, 0.02f).SetLink(card0.gameObject);
            move0_0.SetDelay(0.1f);
            seq.Append(move0_0);

            var card2 = cards[2];
            var target_pos2 = card2.gameObject.transform.localPosition;
            var start_pos2 = target_pos2 + new Vector3(0.002f, 0, 0);
            card2.gameObject.transform.localPosition = start_pos2;
            var move2_0 = card2.gameObject.transform.DOLocalMove(start_pos2, 0.05f).SetLink(card2.gameObject);
            seq.Join(move2_0);
            var move2_1 = card2.gameObject.transform.DOLocalMove(target_pos2, 0.02f).SetLink(card2.gameObject);

            seq.Append(move0_1);
            seq.Join(move2_1);
            seq.AppendInterval(1f);
            seq.onComplete += () =>
            {
                effect?.RemoveFromParent();

                callback?.Invoke();
            };
            seq.SetLink(card.gameObject);
            seq.Play();
            return seq;
        }

        /// <summary>
        /// 添加副子动画
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="onStart"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static Sequence PlayAddMeldAnimation(MJMeldStack stack)
        {
            var lastMeld = stack.GetMeld(stack.count - 1);
            var targetPos = lastMeld.gameObject.transform.localPosition;
            var startPos = targetPos + new Vector3(0.3f, 0, 0);
            lastMeld.gameObject.transform.localPosition = startPos;

            var seq = DOTween.Sequence();
            if (lastMeld.showType == MJMeld.ShowType.Stacking)
            {
                lastMeld.GetCard(lastMeld.count - 1).gameObject.SetActive(false);
            }
            seq.Append(lastMeld.gameObject.transform.DOLocalMove(targetPos, 0.2f).SetLink(lastMeld.gameObject));
            if (lastMeld.showType == MJMeld.ShowType.Stacking)
            {
                seq.AppendCallback(() => PlayGangAnimation(lastMeld));
            }
            seq.SetTarget(lastMeld.gameObject);
            seq.SetLink(lastMeld.gameObject);
            seq.SetAutoKill(true);
            seq.Play();

            return seq;
        }

        /// <summary>
        /// 播放刷新手牌动画，从当前位置移动到Refresh后的位置
        /// </summary>
        /// <param name="handset"></param>
        /// <returns></returns>
        public static Sequence PlayRefreshHandsetAnimation(MJHandSet handset)
        {
            var seq = DOTween.Sequence();

            if (handset.count == 0)
            {
                return seq;
            }

            var start_pos_list = new List<Vector3>();
            handset.Traverse((card, index) => {
                start_pos_list.Add(card.gameObject.transform.localPosition);
            });

            handset.Sort();
            var target_pos_list = new List<Vector3>();
            handset.Traverse((card, index) => {
                target_pos_list.Add(card.gameObject.transform.localPosition);
                card.gameObject.transform.localPosition = start_pos_list[index];

                var move_time = 0.3f;
                var shake = 0.0001f; // 抖动位移距离
                // 原地不动的牌，被碰撞做一次抖动
                if (start_pos_list[index] == target_pos_list[index])
                {
                    var move = DOTween.Sequence();
                    var tween1 = card.gameObject.transform.DOLocalMove(target_pos_list[index] + new Vector3(shake * index, 0, 0), 0.08f).SetLink(card.gameObject);
                    var tween2 = card.gameObject.transform.DOLocalMove(target_pos_list[index], 0.08f).SetLink(card.gameObject);
                    move.Append(tween1);
                    move.Append(tween2);
                    move.SetDelay(move_time);
                    seq.Join(move);
                }
                // 其他牌移动到目标位置
                else
                {
                    var tween = card.gameObject.transform.DOLocalMove(target_pos_list[index], move_time).SetLink(card.gameObject);
                    seq.Join(tween);
                }
            });

            seq.SetLink(handset.GetCard(0).gameObject);

            return seq;
        }


        /// <summary>
        /// 播放提起牌动画。根据传入牌值列表，每个值找一张牌
        /// </summary>
        /// <param name="handset"></param>
        /// <param name="cardvalue"></param>
        /// <returns></returns>
        public static Sequence PlaySelectHandCardAnimation(MJHandSet handset, List<int> values, List<MJCard> outSelected, Action callback = null)
        {
            var cards = handset.GetCardsByValues(values);

            if (outSelected != null)
            {
                outSelected.Clear();
                outSelected.AddRange(cards);
            }

            var seq = PlaySelectHandCardAnimation(cards);

            seq.onComplete += () => callback?.Invoke();

            seq.Play();

            return seq;
        }


        /// <summary>
        /// 同上。随机选择
        /// </summary>
        /// <param name="handSet"></param>
        /// <param name="count"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Sequence PlaySelectRandomHandCardAnimation(MJHandSet handSet, int count, List<MJCard> outSelected, Action onFinished = null)
        {
            count = Mathf.Min(handSet.count, count);

            if (count == 0)
            {
                onFinished?.Invoke();
                return DOTween.Sequence();
            }

            var cards = handSet.GetCards().Clone();
            var selected = new List<MJCard>();
            while(selected.Count < count)
            {
                var index = UnityEngine.Random.Range(0, cards.Count);
                selected.Add(cards[index]);
                cards.RemoveAt(index);
            }

            if (outSelected != null)
            {
                outSelected.Clear();
                outSelected.AddRange(selected);
            }

            var seq = PlaySelectHandCardAnimation(selected);
            seq.onComplete += () => onFinished?.Invoke();
            seq.Play();
            return seq;
        }


        private static Sequence PlaySelectHandCardAnimation(List<MJCard> cards)
        {
            var seq = DOTween.Sequence();
            if (cards.Count == 0)
            {
                return seq;
            }
            cards.ForEach(c => seq.Join(c.gameObject.transform.DOLocalMoveY(0.02f, 0.4f).SetRelative(true).SetLink(c.gameObject)));
            seq.AppendInterval(0.2f);
            seq.SetTarget(cards[0].gameObject);
            seq.SetLink(cards[0].gameObject);
            return seq;
        }

        /// <summary>
        /// 换三张插入手牌
        /// </summary>
        /// <param name="handset"></param>
        /// <param name="insert_values"></param>
        /// <param name="callback"></param>
        public static void PlayInsert3Cards(MJHandSet handset, List<int> insert_values, Action callback = null)
        {
            var start_pos_array = new List<Vector3>();
            var target_pos_array = new List<Vector3>();
            // 保存初始位置
            for (int i = 0; i < handset.count; i++)
            {
                var card = handset.GetCard(i);
                start_pos_array.Add(card.gameObject.transform.localPosition);
            }

            // 添加换三张拿到3张牌
            var insert_cards = new List<MJCard>();
            for (int i = 0; i < insert_values.Count; i++)
            {
                var card = handset.Append(insert_values[i]);
                insert_cards.Add(card);
            }
            // 排序后，保存目标位置，并还原回初始位置
            handset.Sort();
            var index = 0;
            for (int i = 0; i < handset.count; i++)
            {
                var card = handset.GetCard(i);
                target_pos_array.Add(card.gameObject.transform.localPosition);

                var skip = false;
                for (int j = 0; j < insert_cards.Count; j++)
                {
                    var insert_card = handset.FindCard(insert_cards[j].gameObject);
                    if (insert_card == card)
                    {
                        skip = true;
                        index++;
                        break;
                    }
                }

                if (skip)
                {
                    card.gameObject.transform.localPosition = target_pos_array[i];
                    card.gameObject.SetActive(false);
                }
                else
                {
                    card.gameObject.transform.localPosition = start_pos_array[i - index];
                }
            }
            // 播放插牌动画
            for (int i = 0; i < insert_cards.Count; i++)
            {
                var insert_card = handset.FindCard(insert_cards[i].gameObject);
                var tween = insert_card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "card_insert.asset");
                tween.SetDelay(i * 0.1f);

                if(i == insert_cards.Count - 1)
                {
                    tween.onComplete += () =>
                    {
                        callback?.Invoke();
                    };
                }
            }
            // 播放手牌位移动画
            for (int i = 0; i < handset.count; i++)
            {
                var card = handset.GetCard(i);

                var skip = false;
                for (int j = 0; j < insert_cards.Count; j++)
                {
                    var insert_card = handset.FindCard(insert_cards[j].gameObject);
                    if (insert_card == card)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                {
                    continue;
                }
                card.gameObject.transform.DOLocalMove(target_pos_array[i], 0.5f).SetLink(card.gameObject);
            }
        }

        public static void PlayHandCardDealAnimation(MJHandSet handSet, Action onFinished = null)
        {
            var cards = handSet.GetCards();
            var step = 4;
            var delay = 0.06f;

            for (int i = 0; i < cards.Count; i += step)
            {
                for (int j = i; j < Mathf.Min(i + step, cards.Count); j++)
                {
                    var tran = cards[j].gameObject.transform;
                    var seq = tran.RunTweenGraph("MJCommon/MJ/mj_tween", "handset_deal_stand.asset");
                    seq.SetDelay(i * delay);
                }
            }
        }

        public static void PlayShowHandCardAnimation(MJHandSet handSet, Action onFinished = null)
        {
            var cards = handSet.GetCards();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var time = i;
                card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "handset_over_pull.asset", (t) =>
                {
                    var step_time = 0.05f;
                    var delay1 = Mathf.CeilToInt(time * 0.5f) * step_time;
                    var delay2 = Mathf.CeilToInt((cards.Count - time) * 0.5f) * step_time;
                    var delay = Mathf.Min(delay1, delay2);

                    var s = t.RunTweenGraph("MJCommon/MJ/mj_tween", "handset_over_down.asset");
                    s.SetDelay(delay);
                    if (time == cards.Count - 1)
                    {
                        s.onComplete += () =>
                        {
                            onFinished?.Invoke();
                        };
                    }
                    s.Play();
                });
            }

        }

        /// <summary>
        /// 出牌动画（两段过程）
        /// </summary>
        /// <param name="card"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Sequence PlayOutCardAnimation(MJCard card, Vector3 end)
        {
            var offset = new Vector3(-0.02f, 0f, 0.03f); // 同Graph中Move Node内的值对应
            end -= offset;
            var duration = 0.15f;                       // 同Graph中Rotate Node内的值对应

            var s = card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "outcard.asset");
            var seq = DOTween.Sequence();
            var move = card.gameObject.transform.DOLocalMove(end, duration).SetLink(card.gameObject);
            seq.Join(s);
            seq.Join(move);
            seq.SetLink(card.gameObject);
            seq.Play();
            return seq;
        }

        /// <summary>
        /// 出牌动画，直接飞到目标位置
        /// </summary>
        /// <param name="card"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Sequence PlayDirectOutCardAnimation(MJCard card, Vector3 end)
        {
            var duration = 0.15f;
            var direction = (end - card.gameObject.transform.localPosition).normalized;
            var rotation = Quaternion.LookRotation(direction);
            card.gameObject.transform.localRotation = rotation;
            var seq = DOTween.Sequence();
            var move = card.gameObject.transform.DOLocalMove(end, duration).SetLink(card.gameObject);
            seq.Join(move);
            seq.Join(card.gameObject.transform.DOLocalRotate(Vector3.zero, duration, RotateMode.Fast).SetLink(card.gameObject));
            seq.SetLink(card.gameObject);
            seq.Play();
            return seq;
        }

        public static void PlayLiftAnimation(MJSpace space)
        {
            if(space == null)
            {
                return;
            }

            space.table.lifter.RunTweenGraph("MJCommon/MJ/mj_tween", "table_lifter.asset");

            var walls = space.wallRoot.GetWallArray();
            for (int i = 0; i < walls.Length; i++)
            {
                var wall = walls[i];
                var events = new Dictionary<string, Action>();
                events.Add("Begin", () => wall.gameObject.SetActive(true));
                wall.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", $"wall_lifter_{i}.asset", events);
            }
        }

        public static void PlayDealAnimation2(MJSpace space, MJOrientation banker, Action<int> onTakeCard = null, Action<string> onPlaySound = null, Action onFinished = null)
        {
            if (space == null)
            {
                return;
            }

            var handCardSets = space.handRoot.GetMJHandSets();
            var mjWall = space.wallRoot;

            int index = (int)banker;
            var stepCount = 4;
            var stepTime = 0.5f;

            var seqList = new Sequence[4];
            for (int i = 0; i < 4; i++)
            {
                var seq = DOTween.Sequence();
                seq.SetDelay(i * 0.25f);
                seqList[i] = seq;
            }

            //隐藏所有手牌
            HideHandSet(handCardSets);

            while (true)
            {
                var handset = handCardSets[(MJOrientation)(index % 4)];
                var cards = handset.GetCards();

                var count = Mathf.Ceil(cards.Count / (float)stepCount);
                var j = Mathf.FloorToInt((index - (int)banker) / 4);
                var seq = seqList[index % 4];
                // 循环终止条件
                if (j >= count)
                {
                    break;
                }

                // 发stepCount张牌，同时同牌墙移除stepCount张牌
                for (int k = j * stepCount; k < j * stepCount + stepCount; k++)
                {
                    if (k >= cards.Count)
                    {
                        break;
                    }
                    var card = cards[k];
                    seq.AppendCallback(() =>
                    {
                        card.SetActive(true);
                        mjWall.Take();
                        onTakeCard?.Invoke(1);
                    });
                }
                if ((MJOrientation)(index % 4) == MJOrientation.Down)
                {
                    seq.AppendCallback(() => onPlaySound?.Invoke("Deal"));
                }
                // 播放动画
                for (int k = j * stepCount; k < j * stepCount + stepCount; k++)
                {
                    if (k >= cards.Count)
                    {
                        break;
                    }
                    var card = cards[k];
                    var transform = card.gameObject.transform;
                    // 自己的动画
                    if ((MJOrientation)(index % 4) == MJOrientation.Down)
                    {
                        card.HideShadow();
                        var t = transform.DOLocalRotate(new Vector3(0, 0, 0), stepTime);
                        transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        seq.Join(t);
                    }
                    // 其他玩家的动画
                    else
                    {
                        var origin_pos = transform.localPosition;
                        float y = MJDefine.MJCardSizeZ * 0.5f;
                        var end_pos = new Vector3(origin_pos.x, y, origin_pos.z);
                        transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        transform.localPosition = new Vector3(origin_pos.x, MJDefine.MJCardSizeY * 0.5f, -MJDefine.MJCardSizeZ * 0.5f);

                        var s = DOTween.Sequence();
                        var t = transform.DOLocalRotate(new Vector3(90, 0, 0), stepTime, RotateMode.LocalAxisAdd);
                        s.Append(t);
                        var t2 = transform.DOLocalMove(end_pos, stepTime).SetLink(transform.gameObject);
                        s.Join(t2);

                        seq.Join(s);
                    }
                }

                index++;
            }

            seqList[0].AppendCallback(() =>
            {
                onFinished?.Invoke();
                PlaySortHandCardAnimation(handCardSets[0], onFinished);
            });
            seqList[0].Play();
        }

        public static void PlaySortHandCardAnimation(MJHandSet handSet, Action onSortCard = null)
        {
            var cards = handSet.GetCards();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var time = i;
                card.body.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "handset_sort_down.asset", (t)=>
                {
                    if(time == cards.Count - 1)
                    {
                        onSortCard?.Invoke();
                        handSet.Traverse((c, idx) =>
                        {
                            c.body.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "handset_sort_stand.asset").SetDelay(idx / 60f);
                        });
                    }
                });
            }
        }

        public static void PlayDealAnimation(MJSpace space, MJOrientation banker, Action<int> onTakeCard = null, Action<string> onPlaySound = null, Action onFinished = null)
        {
            if (space == null)
            {
                return;
            }

            var handCardSets = space.handRoot.GetMJHandSets();
            var mjWall = space.wallRoot;

            int index = (int)banker;
            var stepCount = 4;
            var stepTime = 0.5f;

            var seqList = new Sequence[4];
            seqList[0] = DOTween.Sequence();
            seqList[1] = DOTween.Sequence();
            seqList[2] = DOTween.Sequence();
            seqList[3] = DOTween.Sequence();

            seqList[0].SetDelay(0);
            seqList[1].SetDelay(stepTime * 0.25f);
            seqList[2].SetDelay(stepTime * 0.5f);
            seqList[3].SetDelay(stepTime * 0.75f);
            //隐藏所有手牌
            HideHandSet(handCardSets);

            while (true)
            {
                var handset = handCardSets[(MJOrientation)(index % 4)];
                var cards = handset.GetCards();

                var count = Mathf.Ceil(cards.Count / (float)stepCount);
                var j = Mathf.FloorToInt((index - (int)banker) / 4);
                var seq = seqList[index % 4];
                // 循环终止条件
                if (j >= count)
                {
                    break;
                }

                // 发stepCount张牌，同时同牌墙移除stepCount张牌
                for (int k = j * stepCount; k < j * stepCount + stepCount; k++)
                {
                    if (k >= cards.Count)
                    {
                        break;
                    }
                    var card = cards[k];
                    seq.AppendCallback(() =>
                    {
                        card.SetActive(true);
                        mjWall.Take();
                        onTakeCard?.Invoke(1);
                    });
                }
                if ((MJOrientation)(index % 4) == MJOrientation.Down)
                {
                    seq.AppendCallback(() => onPlaySound?.Invoke("Deal"));
                }
                // 播放动画
                for (int k = j * stepCount; k < j * stepCount + stepCount; k++)
                {
                    if (k >= cards.Count)
                    {
                        break;
                    }
                    var card = cards[k];
                    var transform = card.gameObject.transform;
                    // 自己的动画
                    if ((MJOrientation)(index % 4) == MJOrientation.Down)
                    {
                        card.HideShadow();
                        var t = transform.DOLocalRotate(new Vector3(0, 0, 0), stepTime);
                        transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        seq.Join(t);
                    }
                    // 其他玩家的动画
                    else
                    {
                        var origin_pos = transform.localPosition;
                        float y = MJDefine.MJCardSizeZ * 0.5f;
                        var end_pos = new Vector3(origin_pos.x, y, origin_pos.z);
                        transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        transform.localPosition = new Vector3(origin_pos.x, MJDefine.MJCardSizeY * 0.5f, -MJDefine.MJCardSizeZ * 0.5f);

                        var s = DOTween.Sequence();
                        var t = transform.DOLocalRotate(new Vector3(90, 0, 0), stepTime, RotateMode.LocalAxisAdd);
                        s.Append(t);
                        var t2 = transform.DOLocalMove(end_pos, stepTime).SetLink(transform.gameObject);
                        s.Join(t2);

                        seq.Join(s);
                    }
                }

                index++;
            }

            // 自己的牌扣下排序后再抬起
            var downhandset = handCardSets[MJOrientation.Down];
            var downcards = downhandset.GetCards();
            for (int i = 0; i < downcards.Count; i++)
            {
                var s = DOTween.Sequence();
                var card = downcards[i];
                var t1 = card.gameObject.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.1f, RotateMode.LocalAxisAdd);
                var t2 = card.gameObject.transform.DOLocalRotate(new Vector3(90, 0, 0), 0.1f, RotateMode.LocalAxisAdd);
                t2.SetDelay(0.2f);

                s.Append(t1);
                s.AppendCallback(() =>
                {
                    downhandset.Sort();
                    if (i == downcards.Count - 1)
                    {
                        onPlaySound?.Invoke("Sort");
                    }
                });
                s.Append(t2);
                if (i == 0)
                {
                    seqList[0].Append(s);
                }
                else
                {
                    seqList[0].Join(s);
                }
            }
            seqList[0].AppendCallback(() => onFinished?.Invoke());
            seqList[0].Play();
        }

        /// <summary>
        /// 播放理牌动画
        /// </summary>
        /// <param name="handCardSet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Sequence PlayInsertCardAnimation(MJHandSet handCardSet, int index = -1)
        {
            var cards = handCardSet.GetCards();
            var count = cards.Count;
            var seq = DOTween.Sequence();
            if (count == 0)
            {
                return seq;
            }

            index = index == -1 ? count - 1 : index;
            var card = cards[index];
            handCardSet.Sort(null, false);
            if (cards.IndexOf(card) == index)
            {
                return PlayRefreshHandsetAnimation(handCardSet);
            }

            var trs = card.gameObject.transform;
            seq.Append(trs.DOLocalMove(new Vector3(0, MJDefine.MJCardSizeZ*1.3f, 0), 0.1f).SetRelative(true).SetLink(trs.gameObject));

            var targetIndex = cards.IndexOf(card);
            var targetState = handCardSet.GetCardState(targetIndex);
            var biasIndex = handCardSet.GetBiasStartIndex();
            var targetPos = handCardSet.ComputePosition(targetIndex, targetState, biasIndex);
            seq.Append(trs.DOLocalMove(targetPos + new Vector3(0, MJDefine.MJCardSizeZ*1.3f, 0), 0.25f).SetLink(trs.gameObject));
            seq.Join(trs.DOLocalRotate(new Vector3(0, 0, -45), 0.25f).SetRelative(true));
            seq.AppendCallback(() =>
            {
                handCardSet.Traverse((c, i) =>
                {
                    if (c == card)
                    {
                        return;
                    }

                    var state = handCardSet.GetCardState(i);
                    var pos = handCardSet.ComputePosition(i, state, biasIndex);
                    seq.Join(c.gameObject.transform.DOLocalMove(pos, 0.3f).SetLink(c.gameObject));
                });
            });
            seq.Append(trs.DOLocalMove(targetPos, 0.35f).SetLink(trs.gameObject));
            seq.Join(trs.DOLocalRotate(new Vector3(0, 0, 45), 0.4f).SetRelative(true));

            seq.SetEase(Ease.Linear);

            seq.SetLink(card.gameObject);

            return seq;
        }

        /// <summary>
        /// 播放抓牌动画
        /// </summary>
        /// <param name="handSet"></param>
        public static Sequence PlaySendCardAnimation(MJHandSet handSet)
        {
            var cards = handSet.GetCards();
            var count = cards.Count;
            var card = cards[count - 1];

            var transform = card.gameObject.transform;
            var origin_pos = transform.localPosition;
            //var start_pos = new Vector3(origin_pos.x, (float)(origin_pos.y + 1.5f * MJDefine.MJCardSizeZ), origin_pos.z);
            var start_pos = new Vector3(origin_pos.x - MJDefine.MJCardSizeX, (float)(origin_pos.y + MJDefine.MJCardSizeZ), origin_pos.z);

            transform.localPosition = start_pos;
            //transform.localRotation = Quaternion.Euler(40, 30, 40);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            var seq = DOTween.Sequence();
            var time = 0.2f;
            var rt = transform.DOLocalRotate(new Vector3(0, 0, 0), time, RotateMode.Fast).SetLink(transform.gameObject);
            var mt = transform.DOLocalMove(new Vector3(origin_pos.x, (float)(origin_pos.y + 0.5 * MJDefine.MJCardSizeZ), origin_pos.z), time).SetLink(transform.gameObject);

            seq.Append(rt).Join(mt);

            var mt2 = transform.DOLocalMove(origin_pos, 0.1f);
            seq.Append(mt2);
            seq.SetLink(card.gameObject);
            return seq;
        }

        private static void HideHandSet(Dictionary<MJOrientation, MJHandSet> handCardSets)
        {
            foreach (var item in handCardSets)
            {
                var handset = item.Value;
                var cards = handset.GetCards();
                for (int i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.SetActive(false);
                }
            }
        }

        public static Tween RotateY(Transform transform, int angle, float duration, Action callback = null)
        {
            var tween = transform.DOLocalRotate(new Vector3(0, angle, 0), 1).SetLink(transform.gameObject);
            tween.onComplete += () =>
            {
                callback?.Invoke();
            };
            return tween;
        }
    }
}
