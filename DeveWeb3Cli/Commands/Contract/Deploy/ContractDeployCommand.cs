using CommandLine;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
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
        public string ContractFileName { get; set; } = null!;

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

            if (!File.Exists(ContractFileName))
            {
                throw new ArgumentException($"Could not find file in path {ContractFileName}");
            }

            var byteCode = File.ReadAllText(ContractFileName);

            if (Path.GetExtension(ContractFileName).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                using (var document = JsonDocument.Parse(byteCode))
                {
                    var bytecodelement = document.RootElement.EnumerateObject().FirstOrDefault(t => t.Name == "bytecode");
                    byteCode = bytecodelement.Value.GetString();
                }
            }

            var account = new Account(PrivateKey, Nethereum.Signer.Chain.Ropsten);
            var web3 = new Web3(account, RpcUrl);



            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(byteCode, account.Address);
            Console.WriteLine($"TransactionHash: {transactionHash}");


            Console.WriteLine("Waiting for receipt...");
            TransactionReceipt receipt;
            do
            {
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                await Task.Delay(5000);
            }
            while (receipt == null);

            Console.WriteLine($"ContractAddress: {receipt.ContractAddress}");
        }
    }
}
