using System;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class NestedAllureApiSyncFixtures
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetUp(
                "Outer AllureApi sync fixture",
                () =>
                {
                    try
                    {
                        AllureApi.SetUp("Inner fixture", () => { });
                    }
                    catch (InvalidOperationException) { }
                }
            );
        }
    }
}
