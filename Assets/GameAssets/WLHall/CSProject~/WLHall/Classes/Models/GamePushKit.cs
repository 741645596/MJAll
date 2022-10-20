// @Author: xuzhihu
// @Date: 2021/5/7 20:03:33 
// @Description: 
using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Reflection;
using Unity.Core;

namespace WLHall
{
    public static class GamePushKit
    {
        private static Dictionary<string, CityData> _coinCfgGame = new Dictionary<string, CityData>();
        private static Dictionary<string, CityData> _friendCfgGame = new Dictionary<string, CityData>();
        private static JsonData _allOriginData;
        private static string _regionTest = "0"; // todo 默认先使用全国数据

        /// <summary>
        /// 获取金币场数据
        /// </summary>
        /// <returns></returns>
        public static CityData GetCfgGame()
        {
            if (!_coinCfgGame.ContainsKey(_regionTest)) {
                HandleData();
            }
            WLDebug.Info("GetCfgGame: " + JsonMapper.ToJson(_coinCfgGame[_regionTest]));
            return _coinCfgGame[_regionTest];
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        private static void HandleData()
        {
            _coinCfgGame[_regionTest] = GetDataByRegion(_regionTest);
            _friendCfgGame[_regionTest] = GetDataByRegion(_regionTest, true);
        }

        /// <summary>
        /// 通过地区码获取数据
        /// </summary>
        /// <param name="region"></param>
        /// <param name="bFriend"></param>
        /// <returns></returns>
        private static CityData GetDataByRegion(string region, bool bFriend = false)
        {
            JsonData originData = GetOriginData(region, bFriend);
            CityData cityData =  UnfoldData(originData);
            return cityData;
        }

        /// <summary>
        /// 通过地区码获取原始数据 （优先：市->省份->全国）
        /// </summary>
        /// <param name="region"></param>
        /// <param name="bFriend"></param>
        /// <returns></returns>
        private static JsonData GetOriginData(string region, bool bFriend = false)
        {
            string type = bFriend ? "friend" : "gold"; 
            TextAsset asset = AssetsManager.Load<TextAsset>("WLHall/Main/hall_push", "10001_821_normal.json");
            _allOriginData = JsonMapper.ToObject(asset.text);

            // 地级
            region = region.PadRight(6, '0');
            if (_allOriginData.Keys.Contains(region) && _allOriginData[region].Keys.Contains(type))
            {
                return _allOriginData[region][type];
            }

            // 找市
            region = region.Substring(0,4).PadRight(6, '0');
            if (_allOriginData.Keys.Contains(region) && _allOriginData[region].Keys.Contains(type))
            {
                return _allOriginData[region][type];
            }

            // 找省
            region = region.Substring(0, 2).PadRight(6, '0');
            if (_allOriginData.Keys.Contains(region) && _allOriginData[region].Keys.Contains(type))
            {
                return _allOriginData[region][type];
            }

            // 找全国
            return _allOriginData["0"][type];
        }

        private static CityData UnfoldData(JsonData originData)
        {
            CityData cityData = new CityData();
            Type type = typeof(CityData);
            foreach (string key in originData.Keys)
            {
                PropertyInfo property = type.GetProperty(key);
                if (property != null)
                {
                    property.SetValue(cityData, UnfoldList(originData[key]));
                }
            }
            return cityData;
        }

        private static List<GamePushData> UnfoldList(JsonData tagData)
        {
            List<GamePushData> gameDataList = new List<GamePushData>();
            for (int i = 0; i < tagData.Count; i++)
            {
                gameDataList.Add(UnfoldSingleData((int)tagData[i]));
            }
            return gameDataList;

        }

        /// <summary>
        /// 通过索引获取实际的数据
        /// </summary>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        private static GamePushData UnfoldSingleData(int dbIndex)
        {
            GamePushData gameData = JsonMapper.ToObject<GamePushData>(_allOriginData["db"][dbIndex].ToJson());
            if (gameData.games != null) 
            {
                gameData.gamesData = UnfoldList(JsonMapper.ToObject(JsonMapper.ToJson(gameData.games)));
            }
            return gameData;
        }

        /// <summary>
        /// 是否是斗地主比赛
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public static bool IsMatch(GamePushData gameData)
        {
            if (gameData == null)
            {
                return false;
            }
            string[] _list = { "比赛", "赢礼券", "福利赛" };
            if (gameData.category == 4 && _list.Contains(gameData.name) && gameData.id == 1) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是合集
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public static bool IsCombination(GamePushData gameData)
        {
            if (gameData == null)
            {
                return false;
            }
            if (gameData.category == 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是游戏
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public static bool IsGame(GamePushData gameData)
        {
            if (gameData == null)
            {
                return false;
            }
            if (gameData.category == 1 && gameData.type == 1)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 每个地区的数据
    /// </summary>
    public class CityData
    {
        public List<GamePushData> main { get; set; }
        public List<GamePushData> left { get; set; }
        public List<GamePushData> city { get; set; }
        public List<GamePushData> cityLeft { get; set; }
        public List<GamePushData> more  { get; set; }
    }

    public enum CityDataType
    {
        Main,
        Left,
        City,
        CityLeft,
        More
    }

    /// <summary>
    /// 每个游戏的数据
    /// </summary>
    public class GamePushData
    {
        public int category;            // 大类别
        public int type;                // 小类别
        public int show;                // 显示局数
        public string name;             // 名字
        public string icon;             // 图片路径
        public string shortname;        // 游戏短名
        public int id;                  // 游戏id
        public List<int> games;         // 游戏列表（合集的时候才有值）
        public List<GamePushData> gamesData; // 游戏列表
    }
}
