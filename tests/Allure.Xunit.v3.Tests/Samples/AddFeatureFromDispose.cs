using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddFeatureFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddFeature("foo");
            }
        }

        [Fact]
        public void TestMethod() { }
    }
}



