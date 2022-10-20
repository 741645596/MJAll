// @Author: tanjinhua
// @Date: 2021/8/12  20:33

using System;
using System.Collections.Generic;
using System.Text;
using Unity.Utility;
using WLCore;

namespace WLHall
{
    /// <summary>
    /// 登陆服务器消息管理器
    /// </summary>
    public class LoginManager : MessageManager
    {
        public enum LoginType
        {
            Account,        // 账号登陆
            Anonymous,      // 匿名登陆
            GetUserList,    // 拉取用户列表
            AllocUser       // 分配新账号
        }

        public struct LoginData
        {
            public LoginType type;
            public string account;
            public string password;
        }

        /// <summary>
        /// 登陆成功事件
        /// </summary>
        public Action<int, string, uint, string, int> onLogin;


        //private ClientSocket _cs;
        private LoginData _loginData;

        internal LoginManager()
        {
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_CHECK_VERSION_REPLY, OnMsgCheckVersionReply);
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_BY_NAME_REPLY, OnMsgLoginReply);
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_BY_NAME_FAILED, OnMsgLoginFailed);
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_SYSTEM_MESSAGE, OnMsgSystemMessage);
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_WEB_REPLY, OnMsgWebReply);
            RegisterMessageProcessor(LoginMsgDefine.MSG_LOGIN_ALLOC_ROLE_REPLY, OnMsgAllocUserReply);
        }

        /// <summary>
        /// 使用账号密码登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void LoginByAccount(string account, string password)
        {
            _loginData = new LoginData
            {
                type = LoginType.Account,
                account = account,
                password = password
            };

            StartConnect();
        }

        #region Socket Events
        public override void OnSocketOpen()
        {
            base.OnSocketOpen();

            SendLoginCheckVersion();
        }
        #endregion


        #region Receiving
        private void OnMsgCheckVersionReply(MsgHeader msg)
        {
            int result = msg.ReadByte();
            bool successed = result == Constants.LCVR_OK || result == Constants.LCVR_WARNING_NEW_VERSION;
            if (!successed)
            {
                WLDebug.LogWarning("登陆版本检查失败, result:", result);
                return;
            }

            byte[] keyBuffer = new byte[Constants.MD5_PSW_LEN];
            if (result != Constants.LCVR_ERROR_NOT_SUPPORT)
                msg.stream.Read(keyBuffer, 0, Constants.MD5_PSW_LEN);

            switch (_loginData.type)
            {
                case LoginType.Account:
                    SendLoginByAccount(_loginData.account, _loginData.password, keyBuffer);
                    break;
                case LoginType.Anonymous:
                    // TODO: 匿名登陆
                    break;
                case LoginType.AllocUser:
                    // TODO: 分配账号
                    break;
            }
        }

        private void OnMsgLoginReply(MsgHeader msg)
        {
            if(_cs != null)
            {
                _cs.Dispose();
                _cs = null;
            }

            byte[] guidBuffer = new byte[Constants.GUID_LEN];
            for (int i = 0; i < Constants.GUID_LEN; i++)
            {
                guidBuffer[i] = (byte)msg.ReadByte();
            }

            uint userId = msg.ReadUint32();
            uint serverIp = msg.ReadUint32();
            int port = msg.ReadUint16();
            string strIp = msg.ReadStringA();
            string session = EncodeHelper.ByteArrayToString(guidBuffer);

            WLDebug.Log($"收到登陆返回，userId:{userId}, serverIp:{serverIp}, port:{port}, strIp:{strIp}, session:{session}");
            //WLDebugTrace.TraceInfo("[LoginManager] OnMsgLoginReply, ip=" + serverIp + ",port=" + port);

            if (QuickConfig.socketType == eSocketType.websocket) port = -1;
            onLogin?.Invoke(0, session, userId, strIp, port);
        }

        private void OnMsgLoginFailed(MsgHeader msg)
        {
            int byResult = msg.ReadByte();
            if (msg.len > msg.position)
            {
                uint dwUserID = msg.ReadUint32();
            }

            WLDebug.LogWarning("LoginMessages._OnMsgLoginFailed: result = ", byResult);
        }

        private void OnMsgSystemMessage(MsgHeader msg)
        {
            WLDebug.Info("_OnMsgSystemMessage");
        }

        private void OnMsgWebReply(MsgHeader msg)
        {
            WLDebug.Info("_OnMsgWebReply");
        }

        private void OnMsgAllocUserReply(MsgHeader msg)
        {
            WLDebug.Info("_OnMsgAllocUserReply");
        }
        #endregion


        #region Sending
        private void SendLoginCheckVersion()
        {
            // TODO: 获取版本号、appid、渠道号等
            ushort[] hallVersion = new ushort[] { 5, 9, 19, 0 };
            uint appId = 1002;
            uint channelId = 818;
            ushort clientType = Constants.CLIENT_TYPE_WEB;
            string deviceCode = "3626EF4D4BF53AB0F8C72DE74456961D14E99B85";

            MsgHeader msg = NetMsgFactory.GetNetMsgOut();
            msg.messageID = LoginMsgDefine.MSG_LOGIN_CHECK_VERSION;
            for (int i = 0; i < hallVersion.Length; ++i)
                msg.WriteUint16(hallVersion[i]);

            msg.WriteUint32(appId);
            msg.WriteUint32(channelId);
            msg.WriteUint16(clientType);

            for (int i = 0; i < Constants.DEVICE_CODE_LEN; i++)
            {
                if (i < deviceCode.Length)
                {
                    msg.WriteByte((byte)deviceCode[i]);
                }
                else
                {
                    msg.WriteByte(0);
                }
            }

            SendData(msg, Constants.CRYPT_MSG);
        }

        private void SendLoginByAccount(string account, string password, byte[] keyBuffer)
        {
            WLDebug.Info($"发送账号登陆请求, account: {account}, password: {password}");

            MsgHeader msgLogin = NetMsgFactory.GetNetMsgOut();
            msgLogin.messageID = LoginMsgDefine.MSG_LOGIN_BY_NAME2;
            msgLogin.WriteUint32(0);
            msgLogin.WriteByte(1); // is weile = true

            MsgHeader encryptStream = NetMsgFactory.GetNetMsgIn();
            encryptStream.WriteStringW(password);
            encryptStream.WriteStringW(account);

            // TODO: 获取设备码
            string deviceCode = "3626EF4D4BF53AB0F8C72DE74456961D14E99B85";
            byte[] codeBuffer = Encoding.ASCII.GetBytes(deviceCode);
            for (int i = 0; i < Constants.MACADDRESS_LEN; i++)
            {
                encryptStream.WriteByte(codeBuffer[i]);
            }
            for (int i = 0; i < Constants.MACADDRESS_LEN; i++)
            {
                encryptStream.WriteByte(0);
            }

            long size = encryptStream.position;
            byte[] data = DesCryptoHelper.Encrypt(encryptStream.ToArray(), keyBuffer);
            NetMsgFactory.RecycleNetMsg(encryptStream);

            long offset = size & 7; 
            if (offset != 0)
            {
                size += 8 - offset;
            }

            for (long i = 0; i < size; i++)
            {
                if (i < data.Length)
                {
                    msgLogin.WriteByte(data[i]);
                }
                else
                {
                    msgLogin.WriteByte(0);
                }
            }

            SendData(msgLogin, Constants.CRYPT_MSG);
        }
        #endregion

        protected void StartConnect()
        {
            // TODO: 从版本检测返回中获取登陆地址

            if(_cs == null)
                _cs = new ClientSocket(this);

            int port = QuickConfig.loginPort;
            if (QuickConfig.socketType == eSocketType.websocket)
                port = QuickConfig.loginPortWebSocket;

            _cs.Connect(QuickConfig.loginIp, port);
        }

        protected override bool NeedsWaiting(MsgHeader msg)
        {
            return false;
        }
    }
}
