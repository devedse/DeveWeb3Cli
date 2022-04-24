using DeveWeb3Cli.Helpers;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using System.Numerics;
using static Nethereum.Util.UnitConversion;

namespace DeveWeb3Cli.InputConverters
{
    public static class EtherValueExtensions
    {
        public static HexBigInteger? AsHexBigIntegerWithNull(this EtherValue? value)
        {
            if (value == null)
            {
                return null;
            }
            return new HexBigInteger(value.Value);
        }

        public static HexBigInteger AsHexBigIntegerWith0(this EtherValue? value)
        {
            if (value == null)
            {
                return new HexBigInteger(BigInteger.Zero);
            }
            return new HexBigInteger(value.Value);
        }
    }

    public class EtherValue
    {
        public BigInteger Value { get; }

        public static implicit operator BigInteger?(EtherValue? etherValue) => etherValue?.Value;

        public EtherValue(string data)
        {
            
            var arguments = data.Split('_', StringSplitOptions.RemoveEmptyEntries);

            var ethValue = arguments.First();
            var ethUnit = "wei";
            if (arguments.Length > 1)
            {
                ethUnit = arguments[1];
            }

            if (BigDecimalTryParse.TryParse(ethValue, out var resultParseGasPrice))
            {
                var desiredEthUnit = Enum.GetValues<EthUnit>().Where(t => t.ToString().Equals(ethUnit, StringComparison.OrdinalIgnoreCase)).ToList();

                if (desiredEthUnit.Count == 0)
                {
                    throw new ArgumentException($"Could not parse EthUnit: {ethUnit}");
                }
                var theEthUnit = desiredEthUnit.First();
                var gasPrice = Web3.Convert.ToWei(resultParseGasPrice, theEthUnit);
                Value = gasPrice;
            }
            else
            {
                throw new ArgumentException($"Could not parse '{ethValue}' as BigInteger");
            }
        }
    }
}
