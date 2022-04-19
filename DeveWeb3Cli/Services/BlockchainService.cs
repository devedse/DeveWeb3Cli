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
                Console.WriteLine("Still waiting...");
                await Task.Delay(5000);
                receipt = await GetReceipt();
            }

            Console.WriteLine($"Got receipt for TransactionHash: {transactionHash}. Result: {receipt.Status.Value}");

            return receipt;
        }
    }
}
