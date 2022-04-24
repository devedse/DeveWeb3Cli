using DeveWeb3Cli.Tests.TestHelpers;
using System.IO;
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

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} {TestPathHelpers.EthernalLockJson}";
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

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl}aoiwefj --private-key {TestConstants.TestAccount1_PrivateKey} {TestPathHelpers.EthernalLockJson}";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(1, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value == ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeployWithConstructorArgumentsAndPayable()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string calldata = "2 \"{ 'R': 128, 'G': 100, 'B': 12 }\" \"Hallo dit is een test\"";

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --value 1000000000000000 --abi {TestPathHelpers.TestConstructorArgsJson} {TestPathHelpers.TestConstructorArgsJson} {calldata}";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeployWithConstructorArgumentsAndPayableFromJson()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                var tempFileDataInput = TempFile.Create(".json");
                File.WriteAllText(tempFileDataInput, "{'number': '2', 'backgroundColor': { 'R': 128, 'G': 100, 'B': 12 }, 'txt': 'Hallo dit is een test' }");

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --value 1000000000000000 --abi {TestPathHelpers.TestConstructorArgsJson} --jsondatafilepath {tempFileDataInput} {TestPathHelpers.TestConstructorArgsJson}";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeploy_WithOldGasMechanism()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --gas-price 10_gwei {TestPathHelpers.EthernalLockJson}";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestDeploy_WithNewGasMechanism()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --maxFeePerGas 10.5_gwei --maxPriorityFeePerGas 2_gwei {TestPathHelpers.EthernalLockJson}";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }
    }
}
