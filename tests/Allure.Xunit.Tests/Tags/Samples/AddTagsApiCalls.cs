using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Tags.Samples.AddTagsApiCalls
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddTags("foo", "bar");
        }

        public void Dispose()
        {
            AllureApi.AddTags("baz");
        }
    }
}
