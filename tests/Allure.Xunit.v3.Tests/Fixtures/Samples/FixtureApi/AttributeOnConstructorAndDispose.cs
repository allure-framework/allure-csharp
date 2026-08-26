using System;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public sealed class AttributeOnConstructorAndDispose : IDisposable
    {
        [AllureSetUp("Constructor setup")]
        public AttributeOnConstructorAndDispose() { }

        [Fact]
        public void TestMethod() { }

        [AllureTearDown("Dispose teardown")]
        public void Dispose() { }
    }
}
