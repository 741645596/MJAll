

using WLCore;
using WLHall;


public class FuziData
{
    public uint weaveKind1;
    public uint weaveKind2;
    public ushort provideUser;
    public int operateCard;
    public int tiHuiCard;
    public int[] cardValues;
    public int[] cardCounts;
    public int yaoJiCard;

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool isValid => weaveKind1 != 0 || weaveKind2 != 0;

    /// <summary>
    /// 是否是暗杠
    /// </summary>
    public bool isConcealedKong =>
        cardValues[0] == 0xFF &&
        cardValues[1] == 0xFF &&
        cardValues[2] == 0xFF;

    /// <summary>
    /// 是否是明杠
    /// </summary>
    public bool isExposedKong =>
        (cardValues[3] == cardValues[0] || cardValues[3] == 0xFF) &&
        cardValues[0] == cardValues[1] &&
        cardValues[1] == cardValues[2];

    public FuziData()
    {
        cardValues = new int[4];
        cardCounts = new int[4];
    }

    public static FuziData From(MsgHeader msg)
    {
        FuziData fuzi = new FuziData
        {
            weaveKind1 = msg.ReadUint32(),
            weaveKind2 = msg.ReadUint32(),
            provideUser = msg.ReadUint16(),
            operateCard = msg.ReadByte(),
            tiHuiCard = msg.ReadByte()
        };

        for (var i = 0; i < 4; i++)
        {
            fuzi.cardValues[i] = msg.ReadByte();
        }

        for (var i = 0; i < 4; i++)
        {
            fuzi.cardCounts[i] = msg.ReadByte();
        }

        fuzi.yaoJiCard = msg.ReadByte();
        return fuzi;
    }
}

