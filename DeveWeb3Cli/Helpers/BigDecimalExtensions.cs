using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveWeb3Cli.Helpers
{
    public static class BigDecimalTryParse
    {
        public static bool TryParse(string value, out BigDecimal result)
        {
            try
            {
                result = BigDecimal.Parse(value);
                return true;
            }
            catch
            {
                result = new BigDecimal(0);
                return false;
            }
        }
    }
}
