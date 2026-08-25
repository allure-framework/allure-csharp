using System;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public class NestedAllureInProcessApiSyncFixtures
    {
        [Fact]
        public void TestMethod()
        {
            AllureInProcessApi.SetUp(
                "Outer AllureInProcessApi sync fixture",
                _ =>
                {
                    try
                    {
                        AllureInProcessApi.SetUp("Inner fixture", _ => { });
                    }
                    catch (InvalidOperationException) { }
                }
            );
        }
    }
}
