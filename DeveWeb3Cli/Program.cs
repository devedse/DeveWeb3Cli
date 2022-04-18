using CommandLine;
using DeveWeb3Cli.CommandHelpers;
using DeveWeb3Cli.Commands.Contract;
using DeveWeb3Cli.Modules;
using Ninject;
using System;

namespace DeveWeb3Cli
{
    public class Program
    {
        private static IKernel kernel;
        private static int returnValue;

        public static async Task<int> Main(string[] args)
        {
            // Composition root is here... we load the injector and modules
            // Business behavior is determined by modules, so commands stay loosely coupled.
            kernel = new StandardKernel(new AppModule());
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

        private static async Task ProcessArgs(IEnumerable<string> args)
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

        private static async Task ExecuteCommand(object arg)
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

        private static void ParseError(IEnumerable<Error> obj) => returnValue = 1;
    }
}