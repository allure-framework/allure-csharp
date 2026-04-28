using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddEpicFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddEpic("foo");
            }
        }

        [Fact]
        public void TestMethod() { }
    }
}



