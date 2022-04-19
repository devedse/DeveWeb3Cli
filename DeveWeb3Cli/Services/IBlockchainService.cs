using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace DeveWeb3Cli.Services
{
    public interface IBlockchainService
    {
        Task<TransactionReceipt> WaitForReceipt(Web3 web3, string transactionHash);

        object[] CreateInputData(Parameter[] parameters, string? JsonDataFilePath, IEnumerable<string>? Data);
    }
}