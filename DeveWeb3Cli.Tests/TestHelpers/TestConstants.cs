using Nethereum.Web3.Accounts;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public static class TestConstants
    {
        public const string TestAccount1_PrivateKey = "0x6e8cf75327b7c5b1701d8b1f60b5c500958a6614b9f4d9d119a79ee5bb7d9549";
        public static Account TestAccount1 => new Account(TestAccount1_PrivateKey, 1337);
    }
}
