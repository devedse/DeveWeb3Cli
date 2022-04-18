using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services.Extensions;
using DeveWeb3Cli.Tests.TestHelpers;

namespace DeveWeb3Cli.Tests.Commands.Contract.Deploy
{
    public class DeployTests
    {
        [Fact]
        public async Task TestDeploy()
        {
            var dockerContainer = new Builder()
                   .UseContainer()
                   .UseImage("trufflesuite/ganache")
                   .ExposePort(0, 8545)
                   .WaitForPort("8545/tcp", TimeSpan.FromSeconds(30), "127.0.0.1")
                   .WaitForMessageInLog("RPC Listening on 0.0.0.0:8545", TimeSpan.FromSeconds(30))
                   .Command("--wallet.seed testseed")
                   //.KeepRunning()
                   //.KeepContainer()
                   .Build();
            using (dockerContainer.Start())
            {
                var endpoint = dockerContainer.ToHostExposedEndpoint("8545/tcp");

                var rpcurl = $"http://localhost:{endpoint.Port}";

                string args = @$"contract deploy --network Private --rpc-url {rpcurl} --private-key {TestConstants.TestAccount1_PrivateKey} ExampleData\EthernalLock.json";
                var result = await Program.Main(args.Split(" "));
                Assert.Equal(0, result);

                var web3 = TestWeb3Creator.GetWeb3(endpoint.Port);
            }
        }
    }
}
