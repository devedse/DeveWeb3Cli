using Ductus.FluentDocker.Services;
using System;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public class Web3Container : IDisposable
    {
        public IContainerService ContainerService { get; }
        public string RpcUrl { get; }

        public Web3Container(IContainerService containerService, string rpcUrl)
        {
            ContainerService = containerService;
            RpcUrl = rpcUrl;
        }

        public void Dispose()
        {
            ContainerService.Dispose();
        }
    }
}
