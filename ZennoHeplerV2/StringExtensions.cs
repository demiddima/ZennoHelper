using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennoHelperV2
{
    public static class StringExtensions
    {
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        public static double ToDouble(this string str)
        {
            return double.Parse(str);
        }

        public static float ToFloat(this string str)
        {
            return float.Parse(str);
        }

        public static bool ToBool(this string str)
        {
            return bool.Parse(str);
        }

        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }

        public static long ToLong(this string str)
        {
            return long.Parse(str);
        }
    }
}
