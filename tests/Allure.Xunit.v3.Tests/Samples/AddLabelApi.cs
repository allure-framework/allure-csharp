using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddLabelApi
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddLabel("test", "foo");
            }
        }

        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddLabel("dispose", "bar");
            }
        }
    }
}



