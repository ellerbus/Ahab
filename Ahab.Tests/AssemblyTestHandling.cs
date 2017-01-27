using System;
using System.IO;
using Ahab.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ahab.Tests
{
    public static class AssemblyTestHandling
    {
        [AssemblyInitialize()]
        public static void Initialize(TestContext testContext)
        {
            string file = Path.Combine(Configuration.CacheDirectory, "MSFT.yprices");

            File.SetLastWriteTime(file, DateTime.Now);
        }
    }
}
