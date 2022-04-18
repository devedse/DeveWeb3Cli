using System;
using System.IO;

namespace DeveWeb3Cli.Tests.TestHelpers
{
    public class TempFile : IDisposable
    {
        public string FilePath { get; }

        public static TempFile Create(string extension) => new TempFile(Guid.NewGuid().ToString().Replace("-", "") + "." + extension.TrimStart('.'));

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        public TempFile(string filePath)
        {
            FilePath = filePath;
        }

        public override string ToString()
        {
            return FilePath;
        }

        public static implicit operator string(TempFile value)
        {
            return value.FilePath;
        }
    }
}
