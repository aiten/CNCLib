using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public static class Tools
    {
        public static int MulDivRound32(int val, int mul, int div)
        {
            return (val * mul + div / 2) / div;
        }
        public static decimal MulDiv(decimal val, decimal mul, decimal div,int decimals)
        {
            return Math.Round((val * mul) / div,decimals);
        }
    }
}
