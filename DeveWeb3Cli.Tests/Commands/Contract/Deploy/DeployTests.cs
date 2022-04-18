using DeveWeb3Cli.Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace DeveWeb3Cli.Tests.Commands.Contract.Deploy
{
    public class DeployTests
    {
        [Fact]
        public async Task TestDeploy()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} ExampleData\EthernalLock.json";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeployFailsWithWrongArguments()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl}aoiwefj --private-key {TestConstants.TestAccount1_PrivateKey} ExampleData\EthernalLock.json";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(1, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value == ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeployWithData()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} ExampleData\EthernalLock.json data1 data2 data3";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }
    }
}
