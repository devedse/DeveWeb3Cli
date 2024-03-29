﻿using Nethereum.ABI;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace DeveWeb3Cli.Helpers
{
    public static class JsonParameterObjectConvertorTestje
    {
        public static object[] ConvertToInputParameterValues(string json, Parameter[] parameters)
        {
            var jObject = JObject.Parse(json);
            return ConvertToInputParameterValues(jObject, parameters);
        }

        public static object[] ConvertToInputParameterValues(this JToken jObject, Parameter[] parameters)
        {
            var output = new List<object>();
            var parametersInOrder = parameters.OrderBy(x => x.Order);
            foreach (var parameter in parametersInOrder)
            {
                var abiType = parameter.ABIType;
                var jToken = jObject[parameter.Name];

                AddJTokenValueInputParameters(output, abiType, jToken);
            }

            return output.ToArray();
        }

        private static void AddJTokenValueInputParameters(List<object> inputParameters, ABIType abiType, JToken jToken)
        {
            var tupleAbi = abiType as TupleType;
            if (tupleAbi != null)
            {
                var tupleValue = jToken;
                inputParameters.Add(ConvertToInputParameterValues(tupleValue, tupleAbi.Components));
            }

            var arrayAbi = abiType as ArrayType;
            if (arrayAbi != null)
            {
                var array = (JArray)jToken;
                var elementType = arrayAbi.ElementType;
                var arrayOutput = new List<object>();
                foreach (var element in array)
                {
                    AddJTokenValueInputParameters(arrayOutput, elementType, element);
                }
                inputParameters.Add(arrayOutput);
            }

            if (abiType is Bytes32Type || abiType is BytesType)
            {
                var bytes = jToken.ToObject<string>().HexToByteArray();
                inputParameters.Add(bytes);
            }

            if (abiType is StringType || abiType is AddressType)
            {
                inputParameters.Add(jToken.ToObject<string>());
            }

            if (abiType is IntType)
            {
                inputParameters.Add(BigInteger.Parse(jToken.ToObject<string>()));
            }

            if (abiType is BoolType)
            {
                inputParameters.Add(jToken.ToObject<bool>());
            }
        }
    }
}
