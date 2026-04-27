using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.NUnit.Tests.Samples.AddDescriptionFromTestHtmlFromDispose
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetDescription("Lorem Ipsum");
        }

        public void Dispose()
        {
            AllureApi.SetDescriptionHtml("Dolor Sit Amet");
        }
    }
}



