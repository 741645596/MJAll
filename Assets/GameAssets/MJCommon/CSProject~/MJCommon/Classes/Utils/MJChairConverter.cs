// @Author: tanjinhua
// @Date: 2021/5/1  23:58



namespace MJCommon
{

    internal class MJChairConverter
    {
        internal static int ToView(int playersPerDesk, int basicViewChairId)
        {
            switch (playersPerDesk)
            {
                case 2:
                    return ToViewChairIdByTwoPlayer(basicViewChairId);

                case 3:
                    return ToViewChairIdByThreePlayer(basicViewChairId);

                default:
                    return basicViewChairId;
            }
        }


        internal static int ToViewBasic(int playersPerDesk, int viewChairId)
        {
            int result;

            switch (playersPerDesk)
            {
                case 2:
                    result = GetBasicViewChairIdByTwoPlayer(viewChairId);
                    break;

                case 3:
                    result = GetBasicViewChairIdByThreePlayer(viewChairId);
                    break;

                default:
                    result = viewChairId;
                    break;
            }

            return result;
        }


        internal static int ToNextView(int playersPerDesk, int currentViewChairId)
        {
            switch (playersPerDesk)
            {
                case 2:
                    return GetNextChairIdByTwoPlayer(playersPerDesk, currentViewChairId);

                case 3:
                    return GetNextChairIdByThreePlayer(playersPerDesk, currentViewChairId);

                default:
                    return (currentViewChairId + 1) % playersPerDesk;
            }
        }


        private static int GetNextChairIdByTwoPlayer(int playersPerDesk, int currentViewChairId)
        {
            switch (currentViewChairId)
            {
                case Chair.Down:
                    return Chair.Up;

                case Chair.Up:
                    return Chair.Down;

                default:
                    return (currentViewChairId + 1) % playersPerDesk;
            }
        }


        private static int GetNextChairIdByThreePlayer(int playersPerDesk, int currentViewChairId)
        {
            switch (currentViewChairId)
            {
                case Chair.Down:
                    return Chair.Right;

                case Chair.Right:
                    return Chair.Left;

                case Chair.Left:
                    return Chair.Down;

                default:
                    return (currentViewChairId + 1) % playersPerDesk;
            }
        }


        private static int GetBasicViewChairIdByThreePlayer(int viewChairId)
        {
            switch (viewChairId)
            {
                case Chair.Down:
                    return 0;

                case Chair.Right:
                    return 1;

                case Chair.Left:
                    return 2;

                default: return viewChairId;
            }
        }


        private static int GetBasicViewChairIdByTwoPlayer(int viewChairId)
        {
            switch (viewChairId)
            {
                case Chair.Down:
                    return 0;

                case Chair.Up:
                    return 1;

                default:
                    return viewChairId;
            }
        }


        private static int ToViewChairIdByTwoPlayer(int basic)
        {
            switch (basic)
            {
                case 0:
                    return Chair.Down;

                case 1:
                    return Chair.Up;

                default:
                    return basic;
            }
        }

        private static int ToViewChairIdByThreePlayer(int basic)
        {
            switch (basic)
            {
                case 0:
                    return Chair.Down;

                case 1:
                    return Chair.Right;

                case 2:
                    return Chair.Left;

                default:
                    return basic;
            }
        }
    }
}
