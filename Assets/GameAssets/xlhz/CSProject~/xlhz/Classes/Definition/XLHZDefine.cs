// @Author: tanjinhua
// @Date: 2021/9/2  14:30

using System;

namespace NS_XLHZ
{
    public class XLHZOperType
    {
        public const int None       =   0;
        public const int Gang_An    =   1;				    // 暗杠
        public const int Gang_Ming  =   2;				    // 明杠
        public const int Gang_Bu    =   3;				    // 补杠
        public const int ZiMo       =   4;				    // 自摸
        public const int DianPao    =   5;				    // 点炮
        public const int CHZ        =   6;				    // 查花猪
        public const int CDJ        =   7;				    // 查大叫
        public const int HJZY       =   8;				    // 呼叫转移
        public const int TuiShui    =   9;				    // 退税
        public const int Piao       =   10;                 // 飘
    }

    public class XLHZHuType
    {
        // 胡牌类型
        public const long None          =   0x0000000000000000;
        public const long QinYaoJiu     =   0x000000000000001;   //清幺九 全由序数牌1和9组成的胡牌  88    不计碰碰胡、双同刻
        public const long LianQiDui     =   0x000000000000002;  // 连七对  由同一种花色序数牌组成序数相连的7个对子的胡牌 不计清一色、不求人、7对
        public const long YiSeSLH       =   0x000000000000004;   //一色双龙会
        public const long TianHu        =   0x000000000000008;    //天胡
        public const long JiuLianBaoDeng=   0x000000000000010;    //九莲宝灯
        public const long LvYiSe        =   0x000000000000020;    //绿一色
        public const long QuanYaoJiu    =   0x000000000000040;    //全带幺
        public const long QuanDaiWu     =   0x000000000000080;//全带五
        public const long ShiBaLuoHan   =   0x000000000000100;//十八罗汉
        public const long DiHu          =   0x000000000000200;//地胡
        public const long QuanDa        =   0x000000000000400;//全大
        public const long QuanZhong     =   0x000000000000800;//全中
        public const long QuanXiao      =   0x000000000001000;//全小
        public const long SiAnKe        =   0x000000000002000;//四暗刻
        public const long SiJieGao      =   0x000000000004000;//四节高
        public const long LongQiDui     =   0x000000000008000;//龙七对
        public const long SanGangZi     =   0x000000000010000;//十二金钗
        public const long QiDui         =   0x000000000020000;//七对
        public const long QuanShuangKe  =   0x000000000040000;//全双刻
        public const long SanJieGao     =   0x000000000080000;//三节高
        public const long JinGouDiao    =   0x000000000100000;//金钩钓
        public const long DaWu          =   0x000000000200000;//大于五
        public const long XiaoWu        =   0x000000000400000;//小于五
        public const long QingYiSe      =   0x000000000800000;//清一色
        public const long SanAnKe       =   0x000000001000000;//三暗刻
        public const long QingLong      =   0x000000002000000;//清龙
        public const long GangShangHua  =   0x000000004000000;//杠上开花
        public const long TuiBuDao      =   0x000000008000000;//推不倒
        public const long BuQiuRen      =   0x000000010000000;//不求人
        public const long DuiDuiHu      =   0x000000020000000;//碰碰胡（对对胡）
        public const long LaoShaoFu     =   0x000000040000000;//老少副 
        public const long MenQing       =   0x000000080000000;//门清
    
        public const long DuanYaoJiu    =   0x000000100000000;//断幺九
        public const long ShuangAnKe    =   0x000000200000000;//双暗刻
        public const long ShuangTongKe  =   0x000000400000000;//双同刻
        public const long KanZhang      =   0x000000800000000;//坎张（中张）
        public const long BianZhang     =   0x000001000000000;//边张
        public const long MiaoShouHuiChun=  0x000002000000000;//妙手回春
        public const long HaiDiLaoYue   =   0x000004000000000;//海底捞月
        public const long GangShangPao  =   0x000008000000000;//杠上炮
        public const long JueZhang      =   0x000010000000000;//绝张
        public const long QiangGangHu   =   0x000020000000000;//抢杠胡
        public const long TuiDaoHu      =   0x000040000000000;//推倒胡
        public const long AnGang        =   0x000080000000000;//暗杠
        public const long MingGang      =   0x000100000000000;//明杠
        public const long Gen           =   0x000200000000000;//根
        public const long HuTypeZiMo    =   0x000400000000000;//自摸

