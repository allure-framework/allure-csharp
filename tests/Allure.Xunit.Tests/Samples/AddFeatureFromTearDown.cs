using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddFeatureFromTearDown
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            AllureApi.AddFeature("foo");
        }

        [Fact]
        public void TestMethod() { }
    }
}
