
using WLCore;
using WLHall;

public class MsgSystemTime
{
    public ushort wYear;
    public ushort wMonth;
    public ushort wDayOfWeek;
    public ushort wDay;
    public ushort wHour;
    public ushort wMinute;
    public ushort wSecond;
    public ushort wMilliseconds;

    public static MsgSystemTime From(MsgHeader msg)
    {
        MsgSystemTime systemTime = new MsgSystemTime
        {
            wYear = msg.ReadUint16(),
            wMonth = msg.ReadUint16(),
            wDayOfWeek = msg.ReadUint16(),
            wDay = msg.ReadUint16(),
            wHour = msg.ReadUint16(),
            wMinute = msg.ReadUint16(),
            wSecond = msg.ReadUint16(),
            wMilliseconds = msg.ReadUint16()
        };

        return systemTime;
    }
    // TODO 美化显示 可删除
    public override string ToString()
    {
        return System.Text.RegularExpressions.Regex.Unescape(LitJson.JsonMapper.ToJson(this));
    }
}
