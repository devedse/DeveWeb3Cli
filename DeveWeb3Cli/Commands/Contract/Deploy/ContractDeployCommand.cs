using CommandLine;
using Nethereum.ABI.ABIDeserialisation;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Numerics;
using System.Text.Json;
using static Nethereum.Util.UnitConversion;

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

            string? abi = null;
            if (!string.IsNullOrWhiteSpace(AbiFilePath))
            {
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
                data = BlockchainService.CreateInputData(contractABI.Constructor.InputParameters, null, Data);
            }




            var deployContractTransBuilder = new DeployContractTransactionBuilder();
            var calldata = deployContractTransBuilder.BuildTransaction(abi, byteCode, account.Address, new HexBigInteger(6000000), gasPrice != null ? new HexBigInteger(gasPrice.Value) : null, new HexBigInteger(value ?? 0), data);

            //var gasEstimate2 = await web3.Eth.DeployContract.EstimateGasAsync(abi, byteCode, account.Address, data);
            var gasEstimate = await web3.TransactionManager.EstimateGasAsync(calldata);
            //var gasEstimate = await web3.Eth.DeployContract.EstimateGasAsync("", byteCode, account.Address);
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, account.Address, new HexBigInteger(gasEstimate), gasPrice != null ? new HexBigInteger(gasPrice.Value) : null, new HexBigInteger(value ?? 0), data);
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
