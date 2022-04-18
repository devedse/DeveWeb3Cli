using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace DeveWeb3Cli.Services
{
    public class BlockchainService : IBlockchainService
    {
        public BlockchainService()
        {

        }

        public async Task<TransactionReceipt> WaitForReceipt(Web3 web3, string transactionHash)
        {
            Console.WriteLine("Waiting for receipt...");

            Task<TransactionReceipt> GetReceipt()
            {
                return web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            var receipt = await GetReceipt();

            while (receipt == null)
            {
                await Task.Delay(5000);
                receipt = await GetReceipt();
            }

            return receipt;
        }
    }
}
