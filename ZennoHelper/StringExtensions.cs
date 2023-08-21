using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennoHelper
{
    public static class StringExtensions
    {
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }
    }
}
