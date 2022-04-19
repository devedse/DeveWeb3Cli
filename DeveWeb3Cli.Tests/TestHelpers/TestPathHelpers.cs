using System.IO;
using System.Reflection;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public static class TestPathHelpers
    {
        public static string EthernalLockJson => @"ExampleData\EthernalLock.json".ToAbsPath();
        public static string TestJson => @"ExampleData\Test.json".ToAbsPath();

        public static string ToAbsPath(this string relativePath)
        {
            var curAssembly = Assembly.GetAssembly(typeof(TestPathHelpers))!.Location;
            var curDir = Path.GetDirectoryName(curAssembly);
            var path1 = Path.Combine(curDir, relativePath);
            var path2 = Path.GetFullPath(path1);
            return path2;
        }
    }
}
