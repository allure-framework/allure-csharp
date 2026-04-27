using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LinkRuntimeApi
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddLink("url-1");
            AllureApi.AddLink("name-2", "type-2", "url-2");
            AllureApi.AddIssue("url-3");
            AllureApi.AddTmsItem("url-4");
        }

        public void Dispose()
        {
            AllureApi.AddLink("name-5", "url-5");
            AllureApi.AddIssue("name-6", "url-6");
            AllureApi.AddTmsItem("name-7", "url-7");
        }
    }
}



