using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DeveWeb3Cli.Services
{
    public class BlockchainService : IBlockchainService
    {
        public static readonly Regex TheStringSplitterRegex = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+", RegexOptions.Compiled);

        public BlockchainService()
        {

        }

        public async Task<TransactionReceipt> WaitForReceipt(Web3 web3, string transactionHash)
        {
            Console.WriteLine("Waiting for receipt...");

            Task<TransactionReceipt> GetReceipt()
            {
                return web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            var receipt = await GetReceipt();

            while (receipt == null)
            {
                Console.WriteLine($"Still waiting on transaction: {transactionHash}");
                await Task.Delay(5000);
                receipt = await GetReceipt();
            }

            Console.WriteLine($"Got receipt for TransactionHash: {transactionHash}. Result: {receipt.Status.Value}");

            return receipt;
        }

        public object[] CreateInputData(Parameter[] parameters, string? jsonDataFilePath, IEnumerable<string>? Data)
        {

            object[] data = new object[0];
            if (!string.IsNullOrWhiteSpace(jsonDataFilePath) && File.Exists(jsonDataFilePath))
            {
                var jsonData = File.ReadAllText(jsonDataFilePath);

                //data = theFunction.ConvertJsonToObjectInputParameters(jsonData);
                data = DeveWeb3Cli.Helpers.JsonParameterObjectConvertorTestje.ConvertToInputParameterValues(jsonData, parameters);
            }
            else if (Data != null && Data.Any())
            {
                var dataString = string.Join(" ", Data);

                Console.WriteLine($"Input data: {dataString}");

                var dataAsList = TheStringSplitterRegex.Matches(dataString).Cast<Match>().Select(m => m.Value).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                var parametersInOrder = parameters.OrderBy(x => x.Order).ToList();

                if (parametersInOrder.Count != dataAsList.Count)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < dataAsList.Count; i++)
                    {
                        sb.AppendLine($"{i}: {dataAsList[i]}");
                    }
                    Console.WriteLine($"Data as string: >{dataString}<");
                    throw new ArgumentException($"Expected {parametersInOrder.Count} elements in Data, but got: {dataAsList.Count}{Environment.NewLine}{sb}");
                }


                var jsonData = new JObject();
                for (int i = 0; i < parametersInOrder.Count; i++)
                {
                    var parameter = parametersInOrder[i];
                    var dataObject = dataAsList[i];

                    var abiType = parameter.ABIType;

                    JToken jtoken;
                    try
                    {
                        jtoken = JToken.Parse(dataObject);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            jtoken = JToken.FromObject(dataObject);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Could not parse {dataObject}");
                            throw;
                        }
                    }
                    jsonData.Add(parameter.Name, jtoken);
                }

                //data = theFunction.ConvertJsonToObjectInputParameters(jsonData);
                data = DeveWeb3Cli.Helpers.JsonParameterObjectConvertorTestje.ConvertToInputParameterValues(jsonData, parameters);
            }

            return data;
        }
    }
}
