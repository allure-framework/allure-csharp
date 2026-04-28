using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetTestNameFromDispose
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod() { }

        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.SetTestName("Lorem Ipsum");
            }
        }
    }
}





