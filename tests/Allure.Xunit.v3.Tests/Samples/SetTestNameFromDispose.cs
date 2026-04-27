using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetTestNameFromDispose
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod() { }

        public void Dispose()
        {
            AllureApi.SetTestName("Lorem Ipsum");
        }
    }
}



