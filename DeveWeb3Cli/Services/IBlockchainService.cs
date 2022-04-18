using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace DeveWeb3Cli.Services
{
    public interface IBlockchainService
    {
        Task<TransactionReceipt> WaitForReceipt(Web3 web3, string transactionHash);
    }
}