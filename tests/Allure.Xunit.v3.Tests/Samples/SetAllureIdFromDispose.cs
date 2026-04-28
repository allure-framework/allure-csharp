using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetAllureIdFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.SetAllureId(1001);
            }
        }

        [Fact]
        public void TestMethod() { }
    }
}



