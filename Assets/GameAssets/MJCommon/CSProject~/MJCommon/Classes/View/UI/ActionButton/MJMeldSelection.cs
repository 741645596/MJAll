// @Author: tanjinhua
// @Date: 2021/4/14  9:37


using System;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJMeldSelection : WNode
    {

        public Action<MJActionData> onClick;

        private MJActionData _actionData;

        public MJMeldSelection(MJActionData data)
        {
            _actionData = data;

            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "meld_selection.prefab");

            gameObject.GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(_actionData));

            CreateCards();

            rectTransform.localScale = new Vector2(0.8f, 0.8f);
        }

        private void CreateCards()
        {
            for (int i = 0; i < _actionData.cardValues.Length; i++)
            {
                int value = _actionData.cardValues[i];

                var card = new MJStandCard2D(value);
                card.AddTo(gameObject);
            }

            float width = _actionData.cardValues.Length * MJStandCard2D.DimensionX + 20;
            float height = MJStandCard2D.DimensionY + 20;
            SetContentSize(width, height);
        }
    }
}
