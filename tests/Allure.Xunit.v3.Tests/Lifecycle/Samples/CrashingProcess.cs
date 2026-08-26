using System;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.CrashingProcess
{
    public class TestClass
    {
        [Fact]
        public void TestMethod()
        {
            Environment.FailFast("Crash from sample test.");
        }
    }
}
