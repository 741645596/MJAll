using System;
using System.Collections.Generic;
using System.Linq;

namespace WLCore.Helper
{
    public static class BitHelper
    {
        public static uint And(uint a, uint b)
        {
            return (a & b) >> 0;
        }

        public static uint Or(uint a, uint b)
        {
            return (a | b) >> 0;
        }
    }
}
