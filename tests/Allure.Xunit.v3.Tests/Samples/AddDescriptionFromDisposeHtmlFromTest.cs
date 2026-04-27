using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.NUnit.Tests.Samples.AddDescriptionFromDisposeHtmlFromTest
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetDescriptionHtml("Lorem Ipsum");
        }

        public void Dispose()
        {
            AllureApi.SetDescription("Dolor Sit Amet");
        }
    }
}



