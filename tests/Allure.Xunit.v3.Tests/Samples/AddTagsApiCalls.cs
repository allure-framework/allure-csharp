using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddTagsApiCalls
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddTags("foo", "bar");
            }
        }

        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddTags("baz");
            }
        }
    }
}



