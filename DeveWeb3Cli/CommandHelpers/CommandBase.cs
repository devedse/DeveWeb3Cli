using CommandLine;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveWeb3Cli.CommandHelpers
{
    /// <summary>
    /// Base class that serves as an interface for execution 
    /// and provides common properties and components for commands.
    /// </summary>
    /// <remarks>
    /// General pattern is that commands that have child verbs should be left abstract
    /// and provide utilities, while leaf commands that do the actual work should be concrete.
    /// </remarks>
    public abstract class CommandBase
    {
        // Properties shared by all commands...
        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [Option('n', "network", Env = "WEB3_NETWORK", Default = "1337", HelpText = "The name of the network. Options: <chainId>, MainNet, Morden, Ropsten, Sepolia, Rinkeby, RootstockMainNet, RootstockTestNet, Kovan, ClassicMainNet, ClassicTestNet, Private. (default: \"1337\") [$WEB3_NETWORK]")]
        public string Network { get; set; } = null!;

        protected int GetChainId()
        {
            if (int.TryParse(Network, out var chainId))
            {
                return chainId;
            }
            var chains = Enum.GetValues<Chain>();
            var foundChains = chains.Where(t => t.ToString().Equals(Network, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!foundChains.Any())
            {
                throw new ArgumentException($"Could not find chain with name: {Network}");
            }
            return (int)foundChains.First();
        }

        //// Components shared by all commands... you can inject a database connection etc.
        //[Inject]
        //public ILogWriter LogWriter { get; set; }

        //// Methods shared by all commands...
        //protected void WriteLog(string value) => LogWriter?.Log(value);

        //protected void WriteError(string value) => LogWriter?.Error(value);

        //protected void WriteVerbose(string value)
        //{
        //    if (Verbose)
        //    {
        //        LogWriter?.Log(value);
        //    }
        //}

        // Interface by which we execute our commands.
        public abstract Task Execute();
    }
}
