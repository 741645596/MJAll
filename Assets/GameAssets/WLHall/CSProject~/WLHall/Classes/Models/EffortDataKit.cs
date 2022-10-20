// @Author: xuzhihu
// @Date: 2021/4/22 14:43:50 
// @Description: 
using System;

namespace WLHall
{
    public static class EffortDataKit
    {
        private static long[] _effortData = new long[Constants.EFFORT_DATA_COUNT]; // 成就任务数据
        private static long[] _dailyData =  new long[Constants.EFFORT_DATA_COUNT]; // 日常任务数据

        public static void SetEffortData(int k, long v)
        {
            _effortData[k] = v;
        }

        public static void SetDailyData(int k, long v)
        {
            _dailyData[k] = v;
        }
    }
}
