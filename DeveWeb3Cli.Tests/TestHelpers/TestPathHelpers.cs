using System.IO;
using System.Reflection;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public static class TestPathHelpers
    {
        public static string EthernalLockJson => @"ExampleData\EthernalLock.json".ToAbsPath();
        public static string TestJson => @"ExampleData\Test.json".ToAbsPath();
        public static string TestConstructorArgsJson => @"ExampleData\TestConstructorArgs.json".ToAbsPath();

        public static string ToAbsPath(this string relativePath)
        {
            var relativePathWithRightPathSeparator = relativePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            var curAssembly = Assembly.GetAssembly(typeof(TestPathHelpers))!.Location;
            var curDir = Path.GetDirectoryName(curAssembly);
            var totalPath = Path.Combine(curDir!, relativePathWithRightPathSeparator);
            var totalPathResolved = Path.GetFullPath(totalPath);
            return totalPathResolved;
        }
    }
}
