namespace DeveWeb3Cli.WasiSdk
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            Console.WriteLine($"Hello this is compiled code from WASM, I can do calculations: {new Random().Next()}");

            return DeveWeb3Cli.Program.Main(args);
        }
    }
}