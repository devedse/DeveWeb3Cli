using CommandLine;
using DeveWeb3Cli.CommandHelpers;
using DeveWeb3Cli.Commands.Contract.Call;
using DeveWeb3Cli.Commands.Contract.Deploy;
using DeveWeb3Cli.InputConverters;
using DeveWeb3Cli.Services;
using Ninject;
using System.Numerics;

namespace DeveWeb3Cli.Commands.Contract
{
    [Verb("contract", HelpText = "Contract operations")]
    [ChildVerbs(typeof(ContractDeployCommand), typeof(ContractCallCommand))]
    public abstract class ContractCommand : CommandBase
    {
        [Inject]
        public IBlockchainService BlockchainService { get; set; } = null!;

        [Option("rpc-url", Env = "WEB3_RPC_URL", Required = false, HelpText = "The network RPC URL [$WEB3_RPC_URL]")]
        public string RpcUrl { get; set; } = null!;

        [Option("outputjson", Required = false, HelpText = "The JSON file path to output to.")]
        public string? OutputJsonPath { get; set; }

        [Option("abi", Required = false, HelpText = "ABI file matching contract (Required when passing calling functions or when passing input parameters to constructor)")]
        public string AbiFilePath { get; set; } = null!;

        [Option("value", Required = false, HelpText = "The desired Value (e.g. 10_gwei)")]
        public EtherValue? Value { get; set; }

        [Option("gas-limit", Required = false, HelpText = "The desired Gas Limit (Will do automatic gas estimation if it is not provided)")]
        public BigInteger? GasLimit { get; set; }

        [Option("gas-price", SetName = "oldgas", Required = false, HelpText = "The desired GasPrice (e.g. 10_gwei)")]
        public EtherValue? GasPrice { get; set; }

        [Option("maxFeePerGas", SetName = "newgas", Required = false, HelpText = "The desired MaxFeePerGas (e.g. 10_gwei)")]
        public EtherValue? MaxFeePerGas { get; set; }
        [Option("maxPriorityFeePerGas", SetName = "newgas", Required = false, HelpText = "The desired MaxPriorityFeePerGas (e.g. 10_gwei)")]
        public EtherValue? MaxPriorityFeePerGas { get; set; }

        [Option("private-key", Env = "WEB3_PRIVATE_KEY", Required = true, HelpText = "The private key [$WEB3_PRIVATE_KEY]")]
        public string? PrivateKey { get; set; }

        [Option("timeout", Required = false, Default = 60, HelpText = "Timeout in seconds (default: 60).")]
        public int TimeoutInSeconds { get; set; }
    }
}
