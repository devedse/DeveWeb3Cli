using CommandLine;
using DeveWeb3Cli.InputConverters;
using Nethereum.ABI.ABIDeserialisation;
using Nethereum.Contracts;
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
        [Option("jsondatafilepath", Required = false, HelpText = "Json File Path with data to pass to smart contract function call")]
        public string? JsonDataFilePath { get; set; }

        [Value(1, Required = true)]
        public string ContractFilePath { get; set; } = null!;

        [Value(2, Required = false)]
        public IEnumerable<string>? Data { get; set; }

        public override async Task Execute()
        {
            string? abi = null;
            if (!string.IsNullOrWhiteSpace(AbiFilePath))
            {
                if (!File.Exists(AbiFilePath))
                {
                    throw new ArgumentException($"Could not find file in path {AbiFilePath}");
                }

                abi = File.ReadAllText(AbiFilePath);

                if (Path.GetExtension(AbiFilePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
                {
                    using (var document = JsonDocument.Parse(abi))
                    {
                        var abielement = document.RootElement.EnumerateObject().FirstOrDefault(t => t.Name == "abi");
                        abi = abielement.Value.ToString();
                    }
                }
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


            var data = new object[0];

            if (Data?.Any() == true)
            {
                if (string.IsNullOrWhiteSpace(abi))
                {
                    throw new ArgumentException("Abi should be provided when passing constructor parameters to the smart contract.");
                }

                var contractABI = ABIDeserialiserFactory.DeserialiseContractABI(abi);
                data = BlockchainService.CreateInputData(contractABI.Constructor.InputParameters, JsonDataFilePath, Data);
            }




            var deployContractTransBuilder = new DeployContractTransactionBuilder();
            var calldata = deployContractTransBuilder.BuildTransaction(abi, byteCode, account.Address, new HexBigInteger(6000000), GasPrice != null ? new HexBigInteger(GasPrice.Value) : null, Value.AsHexBigIntegerWith0(), data);

            var gasEstimate = GasLimit;

            if (gasEstimate == null)
            {
                //var gasEstimate2 = await web3.Eth.DeployContract.EstimateGasAsync(abi, byteCode, account.Address, data);
                gasEstimate = await web3.TransactionManager.EstimateGasAsync(calldata);
                //var gasEstimate = await web3.Eth.DeployContract.EstimateGasAsync("", byteCode, account.Address);
            }

            string transactionHash;

            if (GasPrice != null)
            {
                transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, account.Address, new HexBigInteger(gasEstimate.Value), GasPrice.AsHexBigIntegerWithNull(), Value.AsHexBigIntegerWith0(), data);
            }
            else
            {
                transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, account.Address, new HexBigInteger(gasEstimate.Value), MaxFeePerGas.AsHexBigIntegerWithNull(), MaxPriorityFeePerGas.AsHexBigIntegerWithNull(), Value.AsHexBigIntegerWith0(), null, data);
            }

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
