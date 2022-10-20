using System.Collections.Generic;

namespace WLCore.Helper
{
    public static class BatchCommandHelp
    {
        /// <summary>
        /// 调用Unity批处理命令时候shell传递过来的 参数解析,获得数组内的参数
        /// </summary>
        /// <param name="command"> python调用Unity的类名和方法名 例如:CommandTool.PackAssets </param>
        /// <returns> 返回参数列表 </returns>
        public static List<string> GetArgs(string command)
        {
            //获取Python传过来的参数
            string[] args     = System.Environment.GetCommandLineArgs();
            int      length   = args.Length;
            int      startIdx = 0;
            for (int i = 0; i < length; i++)
            {
                if (args[i] == command)
                {
                    startIdx = i;
                }
            }

            List<string> list = new List<string>();

            for (int i = startIdx + 1; i < length && i > startIdx; i++)
            {
                list.Add(args[i]);
            }

            if (list.Count < 1)
            {
                WLDebug.LogError("接收不到python传递给unity的参数");
                list.Add("");
            }

            return list;
        }
    }
}
