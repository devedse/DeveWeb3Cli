using CommandLine;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeveWeb3Cli.Commands.Contract.Call
{
    [Verb("call", HelpText = "Call contract function")]
    public class ContractCallCommand : ContractCommand
    {
        public static readonly Regex TheStringSplitterRegex = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+", RegexOptions.Compiled);

        [Option("private-key", Env = "WEB3_PRIVATE_KEY", Required = false, HelpText = "The private key [$WEB3_PRIVATE_KEY]")]
        public string? PrivateKey { get; set; }

        [Option("timeout", Required = false, Default = 60, HelpText = "Timeout in seconds (default: 60).")]
        public int TimeoutInSeconds { get; set; }

        [Option("abi", Required = true, HelpText = "ABI file matching deployed contract")]
        public string AbiFilePath { get; set; } = null!;

        [Option("function", Required = true, HelpText = "Target function name")]
        public string Function { get; set; } = null!;

        [Option("address", Env = "WEB3_ADDRESS", Required = false, HelpText = "Deployed contract address [$WEB3_ADDRESS]")]
        public string Address { get; set; } = null!;

        [Option("amount", Required = false, HelpText = "Amount in wei that you want to send to the transaction")]
        public string Amount { get; set; } = null!;

        [Option("jsondatafilepath", Required = false, HelpText = "Json File Path with data to pass to smart contract function call")]
        public string? JsonDataFilePath { get; set; }

        [Value(1, Required = false)]
        public IEnumerable<string>? Data { get; set; }

        public override async Task Execute()
        {
            if (string.IsNullOrEmpty(PrivateKey))
            {
                throw new ArgumentException("private-key field should have been provided.");
            }

            if (string.IsNullOrEmpty(RpcUrl))
            {
                throw new ArgumentException("rpc-url field should have been provided.");
            }

            if (!File.Exists(AbiFilePath))
            {
                throw new ArgumentException($"Could not find file in path {AbiFilePath}");
            }

            if (string.IsNullOrWhiteSpace(Function))
            {
                throw new ArgumentException("Function field should have been provided.");
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                throw new ArgumentException("Address field should have been provided.");
            }

            var abi = File.ReadAllText(AbiFilePath);

            if (Path.GetExtension(AbiFilePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                using (var document = JsonDocument.Parse(abi))
                {
                    var abielement = document.RootElement.EnumerateObject().FirstOrDefault(t => t.Name == "abi");
                    abi = abielement.Value.ToString();
                }
            }

            var account = new Account(PrivateKey, GetChainId());
            var web3 = new Web3(account, RpcUrl);

            var contract = web3.Eth.GetContract(abi, Address);
            var theFunction = contract.GetFunction(Function);


            object[] data = new object[0];
            if (!string.IsNullOrWhiteSpace(JsonDataFilePath) && File.Exists(JsonDataFilePath))
            {
                var jsonData = File.ReadAllText(JsonDataFilePath);

                data = theFunction.ConvertJsonToObjectInputParameters(jsonData);
                //data = DeveWeb3Cli.Helpers.JsonParameterObjectConvertorTestje.ConvertToFunctionInputParameterValues(jsonData, contract.ContractBuilder.GetFunctionBuilder(Function).FunctionABI);
            }
            else if (Data != null && Data.Any())
            {
                var dataString = string.Join(" ", Data);

                Console.WriteLine($"Input data: {dataString}");

                var dataAsList = TheStringSplitterRegex.Matches(dataString).Cast<Match>().Select(m => m.Value).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                var functionBuilder = contract.ContractBuilder.GetFunctionBuilder(Function);
                var functionAbi = functionBuilder.FunctionABI;

                var parametersInOrder = functionAbi.InputParameters.OrderBy(x => x.Order).ToList();

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
                    var val = JValue.Parse(dataObject);
                    jsonData.Add(parameter.Name, val);
                }

                data = theFunction.ConvertJsonToObjectInputParameters(jsonData);
                //data = DeveWeb3Cli.Helpers.JsonParameterObjectConvertorTestje.ConvertToFunctionInputParameterValues(jsonData, contract.ContractBuilder.GetFunctionBuilder(Function).FunctionABI);
            }

            var gasEstimate = await theFunction.EstimateGasAsync(account.Address, new HexBigInteger(6000000), new HexBigInteger(0), data);
            var transactionHash = await theFunction.SendTransactionAsync(account.Address, new HexBigInteger(gasEstimate), new HexBigInteger(0), data);
            Console.WriteLine($"TransactionHash: {transactionHash}");

            var receipt = await BlockchainService.WaitForReceipt(web3, transactionHash);

            if (receipt.HasErrors() == true)
            {
                throw new InvalidOperationException($"Error while executing transaction with hash: {transactionHash}");
            }

            if (!string.IsNullOrEmpty(OutputJsonPath))
            {
                var jsonTxt = JsonConvert.SerializeObject(receipt, Formatting.Indented);
                File.WriteAllText(OutputJsonPath, jsonTxt);
            }
        }
    }
}
