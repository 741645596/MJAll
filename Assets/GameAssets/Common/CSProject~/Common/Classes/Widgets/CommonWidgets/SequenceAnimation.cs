// @Author: tanjinhua
// @Date: 2021/5/13  13:17


using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;

namespace Common
{
    public class SequenceAnimation : WNode
    {

        public float spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;

                LayoutSprites();
            }
        }


        public float animationDuration = 0.8f;


        private float _spacing = 5f;
        private List<WSprite> _sprites;

        public SequenceAnimation(string assetName, List<string> images)
        {
            var obj = new GameObject("seuqence_animation", typeof(RectTransform));

            InitGameObject(obj);

            _sprites = new List<WSprite>();

            images.ForEach(image => 
            {
                WSprite sprite = WSprite.Create(assetName, image);

                sprite.AddTo(gameObject);

                sprite.rectTransform.anchorMin = Vector2.zero;

                sprite.rectTransform.anchorMax = Vector2.zero;

                _sprites.Add(sprite);
            });

            LayoutSprites();
        }


        public void PlayJump(int startIndex = 0, bool loop = true, float delay = 0.3f, float jumpHeight = 8f)
        {
            LayoutSprites();

            for (int i = startIndex; i < _sprites.Count; i++)
            {
                WSprite sprite = _sprites[i];

                Vector2 pos = sprite.GetPosition();

                Vector2 hightPos = pos + new Vector2(0, jumpHeight);

                Vector2 lowPos = pos - new Vector2(0, 2);

                CCAction action = CCAction.NewSequence()
                    .DelayTime((i - startIndex) * delay)
                    .MoveTo(0.3f, hightPos)
                    .MoveTo(0.4f, lowPos)
                    .MoveTo(0.1f, pos);

                if (loop && i == _sprites.Count - 1)
                {
                    action.CallFunc(() => PlayJump(startIndex, loop, delay, jumpHeight));
                }

                sprite.RunAction(action.ToAction());
            }
        }


        protected virtual void LayoutSprites()
        {
            float w = 0, h = 0;

            for (int i = 0; i < _sprites.Count; i++)
            {
                WSprite sprite = _sprites[i];

                sprite.StopAllActions();

                sprite.SetAnchor(cc.p(0, 0));

                sprite.SetPosition(new Vector2(w, 0));

                var size = sprite.GetContentSize();

                w += size.x;

                h = size.y > h ? size.y : h;

                if (i != _sprites.Count - 1)
                {
                    w += _spacing;
                }
            }

            SetContentSize(new Vector2(w, h));
        }
    }
}
