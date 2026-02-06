using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddEpicFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            AllureApi.AddEpic("foo");
        }

        [Fact]
        public void TestMethod() { }
    }
}
