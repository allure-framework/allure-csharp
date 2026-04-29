using System;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.NUnit.Tests.Samples.AddDescriptionFromDisposeHtmlFromTest
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.SetDescriptionHtml("Lorem Ipsum");
            }
        }

        public void Dispose()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.SetDescription("Dolor Sit Amet");
            }
        }
    }
}





