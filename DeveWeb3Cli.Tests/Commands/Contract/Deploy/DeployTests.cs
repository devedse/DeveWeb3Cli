using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Ductus.FluentDocker.Builders;

namespace DeveWeb3Cli.Tests.Commands.Contract.Deploy
{
    public class DeployTests
    {
        [Fact]
        public async Task TestDeploy()
        {
            using (var dockerContainerPosgres = new Builder()
                   .UseContainer()
                   .UseImage("trufflesuite/ganache-cli")
                   .ExposePort(0)
                   .WaitForPort("8545/tcp", TimeSpan.FromSeconds(30), "127.0.0.1")
                   .WaitForMessageInLog("Listening on", TimeSpan.FromSeconds(30))
                   .WaitForProcess("node")
                   //.KeepRunning()
                   //.KeepContainer()
                   .Build()
                   .Start())
            {

            }
        }
    }
}
