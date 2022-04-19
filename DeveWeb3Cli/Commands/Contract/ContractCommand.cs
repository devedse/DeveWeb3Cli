using CommandLine;
using DeveWeb3Cli.CommandHelpers;
using DeveWeb3Cli.Commands.Contract.Call;
using DeveWeb3Cli.Commands.Contract.Deploy;
using DeveWeb3Cli.Services;
using Ninject;

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

        [Option("value", Required = false, Default = "0", HelpText = "")]
        public string? Value { get; set; }

        [Option("gas-price-gwei", Required = false, HelpText = "")]
        public string? GasPriceGwei { get; set; }

        [Value(1, Required = false)]
        public IEnumerable<string>? Data { get; set; }
    }
}
