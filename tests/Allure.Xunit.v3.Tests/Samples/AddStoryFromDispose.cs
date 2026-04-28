using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddStoryFromDispose
{
    public class TestsClass : IDisposable
    {
        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddStory("foo");
            }
        }

        [Fact]
        public void TestMethod() { }
    }
}





