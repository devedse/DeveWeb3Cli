using CommandLine;
using DeveWeb3Cli.CommandHelpers;
using DeveWeb3Cli.Commands.Contract;
using DeveWeb3Cli.Modules;
using Ninject;

namespace DeveWeb3Cli
{
    public class Program
    {
        private IKernel kernel;
        private int returnValue;

        public static Task<int> Main(string[] args)
        {
            var program = new Program();
            return program.ProcessCommandLineArguments(args);
        }

        public Program()
        {
            // Composition root is here... we load the injector and modules
            // Business behavior is determined by modules, so commands stay loosely coupled.
            kernel = new StandardKernel(new AppModule());
        }

        private async Task<int> ProcessCommandLineArguments(string[] args)
        {
            try
            {
                await ProcessArgs(args);
                return returnValue;
            }
            finally
            {
                kernel.Dispose();
            }
        }

        private async Task ProcessArgs(IEnumerable<string> args)
        {
            try
            {
                // We can use generic helpers or pass the command types manually.
                var parsedArguments = Parser.Default.ParseVerbs<ContractCommand>(args);

                await parsedArguments.WithParsedAsync(ExecuteCommand);
                await parsedArguments.WithNotParsedAsync(ExecuteCommand);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parser exception: {ex.Message}");
            }
        }

        private async Task ExecuteCommand(object arg)
        {
            CommandBase command;
            try
            {
                command = (CommandBase)arg;

                // The kernel will inject dependencies to the command.
                kernel.Inject(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to resolve command dependencies: {ex.Message}");
                returnValue = 1;
                return;
            }

            try
            {
                // Actual work is done here.
                await command.Execute();
            }
            catch (Exception ex)
            {
                // Ideally this code should never execute.
                Console.WriteLine(ex.Message);
                if (command.Verbose)
                {
                    Console.WriteLine(Environment.NewLine + "=== Exception ===");
                    Console.WriteLine(ex.ToString());
                }

                returnValue = 1;
            }
        }

        private void ParseError(IEnumerable<Error> obj) => returnValue = 1;
    }
}