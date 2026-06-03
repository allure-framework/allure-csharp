using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.BddLabels.Samples.AddStoryFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            AllureApi.AddStory("foo");
        }

        [Fact]
        public void TestMethod() { }
    }
}
