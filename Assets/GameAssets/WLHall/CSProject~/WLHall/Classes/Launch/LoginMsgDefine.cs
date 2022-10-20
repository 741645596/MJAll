// @Author: tanjinhua
// @Date: 2021/3/18  14:17


namespace WLHall
{
    public static class LoginMsgDefine
    {
        public const ushort MSG_LOGIN_CHECK_VERSION_REPLY = 1;
        public const ushort MSG_LOGIN_BY_NAME_REPLY = 3;
        public const ushort MSG_LOGIN_BY_NAME_FAILED = 4;

        public const ushort MSG_LOGIN_BY_UNNAME = 9;
        public const ushort MSG_LOGIN_BY_UNNAME_FAILED = 10;
        public const ushort MSG_LOGIN_BY_UNNAME_REPLY = 11;
        public const ushort MSG_LOGIN_ALLOC_ROLE_REPLY = 15;   //分配用户应答
        public const ushort MSG_LOGIN_SYSTEM_MESSAGE = 16;
        public const ushort MSG_LOGIN_BY_NAME2 = 17;
        public const ushort MSG_LOGIN_ALLOC_ROLE2 = 18;     //分配用户名（带用户平台来源)
        public const ushort MSG_LOGIN_CHECK_VERSION = 19;
        public const ushort MSG_LOGIN_WEB_GET = 21;
        public const ushort MSG_LOGIN_WEB_POST = 22;
        public const ushort MSG_LOGIN_WEB_REPLY = 23;
    }
}
