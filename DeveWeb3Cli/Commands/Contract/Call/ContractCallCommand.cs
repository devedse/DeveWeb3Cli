using CommandLine;
using DeveWeb3Cli.InputConverters;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Text.Json;

namespace DeveWeb3Cli.Commands.Contract.Call
{
    [Verb("call", HelpText = "Call contract function")]
    public class ContractCallCommand : ContractCommand
    {
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

            if (string.IsNullOrWhiteSpace(Function))
            {
                throw new ArgumentException("Function field should have been provided.");
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                throw new ArgumentException("Address field should have been provided.");
            }

            if (!File.Exists(AbiFilePath))
            {
                throw new ArgumentException($"Could not find file in path {AbiFilePath}");
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


            var gasEstimate = GasLimit;
            if (gasEstimate == null)
            {
                //var gasEstimate2 = await web3.Eth.DeployContract.EstimateGasAsync(abi, byteCode, account.Address, data);
                gasEstimate = await theFunction.EstimateGasAsync(account.Address, new HexBigInteger(6000000), Value.AsHexBigIntegerWith0(), data);
                //var gasEstimate = await web3.Eth.DeployContract.EstimateGasAsync("", byteCode, account.Address);
            }

            string transactionHash;
            if (GasPrice != null)
            {
                transactionHash = await theFunction.SendTransactionAsync(account.Address, new HexBigInteger(gasEstimate.Value), GasPrice.AsHexBigIntegerWithNull(), Value.AsHexBigIntegerWith0(), data);
            }
            else
            {
                transactionHash = await theFunction.SendTransactionAsync(account.Address, new HexBigInteger(gasEstimate.Value), Value.AsHexBigIntegerWith0(), MaxFeePerGas.AsHexBigIntegerWithNull(), MaxPriorityFeePerGas.AsHexBigIntegerWithNull(), data);
            }

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
