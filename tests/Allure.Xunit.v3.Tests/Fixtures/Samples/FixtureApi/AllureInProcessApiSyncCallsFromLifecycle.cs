using System;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public sealed class AllureInProcessApiSyncCallsFromLifecycle : IDisposable
    {
        public AllureInProcessApiSyncCallsFromLifecycle()
        {
            AllureInProcessApi.SetUp("setup", context =>
            {
                context.SetName("AllureInProcessApi sync setup");
                context.AddParameter("context", "works");
            });
        }

        [Fact]
        public void TestMethod() { }

        public void Dispose()
        {
            AllureInProcessApi.TearDown("teardown", context =>
            {
                context.SetName("AllureInProcessApi sync teardown");
                context.AddParameter("context", "works");
            });
        }
    }
}
