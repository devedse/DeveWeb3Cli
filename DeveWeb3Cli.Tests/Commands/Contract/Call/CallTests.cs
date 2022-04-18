using DeveWeb3Cli.Tests.TestHelpers;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveWeb3Cli.Tests.Commands.Contract.Call
{
    public class CallTests
    {
        [Fact]
        public async Task TestCallWithDataFromJson()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                using var tempFileOutput1 = TempFile.Create(".json");
                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --outputjson {tempFileOutput1} ExampleData\EthernalLock.json";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var stringOutput = File.ReadAllText(tempFileOutput1);
                //Only newtonsoft can work with parameterless constructors
                var transactionReceipt = JsonConvert.DeserializeObject<TransactionReceipt>(stringOutput);


                var tempFileDataInput = TempFile.Create(".json");
                File.WriteAllText(tempFileDataInput, "{'count': '2'}");


                string args2 = @$"contract call --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --address {transactionReceipt.ContractAddress} --abi ExampleData\EthernalLock.json --function setHighestValidLockType --jsondatafilepath {tempFileDataInput} 2";
                var result2 = await Program.Main(args2.Split(" "));
                Assert.Equal(0, result2);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

        [Fact]
        public async Task TestCallWithDataDirect()
        {
            using (var web3Container = Web3ContainerCreator.CreateContainer())
            {
                var web3 = TestWeb3Creator.GetWeb3(web3Container.RpcUrl);
                var ethBefore = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);

                using var tempFileOutput1 = TempFile.Create(".json");
                string args = @$"contract deploy --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --outputjson {tempFileOutput1} ExampleData\EthernalLock.json";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var stringOutput = File.ReadAllText(tempFileOutput1);
                //Only newtonsoft can work with parameterless constructors
                var transactionReceipt = JsonConvert.DeserializeObject<TransactionReceipt>(stringOutput);


                var tempFileDataInput = TempFile.Create(".json");
                File.WriteAllText(tempFileDataInput, "{'count': '2'}");


                string args2 = @$"contract call --network Private --rpc-url {web3Container.RpcUrl} --private-key {TestConstants.TestAccount1_PrivateKey} --address {transactionReceipt.ContractAddress} --abi ExampleData\EthernalLock.json --function setHighestValidLockType 2";
                var result2 = await Program.Main(args2.Split(" "));
                Assert.Equal(0, result2);

                var ethAfter = await web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccount1.Address);
                Assert.True(ethBefore.Value > ethAfter.Value);
            }
        }

    }
}
