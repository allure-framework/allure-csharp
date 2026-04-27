using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddLabelApi
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddLabel("test", "foo");
        }

        public void Dispose()
        {
            AllureApi.AddLabel("dispose", "bar");
        }
    }
}



