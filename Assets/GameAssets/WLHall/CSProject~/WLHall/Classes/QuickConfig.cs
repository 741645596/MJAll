
// @Date: 2021/8/16  15:08

using System.Collections.Generic;
using WLCore;

namespace WLHall
{
    /// <summary>
    /// 临时配置
    /// </summary>
    public class QuickConfig
    {
        public static string loginIp = "192.168.67.228";
        public static int loginPort = 6532;                 // socket port
        public static int loginPortWebSocket = 8181;        // websocket port

        public static string account = "testuser18563";

        public static string password = "123654";

        public static Dictionary<string, uint> testRoomIds = new Dictionary<string, uint>
        {
            {"普通场", 6484 }, {"精英场", 6485}, {"土豪场", 6486}, {"至尊场", 6487}
        };

        public static bool autoJoinRoom = false;
        public static uint autoJoinRoomId = 6484;
        public static eSocketType socketType = eSocketType.socket;



        // 朋友场
        public static string shortName = "xlhz";
        public static Dictionary<string, uint> friendRoomIds = new Dictionary<string, uint>
        {
            {"2人", 6505 }, {"3人", 6504}, {"4人", 6503}
        };
        // 规则数组
        public static List<byte> rules = new List<byte> {11, 16, 3, 4, 1, 4, 1, 1, 1, 1, 1, 1, 0, 4, 0, 0, 0, 0};
    }
}
