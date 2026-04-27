using System;
using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddTestParameters
{
    public class TestsClass : IDisposable
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddTestParameter("name1", "value-1");
            AllureApi.AddTestParameter("name2", "value-2", ParameterMode.Masked);
            AllureApi.AddTestParameter("name3", "value-3", ParameterMode.Hidden);
            AllureApi.AddTestParameter("name4", "value-4", excluded: true);
        }

        public void Dispose()
        {
            AllureApi.AddTestParameter("name5", "value-5", ParameterMode.Masked, true);
        }
    }
}



