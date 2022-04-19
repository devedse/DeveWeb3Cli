using CommandLine;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Nethereum.Util.UnitConversion;

namespace DeveWeb3Cli.Commands.Contract.Call
{
    [Verb("call", HelpText = "Call contract function")]
    public class ContractCallCommand : ContractCommand
    {

        [Option("private-key", Env = "WEB3_PRIVATE_KEY", Required = false, HelpText = "The private key [$WEB3_PRIVATE_KEY]")]
        public string? PrivateKey { get; set; }

        [Option("timeout", Required = false, Default = 60, HelpText = "Timeout in seconds (default: 60).")]
        public int TimeoutInSeconds { get; set; }

        [Option("function", Required = true, HelpText = "Target function name")]
        public string Function { get; set; } = null!;

        [Option("address", Env = "WEB3_ADDRESS", Required = false, HelpText = "Deployed contract address [$WEB3_ADDRESS]")]
        public string Address { get; set; } = null!;

        [Option("amount", Required = false, HelpText = "Amount in wei that you want to send to the transaction")]
        public string Amount { get; set; } = null!;

        [Option("jsondatafilepath", Required = false, HelpText = "Json File Path with data to pass to smart contract function call")]
        public string? JsonDataFilePath { get; set; }



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

            BigInteger? gasPrice = null;
            if (!string.IsNullOrWhiteSpace(GasPriceGwei))
            {
                if (BigInteger.TryParse(GasPriceGwei, out var resultParseGasPrice))
                {
                    gasPrice = Web3.Convert.ToWei(resultParseGasPrice, EthUnit.Gwei);
                }
                else
                {
                    throw new ArgumentException($"Could not parse GasPriceGwei: {GasPriceGwei}");
                }
            }

            BigInteger? value = null;
            if (!string.IsNullOrWhiteSpace(Value))
            {
                if (BigInteger.TryParse(Value, out var resultParseValue))
                {
                    value = resultParseValue;
                }
                else
                {
                    throw new ArgumentException($"Could not parse Value: {Value}");
                }
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



            var functionBuilder = contract.ContractBuilder.GetFunctionBuilder(Function);
            var functionAbi = functionBuilder.FunctionABI;

            var data = BlockchainService.CreateInputData(functionAbi.InputParameters, JsonDataFilePath, Data);




            var gasEstimate = await theFunction.EstimateGasAsync(account.Address, new HexBigInteger(6000000), new HexBigInteger(value ?? 0), data);

            var transactionHash = await theFunction.SendTransactionAsync(account.Address, new HexBigInteger(gasEstimate), gasPrice != null ? new HexBigInteger(gasPrice.Value) : null, new HexBigInteger(value ?? 0), data);

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