        public const long SuHu          =   0x000800000000000;//素胡
        public const long YiBanGao      =   0x001000000000000;//一般高
        public const long LiuLianShun   =   0x002000000000000;//六连顺
        public const long ZhuoWuKui     =   0x004000000000000;//捉五魁
        public const long BaiWanShi     =   0x008000000000000;//百万石
        public const long JiangDui      =   0x010000000000000;//将对

        public const long Hu            =   0x080000000000000;//
    }

    public class XLHZActionType
    {
        public const uint None 			    = 0x00000000; 			// 无动作    
	    public const uint Chi_Left 		    = 0x00000001; 			// 左吃
	    public const uint Chi_Center 		= 0x00000002; 			// 中吃
	    public const uint Chi_Right 		= 0x00000004; 			// 右吃
	    public const uint Peng 			    = 0x00000008; 			// 碰
	    public const uint DingZhang 		= 0x00000010; 			// 定掌
	    public const uint Gang_Ming 		= 0x00000020; 			// 明杠
	    public const uint Gang_An 			= 0x00000040; 			// 暗杠(64)
	    public const uint Gang_PuBuGang 	= 0x00000080; 			// 普补杠(由碰补成明杠)
	    public const uint Gang_Xi 			= 0x00000100; 			// 喜杠
	    public const uint Gang_Yao 		    = 0x00000200; 			// 幺杠
	    public const uint Gang_Jiu 		    = 0x00000400; 			// 九杠        
	    public const uint Gang_3Feng_QueDong = 0x00000800; 			// 三风杠(西南北)
	    public const uint Gang_3Feng_QueNan  = 0x00001000; 			// 三风杠(东西北)
	    public const uint Gang_3Feng_QueXi   = 0x00002000; 			// 三风杠(东南北)
	    public const uint Gang_3Feng_QueBei  = 0x00004000; 			// 三风杠(东南西)
	    public const uint Gang_4Feng 		= 0x00008000; 			// 四风杠
	    public const uint Gang_HuiPi 		= 0x00010000; 			// 会皮杠
	    public const uint TiHui 			= 0x00020000; 			// 提会
	    public const uint Ting 			    = 0x00040000; 			// 听牌
	    public const uint Chi_Left_Ting 	= 0x00080000; 			// 左吃听
	    public const uint Chi_Center_Ting 	= 0x00100000; 			// 中吃听
	    public const uint Chi_Right_Ting 	= 0x00200000; 			// 右吃听
	    public const uint Peng_Ting 		= 0x00400000; 			// 碰听
	    public const uint Gang_Ting 		= 0x00800000; 			// 杠听
	    public const uint Hu 				= 0x01000000; 			// 胡牌
	    public const uint Hu_BaoPai 		= 0x02000000; 			// 宝牌
	    public const uint Hu_dianpao 		= 0x03000000; 			// 点炮 （客户端后填）
	    public const uint Hu_zimo 			= 0x05000000; 			// 自摸 （客户端后填）
	    public const uint Hu_duibao 		= 0x06000000; 			// 对宝 （客户端后填）
	    public const uint Chu_Pai 			= 0x04000000; 			// 选择出牌牌值
	    public const uint Gang_Hui 		    = 0x08000000; 			// 会杠
	    public const uint Gang_Hui_1Gang 	= 0x10000000; 			// 会幺杠
	    public const uint Gang_Hui_9Gang 	= 0x20000000; 			// 会九杠
	    public const uint Gang_Hui_XiGang 	= 0x40000000; 			// 会喜杠
	    public const uint Gang_Hui_FengGang = 0x80000000; 			// 会风杠
	    public const uint ZhiDui 			= 0x00000800; 			// 支对
	
	    //////////////////////////////actionType2 分割线 //////////////////////////////////////

	    public const uint TiHui_1Gang 		= 0x00000001; 			// 提1
	    public const uint TiHui_9Gang 		= 0x00000002; 			// 提9
	    public const uint TiHui_FengGang 	= 0x00000004; 			// 提风
	    public const uint TiHui_XiGang 	    = 0x00000008; 			// 提喜
	    public const uint XiGang_Zhong 	    = 0x00000010; 			// 喜杠带中 
	    public const uint XiGang_Fa 		= 0x00000020; 			// 喜杠带发
	    public const uint XiGang_Bai 		= 0x00000040; 			// 喜杠带白
	    public const uint PengBao 			= 0x00000080; 			// 碰宝(算明杠
	    public const uint GangBao 			= 0x00000100; 			// 杠宝牌（算暗杠）
	    public const uint QueTuiYJGang 	    = 0x00000200; 			// 瘸腿幺九杠
	    public const uint FangQi 			= 0x10000000; 			// 放弃
    }
}
