using System;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public sealed class AllureApiSyncCallsFromLifecycle : IDisposable
    {
        public AllureApiSyncCallsFromLifecycle()
        {
            AllureApi.SetUp("setup", context =>
            {
                context.SetName("AllureApi sync setup");
                context.AddParameter("context", "works");
            });
        }

        [Fact]
        public void TestMethod() { }

        public void Dispose()
        {
            AllureApi.TearDown("teardown", context =>
            {
                context.SetName("AllureApi sync teardown");
                context.AddParameter("context", "works");
            });
        }
    }
}
