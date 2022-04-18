using DeveWeb3Cli.Services;
using Ninject.Modules;

namespace DeveWeb3Cli.Modules
{
    /// <summary>
    /// Binds components which will be injected to commands.
    /// </summary>
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBlockchainService>().To<BlockchainService>();
        }
    }
}
