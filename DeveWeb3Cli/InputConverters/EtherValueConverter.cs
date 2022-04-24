//using CommandLine;
//using CSharpx;
//using Nethereum.Web3;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;
//using static Nethereum.Util.UnitConversion;

//namespace DeveWeb3Cli.InputConverters
//{
//    public class EtherValueConverter : ICustomConverter
//    {
//        public Maybe<object> Convert(IEnumerable<string> vals, Type type, bool isScalar)
//        {
//            var firstVal = vals.First();

//            var splitted = firstVal.Split('_', StringSplitOptions.RemoveEmptyEntries);

//            var ethValue = splitted.First();
//            var ethUnit = "wei";
//            if (splitted.Length > 1)
//            {
//                ethUnit = splitted[1];
//            }

//            if (BigInteger.TryParse(ethValue, out var resultParseGasPrice))
//            {
//                var desiredEthUnit = Enum.GetValues<EthUnit>().Where(t => t.ToString().Equals(ethUnit, StringComparison.OrdinalIgnoreCase)).ToList();

//                if (desiredEthUnit.Count == 0)
//                {
//                    throw new ArgumentException($"Could not parse EthUnit: {ethUnit}");
//                }
//                var theEthUnit = desiredEthUnit.First();
//                var gasPrice = Web3.Convert.ToWei(resultParseGasPrice, theEthUnit);
//                return new Just<object>(gasPrice);
//            }
//            return new Just<object>(null!);
//        }
//    }
//}
