using Nethereum.Web3;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public static class TestWeb3Creator
    {
        public static Web3 GetWeb3(string rpcUrl) => new Web3(TestConstants.TestAccount1, rpcUrl);
    }
}
