using System;
using System.Collections.Generic;
using Unity.Core;
using UnityEngine;

namespace WLHall
{
    public class FunctionEx
    {

        /// <summary>
        /// 加载游戏DLL
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="onFinished"></param>
        public static void LoadGameAssemblies(string shortName, Action<bool> onFinished)
        {
            GameInfo gameInfo = GameInfoKit.GetGameByShortName(shortName);

            LoadGameAssemblies(gameInfo, onFinished);
        }

        /// <summary>
        /// 加载游戏DLL
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <param name="onFinished"></param>
        public static void LoadGameAssemblies(GameInfo gameInfo, Action<bool> onFinished)
        {
            List<string> moduleNames = GetGameDependencyModules(gameInfo);

            LoadDll.Load(moduleNames, onFinished, true);
        }

        /// <summary>
        /// 获取游戏依赖模块列表
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <returns></returns>
        public static List<string> GetGameDependencyModules(GameInfo gameInfo)
        {
            List<string> moduleNames = new List<string>() { Constants.VER_MODULE_GAME_COMMON };

            if ((Constants.GAME_GROUP_MAHJONG & gameInfo.type) > 0)
            {
                moduleNames.Add(Constants.VER_MODULE_MJ_COMMON);
            }
            else if ((Constants.GAME_GROUP_POKER & gameInfo.type) > 0)
            {
                moduleNames.Add(Constants.VER_MODULE_PK_COMMON);
            }
            if (gameInfo.cmd != null && gameInfo.cmd.ContainsKey("dp"))
            {
                moduleNames.Add($"area_{gameInfo.cmd["dp"]}");
            }
            moduleNames.Add(gameInfo.shortName);

            return moduleNames;
        }

        /// <summary>
        /// 获取当前大厅版本的字符串，格式为：1.1.1
        /// </summary>
        /// <returns></returns>
        public static string GetHallVersion()
        {
            //return CfgPackage.HALL_APP_VERSION;
            return null;
        }

        /// <summary>
        /// 获取完整版本号(包括通用组件和大厅组件版本号)，格式为：1.1.1.20.50
        /// 第4位为通用组件版本号，第5位为大厅组件版本号
        /// </summary>
        /// <returns></returns>
        public static string GetFullVersion()
        {
            string common_ver = "20";
            string hall_ver = "50";
            return string.Format("{0}.{1}.{2}", GetHallVersion(), common_ver, hall_ver);
        }

        /// <summary>
        /// 生成唯一的字符串
        /// </summary>
        /// <returns></returns>
        public static string GenerateStringID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {

                i *= ((int)b + 1);

            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>
        /// 转换货币单位
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ExchangeCurrencyUnit(long count)
        {
            var str = "" + count;
            if (count >= 10000 && count <= 99999999)
            {
                // 千位数向下取整
                str = Math.Round((Mathf.Floor((count / 1000.0f)) / 10), 1).ToString("0.0") + "万";
            }
            else if (count >= 100000000)
            {
                str = Math.Round(count / 100000000.0f, 1).ToString("0.0") + "亿";
            }
            return str;
        }

        public static int GetRoomMode(uint roomType)
        {
            return (int)roomType & Constants.ROOM_TYPE_SIT_MODE_MASK;

        }
    }
}