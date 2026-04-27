using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetAllureIdFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            AllureApi.SetAllureId(1001);
        }

        [Fact]
        public void TestMethod() { }
    }
}



