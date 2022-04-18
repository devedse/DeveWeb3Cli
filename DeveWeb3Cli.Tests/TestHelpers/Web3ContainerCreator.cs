using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services.Extensions;
using System;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public static class Web3ContainerCreator
    {
        public static Web3Container CreateContainer()
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
            var startedContainer = dockerContainer.Start();

            var endpoint = dockerContainer.ToHostExposedEndpoint("8545/tcp");

            var rpcurl = $"http://localhost:{endpoint.Port}";

            return new(startedContainer, rpcurl);
        }
    }
}
