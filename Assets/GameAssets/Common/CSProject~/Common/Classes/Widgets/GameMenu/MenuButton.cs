// @Author: tanjinhua
// @Date: 2021/4/21  9:53


using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using UnityEngine.Events;
using Unity.Core;

namespace Common
{
    public class MenuButton : WNode
    {

        public struct Config
        {
            public string assetName;
            public string imagePath;
            public string name;
            public Action<string> onClick;
        }


        public Action onClick;


        public MenuButton(Config config)
        {
            InitGameObject("Common/Game/game_ui_prefabs", "menu_btn.prefab");

            gameObject.name = config.name;

            Image image = gameObject.GetComponent<Image>();

            image.sprite = AssetsManager.Load<Sprite>(config.assetName, config.imagePath);

            image.SetNativeSize();

            Button button = gameObject.GetComponent<Button>();

            button.onClick.AddListener(new UnityAction(() =>
            {
                config.onClick?.Invoke(config.name);

                onClick?.Invoke();
            }));
        }
    }
}
