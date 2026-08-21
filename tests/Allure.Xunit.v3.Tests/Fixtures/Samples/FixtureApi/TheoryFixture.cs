using System;
using System.Collections.Generic;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi
{
    public sealed class TheoryFixture : IDisposable
    {
        [AllureSetUp("Set up")]
        public TheoryFixture() { }

        [Theory]
        [MemberData(nameof(GetData), DisableDiscoveryEnumeration = true)]
        public void TestMethod(int _) { }

        [AllureTearDown("Tear down")]
        public void Dispose() { }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]{ 1 };
            yield return new object[]{ 2 };
        }
    }
}
