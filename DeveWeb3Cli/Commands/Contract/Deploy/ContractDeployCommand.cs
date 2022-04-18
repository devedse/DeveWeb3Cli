using CommandLine;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Text.Json;

namespace DeveWeb3Cli.Commands.Contract.Deploy
{
    [Verb("deploy", HelpText = "Deploy the specified contract to the network. eg: web3 contract deploy MyContract.bin")]
    public class ContractDeployCommand : ContractCommand
    {
        [Option("private-key", Env = "WEB3_PRIVATE_KEY", Required = false, HelpText = "The private key [$WEB3_PRIVATE_KEY]")]
        public string? PrivateKey { get; set; }

        [Option("timeout", Required = false, Default = 60, HelpText = "Timeout in seconds (default: 60).")]
        public int TimeoutInSeconds { get; set; }

        [Value(1, Required = true)]
        public string ContractFilePath { get; set; } = null!;

        [Value(2, Required = false)]
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

            if (!File.Exists(ContractFilePath))
            {
                throw new ArgumentException($"Could not find file in path {ContractFilePath}");
            }

            var byteCode = File.ReadAllText(ContractFilePath);

            if (Path.GetExtension(ContractFilePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                using (var document = JsonDocument.Parse(byteCode))
                {
                    var bytecodelement = document.RootElement.EnumerateObject().FirstOrDefault(t => t.Name == "bytecode");
                    byteCode = bytecodelement.Value.GetString();
                }
            }

            var account = new Account(PrivateKey, GetChainId());
            var web3 = new Web3(account, RpcUrl);

            var gasEstimate = await web3.Eth.DeployContract.EstimateGasAsync("", byteCode, account.Address);
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(byteCode, account.Address, new HexBigInteger(gasEstimate), new HexBigInteger(0));
            Console.WriteLine($"TransactionHash: {transactionHash}");

            var receipt = await BlockchainService.WaitForReceipt(web3, transactionHash);

            Console.WriteLine($"ContractAddress: {receipt.ContractAddress}");

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
